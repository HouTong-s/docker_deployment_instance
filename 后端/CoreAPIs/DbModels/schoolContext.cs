using System;
using CoreAPIs.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
#nullable disable

namespace CoreAPIs.DbModels
{
    public partial class schoolContext : DbContext
    {
        public schoolContext()
        {
        }

        public schoolContext(DbContextOptions<schoolContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Enrollment> Enrollments { get; set; }
        public virtual DbSet<Information> Information { get; set; }
        public virtual DbSet<Lesson> Lessons { get; set; }
        public virtual DbSet<LessonRequirement> LessonRequirements { get; set; }
        public virtual DbSet<Notice> Notices { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<ScheduleTime> ScheduleTimes { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql(SettingsReader.GetMySQLConnectionString(), Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.28-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("admin");

                entity.HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.AdminId)
                    .ValueGeneratedNever()
                    .HasColumnName("admin_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("password");

                entity.Property(e => e.Salt)
                    .HasMaxLength(45)
                    .HasColumnName("salt")
                    .HasComment("MD5码的盐");
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => new { e.StudentId, e.ScheduleId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("enrollment");

                entity.HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");

                entity.Property(e => e.GradePoint)
                    .HasColumnName("grade_point")
                    .HasComment("绩点");

                entity.Property(e => e.GradeStatus)
                    .HasMaxLength(45)
                    .HasColumnName("grade_status")
                    .HasComment("成绩状况");

                entity.Property(e => e.InputTime)
                    .HasColumnType("datetime")
                    .HasColumnName("input_time")
                    .HasComment("成绩录入时间");

                entity.Property(e => e.Score)
                    .HasColumnName("score")
                    .HasComment("分数");

                entity.Property(e => e.SelectStatus)
                    .HasColumnName("select_status")
                    .HasComment("1为正常选课，2为重修选课");
            });

            modelBuilder.Entity<Information>(entity =>
            {
                entity.ToTable("information");

                entity.HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Id)
                    .HasMaxLength(45)
                    .HasColumnName("id");

                entity.Property(e => e.CanImportGrade)
                    .HasColumnName("can_import_grade")
                    .HasComment("能否导入成绩");

                entity.Property(e => e.GradeBeginTime)
                    .HasColumnType("datetime")
                    .HasColumnName("grade_begin_time")
                    .HasComment("导入成绩开始时间");

                entity.Property(e => e.GradeEndTime)
                    .HasColumnType("datetime")
                    .HasColumnName("grade_end_time")
                    .HasComment("导入成绩截止时间");

                entity.Property(e => e.Half)
                    .HasColumnName("half")
                    .HasComment("上下半年,0为上,1为下");

                entity.Property(e => e.SelectBeginTime)
                    .HasColumnType("datetime")
                    .HasColumnName("select_begin_time")
                    .HasComment("选课开始时间");

                entity.Property(e => e.SelectEndTime)
                    .HasColumnType("datetime")
                    .HasColumnName("select_end_time")
                    .HasComment("选课截止时间");

                entity.Property(e => e.SelectStatus)
                    .HasColumnName("select_status")
                    .HasComment("选课状态，0为不能选课，1为正常选课，2为重修选课");

                entity.Property(e => e.SemesterBeginTime)
                    .HasColumnType("datetime")
                    .HasColumnName("semester_begin_time")
                    .HasComment("学期开始时间");

                entity.Property(e => e.Year)
                    .HasColumnName("year")
                    .HasComment("年份");
            });

            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.ToTable("lesson");

                entity.HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.LessonId).HasColumnName("lesson_id");

                entity.Property(e => e.Credit)
                    .HasPrecision(3, 1)
                    .HasColumnName("credit")
                    .HasComment("课程占几个学分");

                entity.Property(e => e.Identity)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("identity")
                    .HasComment("本科生研究生还是博士生");

                entity.Property(e => e.LessonName)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("lesson_name");

                entity.Property(e => e.NeedDepart)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("need_depart")
                    .HasComment("什么专业能选,若为 'all'则都可以选");

                entity.Property(e => e.Note)
                    .HasMaxLength(45)
                    .HasColumnName("note")
                    .HasComment("备注");

                entity.Property(e => e.PreqId)
                    .HasColumnName("preq_id")
                    .HasComment("替代的以往课程的id");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("type")
                    .HasComment("属于哪一大类的课程");
            });

            modelBuilder.Entity<LessonRequirement>(entity =>
            {
                entity.HasKey(e => new { e.LessonId, e.InYear })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("lesson_requirement");

                entity.Property(e => e.LessonId).HasColumnName("lesson_id");

                entity.Property(e => e.InYear)
                    .HasColumnName("in_year")
                    .HasComment("哪一级能选(比如2018级)");

                entity.Property(e => e.MaxGrade)
                    .HasColumnName("max_grade")
                    .HasComment("最高几年级能选");

                entity.Property(e => e.MinGrade)
                    .HasColumnName("min_grade")
                    .HasComment("最低几年级能选");
            });

            modelBuilder.Entity<Notice>(entity =>
            {
                entity.ToTable("notice");

                entity.HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.NoticeId).HasColumnName("notice_id");

                entity.Property(e => e.AdminId).HasColumnName("admin_id");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("content")
                    .HasComment("内容");

                entity.Property(e => e.Time)
                    .HasColumnType("datetime")
                    .HasColumnName("time");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("title");
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("schedule");

                entity.HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");

                entity.Property(e => e.BeginWeek)
                    .HasColumnName("begin_week")
                    .HasComment("开始周");

                entity.Property(e => e.Campus)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("campus")
                    .HasComment("哪个校区");

                entity.Property(e => e.CanRetake)
                    .HasColumnName("can_retake")
                    .HasComment("学生是否能重修选择该课，0为否，1为是");

                entity.Property(e => e.CurrentNum)
                    .HasColumnName("current_num")
                    .HasComment("当前人数");

                entity.Property(e => e.EndWeek).HasColumnName("end_week");

                entity.Property(e => e.Half)
                    .HasColumnName("half")
                    .HasComment("上下半年,上为0,下为1");

                entity.Property(e => e.IsOver)
                    .HasColumnName("is_over")
                    .HasComment("是否结束");

                entity.Property(e => e.LessonId).HasColumnName("lesson_id");

                entity.Property(e => e.MaxNum)
                    .HasColumnName("max_num")
                    .HasComment("人数上限");

                entity.Property(e => e.Note)
                    .HasMaxLength(45)
                    .HasColumnName("note")
                    .HasComment("备注");

                entity.Property(e => e.Place)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("place")
                    .HasComment("上课地点");

                entity.Property(e => e.TeacherId).HasColumnName("teacher_id");

                entity.Property(e => e.TeachingMaterial)
                    .HasMaxLength(45)
                    .HasColumnName("teaching_material")
                    .HasComment("教材");

                entity.Property(e => e.Year)
                    .HasColumnName("year")
                    .HasComment("年份");
            });

            modelBuilder.Entity<ScheduleTime>(entity =>
            {
                entity.HasKey(e => new { e.ScheduleId, e.BeginSection, e.DayWeek })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

                entity.ToTable("schedule_time");

                entity.HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ScheduleId)
                    .HasColumnName("schedule_id")
                    .HasComment("排课的上课时间信息");

                entity.Property(e => e.BeginSection)
                    .HasColumnName("begin_section")
                    .HasComment("第几节课开始");

                entity.Property(e => e.DayWeek)
                    .HasColumnName("day_week")
                    .HasComment("星期几");

                entity.Property(e => e.EndSection)
                    .HasColumnName("end_section")
                    .HasComment("到第几节课为止(包括在其中)");

                entity.Property(e => e.SingleOrDouble)
                    .HasColumnName("Single_OR_Double")
                    .HasComment("单双周，1为单，2为双，3为全");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("student");

                entity.HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.StudentId)
                    .ValueGeneratedNever()
                    .HasColumnName("student_id");

                entity.Property(e => e.Department)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("department");

                entity.Property(e => e.Identity)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("identity")
                    .HasComment("本科生研究生还是博士生");

                entity.Property(e => e.InYear)
                    .HasColumnName("in_year")
                    .HasComment("哪一级(如2018级)，可能是留级的");

                entity.Property(e => e.IsGraduate)
                    .HasColumnName("is_graduate")
                    .HasComment("是否毕业了,0为否，1为是");

                entity.Property(e => e.OriginInyear)
                    .HasColumnName("origin_inyear")
                    .HasComment("最初的年级(即刚入学时候的年级)");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("password");

                entity.Property(e => e.Salt)
                    .HasMaxLength(45)
                    .HasColumnName("salt")
                    .HasComment("MD5码的盐");

                entity.Property(e => e.StudentName)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("student_name");
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.ToTable("teacher");

                entity.HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TeacherId)
                    .ValueGeneratedNever()
                    .HasColumnName("teacher_id");

                entity.Property(e => e.Department)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("department");

                entity.Property(e => e.IsQuit)
                    .HasColumnName("is_quit")
                    .HasComment("是否离职，0为否，1为是");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("password");

                entity.Property(e => e.Salt)
                    .HasMaxLength(45)
                    .HasColumnName("salt")
                    .HasComment("MD5码的盐");

                entity.Property(e => e.TeacherName)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("teacher_name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
