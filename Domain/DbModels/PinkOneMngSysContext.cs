using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Domain
{
    public partial class PinkOneMngSysContext : DbContext
    {
        DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
        
        public PinkOneMngSysContext()
        {
            
        }

        public PinkOneMngSysContext(DbContextOptions<PinkOneMngSysContext> options)
            : base(options)
        {
        }

        public virtual DbSet<FK_Stu_Parent> FK_Stu_Parent { get; set; }
        public virtual DbSet<SYS_Admin> SYS_Admin { get; set; }
        public virtual DbSet<SYS_Album> SYS_Album { get; set; }
        public virtual DbSet<SYS_Card> SYS_Card { get; set; }
        public virtual DbSet<SYS_Class> SYS_Class { get; set; }
        public virtual DbSet<SYS_Notice> SYS_Notice { get; set; }
        public virtual DbSet<SYS_Parent> SYS_Parent { get; set; }
        public virtual DbSet<SYS_PhotoRecord> SYS_PhotoRecord { get; set; }
        public virtual DbSet<SYS_School> SYS_School { get; set; }
        public virtual DbSet<SYS_Staff> SYS_Staff { get; set; }
        public virtual DbSet<SYS_StaffAttRecord> SYS_StaffAttRecord { get; set; }
        public virtual DbSet<SYS_StaffRole> SYS_StaffRole { get; set; }
        public virtual DbSet<SYS_Student> SYS_Student { get; set; }
        public virtual DbSet<SYS_StudentAttRecord> SYS_StudentAttRecord { get; set; }
        public virtual DbSet<Wx_Setting> Wx_Setting { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer("Server=212.64.49.60;Database=PinkOneMngSys;user id=admin;password=Pinkone_2019;");
        //    }
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FK_Stu_Parent>(entity =>
            {
                entity.HasIndex(e => e.SchoolId)
                    .HasName("NonClusteredIndex-20190326-221155");

                entity.Property(e => e.OpenId).HasMaxLength(100);
            });

            modelBuilder.Entity<SYS_Admin>(entity =>
            {
                entity.Property(e => e.AvatarPic).HasMaxLength(100);

                entity.Property(e => e.CreateTime).HasColumnType("smalldatetime");

                entity.Property(e => e.Creater).HasMaxLength(10);

                entity.Property(e => e.IdNumber).HasMaxLength(50);

                entity.Property(e => e.OpenId).HasMaxLength(100);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PersonName).HasMaxLength(10);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<SYS_Album>(entity =>
            {
                entity.HasIndex(e => e.SchoolId)
                    .HasName("NonClusteredIndex-20190509-195250");

                entity.Property(e => e.AlbumName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<SYS_Card>(entity =>
            {
                entity.Property(e => e.CardMoney).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.CardNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreateTime).HasColumnType("smalldatetime");

                entity.Property(e => e.Remark).HasMaxLength(50);
            });

            modelBuilder.Entity<SYS_Class>(entity =>
            {
                entity.Property(e => e.ClassName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ClassNo).HasMaxLength(50);
            });

            modelBuilder.Entity<SYS_Notice>(entity =>
            {
                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.StartDate).HasColumnType("date");
            });

            modelBuilder.Entity<SYS_Parent>(entity =>
            {
                entity.HasKey(e => e.ID)
                    .ForSqlServerIsClustered(false);

                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.CreatTime).HasColumnType("smalldatetime");

                entity.Property(e => e.IdNumber).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.NickName).HasMaxLength(50);

                entity.Property(e => e.OpenId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Remark).HasMaxLength(50);
            });

            modelBuilder.Entity<SYS_PhotoRecord>(entity =>
            {
                entity.HasIndex(e => e.AlbumId)
                    .HasName("NonClusteredIndex-20190509-195320");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Describe).HasMaxLength(20);

                entity.Property(e => e.ImgUrl).IsRequired();

                entity.Property(e => e.SizeStr)
                    .IsRequired()
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<SYS_School>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.AssociatedStaffOpenId1).HasMaxLength(100);

                entity.Property(e => e.AssociatedStaffOpenId2).HasMaxLength(100);

                entity.Property(e => e.AssociatedStaffOpenId3).HasMaxLength(100);

                entity.Property(e => e.AssociatedStaffOpenId4).HasMaxLength(100);

                entity.Property(e => e.AssociatedStaffOpenId5).HasMaxLength(100);

                entity.Property(e => e.ContactInfo).HasMaxLength(20);

                entity.Property(e => e.CreateTime).HasColumnType("smalldatetime");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.SchoolName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<SYS_Staff>(entity =>
            {
                entity.Property(e => e.AvatarPic).HasMaxLength(100);

                entity.Property(e => e.CardNo).HasMaxLength(50);

                entity.Property(e => e.CreateTime).HasColumnType("smalldatetime");

                entity.Property(e => e.IdNumber).HasMaxLength(50);

                entity.Property(e => e.NickName).HasMaxLength(50);

                entity.Property(e => e.OpenId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.Remark).HasMaxLength(50);

                entity.Property(e => e.StaffName).HasMaxLength(10);

                entity.Property(e => e.WorkNo).HasMaxLength(50);
            });

            modelBuilder.Entity<SYS_StaffAttRecord>(entity =>
            {
                entity.HasIndex(e => e.AttTime)
                    .HasName("NonClusteredIndex-20190326-214451");

                entity.Property(e => e.AttTime).HasColumnType("datetime");

                entity.Property(e => e.AttTimeStr)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Remark).HasMaxLength(50);
            });

            modelBuilder.Entity<SYS_StaffRole>(entity =>
            {
                entity.HasIndex(e => e.SchoolId)
                    .HasName("NonClusteredIndex-20190405-170750");

                entity.Property(e => e.Remark).HasMaxLength(50);

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<SYS_Student>(entity =>
            {
                entity.HasIndex(e => e.SchoolId)
                    .HasName("NonClusteredIndex-20190326-215104");

                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.AvatarPic).HasMaxLength(100);

                entity.Property(e => e.Birthday).HasColumnType("smalldatetime");

                entity.Property(e => e.CreateTime).HasColumnType("smalldatetime");

                entity.Property(e => e.Creater).HasMaxLength(10);

                entity.Property(e => e.IdNumber).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.Remark).HasMaxLength(50);

                entity.Property(e => e.StuName)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.StuNo).HasMaxLength(50);
            });

            modelBuilder.Entity<SYS_StudentAttRecord>(entity =>
            {
                entity.HasIndex(e => e.AttTime)
                    .HasName("NonClusteredIndex-20190326-214336");

                entity.Property(e => e.AttTime).HasColumnType("datetime");

                entity.Property(e => e.AttTimeStr)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MonitoringImg).HasMaxLength(100);

                entity.Property(e => e.Remark).HasMaxLength(50);

                entity.Property(e => e.Temperature).HasMaxLength(5);
            });

            modelBuilder.Entity<Wx_Setting>(entity =>
            {
                entity.Property(e => e.ID)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.AppId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.AppName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.AppSecret)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasColumnType("smalldatetime");
            });
        }
    }
}
