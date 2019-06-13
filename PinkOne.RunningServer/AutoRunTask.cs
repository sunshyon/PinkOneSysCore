using Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utility;

namespace PinkOne.RunningServer
{
    public class AutoRunProgram
    {
        private static int _interval = 30;//分钟
        private Thread attMsgThread;
        private Task updateTask;
        private AdoNetHelper adoHelper;
        private string cnnStr;

        #region 构造函数、对象实例、内嵌延时实体
        /// <summary>
        /// 构造函数
        /// </summary>
        public AutoRunProgram()
        {
            if (int.TryParse(ConfigHelper.AppSettings("AutoUpdateAttStatusInterval"), out _interval))
            {
                cnnStr = "Server=.;Database=PinkOneMngSys;user id=admin;password=Pinkone_2019;";
#if DEBUG
                cnnStr = "Server=212.64.49.60;Database=PinkOneMngSys;user id=admin;password=Pinkone_2019;";
#endif
                adoHelper = new AdoNetHelper(cnnStr);

                //更新数据
                updateTask = new Task(Update_Handle);
                updateTask.Start();

            }

        }
        public static AutoRunProgram Instance
        {
            get
            {
                return Nested.arp;
            }
        }
        private class Nested
        {
            static Nested() { }
            internal static readonly AutoRunProgram arp
                = new AutoRunProgram();
        }
        #endregion

        #region 方法区域
        public void StartUp()
        {
        }
        #endregion

        #region 委托事件区域
        /// <summary>
        /// 时钟触发事件
        /// </summary>
        private void Update_Handle()
        {
            var sleep = 2*1000;//2s
            while (true)
            {
                Thread.Sleep(sleep);//休眠

                sleep =UpdateWxToken();//更新微信票据

                UpdateAttStatus();//更新考勤状态

                Thread.Sleep(sleep);//休眠 半小时
            }
        }
        #endregion

        #region 函数
        private int UpdateWxToken()
        {
            var sleep = 1000*60*30;//30min
            try
            {
                var dbContext = new PinkOneMngSysContext();
                var wxPubInfos = dbContext.Wx_PublicInfo.Where(x => true).ToList();
                foreach (var wpi in wxPubInfos)
                {
                    ModelWxToken token = WXOAuthApiHelper.GetWxToken(wpi.AppId,wpi.AppSecret);
                    if (null != token && token.access_token != null)
                    {
                        ModelWxJsTicket ticket = WXOAuthApiHelper.GetWxJsTicket(token);
                        if (null != ticket)
                        {
                            string sql = "update Wx_PublicInfo set UpdateTime='{0}', AccessToken='{1}', JsApiTicket='{2}' where ID=" + wpi.ID;
                            sql = string.Format(sql, DateTime.Now, token.access_token, ticket.ticket);
                            var isOK= adoHelper.ExecuteNonQueryCmd(sql);
                            if (isOK > 0)
                            {
                                Console.WriteLine("UpdateWxToken："+DateTime.Now+" "+wpi.AppName+"Token更新成功");
                            }
                        }
                        //sleep = token.expires_in * 60;
                    }
                    else
                    {
                        Console.WriteLine("UpdateWxToken：" + DateTime.Now + " " + wpi.AppName + "Token更新失败，原因->"+token.errmsg);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("UpdateWxToken：" + DateTime.Now + " " + "出现错误，" + e.Message);
                //LogHelper.Error(e.Message);
            }
            return sleep;
        }

        private void UpdateAttStatus()
        {
            try
            {
                //操作
                if (DateTime.Now.Hour >= 0&& DateTime.Now.Hour<=2)
                {
                    string sql = "update SYS_Student set AttStatus=3 where Status=1 and AttStatus<>3";
                    var p = new[]
                    {
                        new SqlParameter(),
                        new SqlParameter(),
                    };
                    var isOK = adoHelper.ExecuteNonQueryCmd(sql);
                    if (isOK > 0)
                    {
                        Console.WriteLine("UpdateAttStatus：" + DateTime.Now + " " + "所有学生考勤状态复位成功");
                    }
                }
                if (DateTime.Now.Hour >=1&& DateTime.Now.Hour <= 3)
                {
                    string sql = "update SYS_Staff set AttStatus=3 where Status=1 and AttStatus<>3";
                    var p = new[]
                    {
                        new SqlParameter(),
                        new SqlParameter(),
                    };
                    var isOK= adoHelper.ExecuteNonQueryCmd(sql);
                    if (isOK > 0)
                    {
                        Console.WriteLine("UpdateAttStatus：" + DateTime.Now + " " + "所有老师考勤状态复位成功");
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("UpdateAttStatus：" + DateTime.Now + " " + "出现错误，" + e.Message);
                //LogHelper.Error("自动更新考勤状态程序报错" + e.Message);
            }

        }
        #endregion
    }
}
