using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace PinkOneSysCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //实现属性注入https://cloud.tencent.com/developer/article/1328862
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

            //cookie
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            //session
            services.AddSession(options =>
            {
                // 设置 Session 过期时间5分钟
                options.IdleTimeout = TimeSpan.FromMinutes(5);
            });
            //MemoryCache缓存
            services.AddMemoryCache();

            //添加HttpContext
            services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();

            //数据库连接
            services.AddDbContext<PinkOneMngSysContext>(options => options.UseSqlServer("Server=212.64.49.60;Database=PinkOneMngSys;user id=admin;password=Pinkone_2019;"));

            //SignalR
            services.AddSignalR();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                //全局配置Json序列化处理
                .AddJsonOptions(options=> {
                    //忽略循环引用
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    //不使用驼峰样式的key(首字母小写)
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    //设置时间格式
                    ///options.SerializerSettings.DateFormatString = "yyyy-MM-dd";
                });
            
            //Autofac注册
            return RegisterAutofac(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    app.UseHsts();
            //}

            app.UseMiddleware<Utility.ErrorHandleMiddleware>();

            //app.UseHttpsRedirection();//https
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseSignalR(routes=> {
                routes.MapHub<StaffChatHub>("/staffChatHub");
            });

            //配置HttpContext
            Utility.HttpContextCore.Configure(app.ApplicationServices.GetRequiredService<Microsoft.AspNetCore.Http.IHttpContextAccessor>());

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                  name: "areas",
                  template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
            });

        }


        #region Autofac注册
        public IContainer ApplicationContainer { get; private set; }

        private IServiceProvider RegisterAutofac(IServiceCollection services)
        {
            var assembly = this.GetType().GetTypeInfo().Assembly;
            var builder = new ContainerBuilder();
            var manager = new ApplicationPartManager();

            manager.ApplicationParts.Add(new AssemblyPart(assembly));
            manager.FeatureProviders.Add(new ControllerFeatureProvider());

            var feature = new ControllerFeature();

            manager.PopulateFeature(feature);

            //var optionsBuilder = new DbContextOptionsBuilder<PinkOneMngSysContext>();
            //optionsBuilder.UseSqlServer("Server=212.64.49.60;Database=PinkOneMngSys;user id=admin;password=Pinkone_2019;", b => b.MigrationsAssembly("Domain"));
            //builder.RegisterType<PinkOneMngSysContext>()
            //    .As<PinkOneMngSysContext>()
            //    .WithMetadata("options", optionsBuilder.Options)
            //    .InstancePerLifetimeScope();

            builder.RegisterType<ApplicationPartManager>().AsSelf().SingleInstance();
            builder.RegisterTypes(feature.Controllers.Select(ti => ti.AsType()).ToArray()).PropertiesAutowired();
            builder.Populate(services);


            builder.RegisterAssemblyTypes(Assembly.Load("DataService"))
               .Where(t => t.Name.EndsWith("Service"))
               .AsImplementedInterfaces()
               .UsingConstructor()
               .PropertiesAutowired()
               .InstancePerLifetimeScope();

            this.ApplicationContainer = builder.Build();

            return new AutofacServiceProvider(this.ApplicationContainer);
        }

        #endregion
    }
}
