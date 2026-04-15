using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Schedule_Repository.Models;

public partial class ScheduleForTeacherContext : DbContext
{
    public ScheduleForTeacherContext()
    {
    }

    public ScheduleForTeacherContext(DbContextOptions<ScheduleForTeacherContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ProjectCourse> ProjectCourses { get; set; }

    public virtual DbSet<ProjectGroup> ProjectGroups { get; set; }

    public virtual DbSet<ProjectSupervisor> ProjectSupervisors { get; set; }

    public virtual DbSet<ReviewAssignment> ReviewAssignments { get; set; }

    public virtual DbSet<ReviewAssignmentTeacher> ReviewAssignmentTeachers { get; set; }

    public virtual DbSet<ReviewRound> ReviewRounds { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Semester> Semesters { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<TeacherAvailability> TeacherAvailabilities { get; set; }

    public virtual DbSet<TeacherCompatibility> TeacherCompatibilities { get; set; }

    public virtual DbSet<TimeSlot> TimeSlots { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

     /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
 #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
         => optionsBuilder.UseSqlServer("Server=Tuandeptrai;Database=ScheduleForTeacher;Uid=sa;Pwd=12345;TrustServerCertificate=True");*/

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProjectCourse>(entity =>
        {
            entity.HasKey(e => e.ProjectCourseId).HasName("PK__ProjectC__E26F9A2E84D8E39F");

            entity.HasIndex(e => new { e.SemesterId, e.CourseCode }, "UQ_ProjectCourses_SemesterId_CourseCode").IsUnique();

            entity.Property(e => e.CourseCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CourseName).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasOne(d => d.Semester).WithMany(p => p.ProjectCourses)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectCourses_Semesters");
        });

        modelBuilder.Entity<ProjectGroup>(entity =>
        {
            entity.HasKey(e => e.ProjectGroupId).HasName("PK__ProjectG__17125D9E20CF4E3C");

            entity.HasIndex(e => e.ProjectCourseId, "IX_ProjectGroups_ProjectCourseId");

            entity.HasIndex(e => new { e.ProjectCourseId, e.GroupCode }, "UQ_ProjectGroups_ProjectCourseId_GroupCode").IsUnique();

            entity.Property(e => e.GroupCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.GroupName).HasMaxLength(200);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("ACTIVE");

            entity.HasOne(d => d.ProjectCourse).WithMany(p => p.ProjectGroups)
                .HasForeignKey(d => d.ProjectCourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectGroups_ProjectCourses");
        });

        modelBuilder.Entity<ProjectSupervisor>(entity =>
        {
            entity.HasKey(e => e.ProjectSupervisorId).HasName("PK__ProjectS__4986CDDDFA7D70CE");

            entity.HasIndex(e => e.ProjectGroupId, "IX_ProjectSupervisors_ProjectGroupId");

            entity.HasIndex(e => e.TeacherId, "IX_ProjectSupervisors_TeacherId");

            entity.HasIndex(e => new { e.ProjectGroupId, e.SupervisorOrder }, "UQ_ProjectSupervisors_ProjectGroupId_SupervisorOrder").IsUnique();

            entity.HasIndex(e => new { e.ProjectGroupId, e.TeacherId }, "UQ_ProjectSupervisors_ProjectGroupId_TeacherId").IsUnique();

            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.ProjectGroup).WithMany(p => p.ProjectSupervisors)
                .HasForeignKey(d => d.ProjectGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectSupervisors_ProjectGroups");

            entity.HasOne(d => d.Teacher).WithMany(p => p.ProjectSupervisors)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectSupervisors_Teachers");
        });

        modelBuilder.Entity<ReviewAssignment>(entity =>
        {
            entity.HasKey(e => e.ReviewAssignmentId).HasName("PK__ReviewAs__5FDBB6CAF1E3CDF9");

            entity.HasIndex(e => new { e.AssignedDate, e.TimeSlotId }, "IX_ReviewAssignments_AssignedDate_TimeSlotId");

            entity.HasIndex(e => e.ReviewRoundId, "UQ_ReviewAssignments_ReviewRoundId").IsUnique();

            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("DRAFT");

            entity.HasOne(d => d.AssignedByUser).WithMany(p => p.ReviewAssignments)
                .HasForeignKey(d => d.AssignedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReviewAssignments_Users");

            entity.HasOne(d => d.ReviewRound).WithOne(p => p.ReviewAssignment)
                .HasForeignKey<ReviewAssignment>(d => d.ReviewRoundId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReviewAssignments_ReviewRounds");

            entity.HasOne(d => d.TimeSlot).WithMany(p => p.ReviewAssignments)
                .HasForeignKey(d => d.TimeSlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReviewAssignments_TimeSlots");
        });

        modelBuilder.Entity<ReviewAssignmentTeacher>(entity =>
        {
            entity.HasKey(e => e.ReviewAssignmentTeacherId).HasName("PK__ReviewAs__2E4545EFAE243FE1");

            entity.HasIndex(e => e.TeacherId, "IX_ReviewAssignmentTeachers_TeacherId");

            entity.HasIndex(e => new { e.ReviewAssignmentId, e.TeacherId }, "UQ_ReviewAssignmentTeachers_ReviewAssignmentId_TeacherId").IsUnique();

            entity.Property(e => e.RoleInPanel)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.ReviewAssignment).WithMany(p => p.ReviewAssignmentTeachers)
                .HasForeignKey(d => d.ReviewAssignmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReviewAssignmentTeachers_ReviewAssignments");

            entity.HasOne(d => d.Teacher).WithMany(p => p.ReviewAssignmentTeachers)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReviewAssignmentTeachers_Teachers");
        });

        modelBuilder.Entity<ReviewRound>(entity =>
        {
            entity.HasKey(e => e.ReviewRoundId).HasName("PK__ReviewRo__CD352976C364E68F");

            entity.HasIndex(e => e.ProjectGroupId, "IX_ReviewRounds_ProjectGroupId");

            entity.HasIndex(e => new { e.ProjectGroupId, e.RoundNumber }, "UQ_ReviewRounds_ProjectGroupId_RoundNumber").IsUnique();

            entity.Property(e => e.RoundName).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("PENDING");

            entity.HasOne(d => d.ProjectGroup).WithMany(p => p.ReviewRounds)
                .HasForeignKey(d => d.ProjectGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReviewRounds_ProjectGroups");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A3A4D9F76");

            entity.HasIndex(e => e.RoleCode, "UQ_Roles_RoleCode").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.RoleCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RoleName).HasMaxLength(100);
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(e => e.SemesterId).HasName("PK__Semester__043301DD88715831");

            entity.HasIndex(e => e.SemesterCode, "UQ_Semesters_SemesterCode").IsUnique();

            entity.Property(e => e.AcademicYear)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.SemesterCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SemesterName).HasMaxLength(200);
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.TeacherId).HasName("PK__Teachers__EDF25964E7DAC1CC");

            entity.HasIndex(e => e.TeacherCode, "UQ_Teachers_TeacherCode").IsUnique();

            entity.HasIndex(e => e.UserId, "UQ_Teachers_UserId").IsUnique();

            entity.Property(e => e.AcademicRank).HasMaxLength(100);
            entity.Property(e => e.Department).HasMaxLength(200);
            entity.Property(e => e.IsAvailableForProjectReview).HasDefaultValue(true);
            entity.Property(e => e.MaxAssignmentsPerDay).HasDefaultValue(4);
            entity.Property(e => e.Specialization).HasMaxLength(200);
            entity.Property(e => e.TeacherCode)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithOne(p => p.Teacher)
                .HasForeignKey<Teacher>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Teachers_Users");
        });

        modelBuilder.Entity<TeacherAvailability>(entity =>
        {
            entity.HasKey(e => e.TeacherAvailabilityId).HasName("PK__TeacherA__A845A5B858C16397");

            entity.HasIndex(e => new { e.TeacherId, e.AvailableDate }, "IX_TeacherAvailabilities_TeacherId_AvailableDate");

            entity.HasIndex(e => new { e.TeacherId, e.AvailableDate, e.TimeSlotId }, "UQ_TeacherAvailabilities_TeacherId_AvailableDate_TimeSlotId").IsUnique();

            entity.Property(e => e.AvailabilityStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("AVAILABLE");
            entity.Property(e => e.Note).HasMaxLength(255);

            entity.HasOne(d => d.Teacher).WithMany(p => p.TeacherAvailabilities)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TeacherAvailabilities_Teachers");

            entity.HasOne(d => d.TimeSlot).WithMany(p => p.TeacherAvailabilities)
                .HasForeignKey(d => d.TimeSlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TeacherAvailabilities_TimeSlots");
        });

        modelBuilder.Entity<TeacherCompatibility>(entity =>
        {
            entity.HasKey(e => e.TeacherCompatibilityId).HasName("PK__TeacherC__1AF11416AC866B79");

            entity.HasIndex(e => new { e.TeacherId1, e.TeacherId2 }, "UQ_TeacherCompatibilities_TeacherId1_TeacherId2").IsUnique();

            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.PreferenceType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("NEUTRAL");

            entity.HasOne(d => d.TeacherId1Navigation).WithMany(p => p.TeacherCompatibilityTeacherId1Navigations)
                .HasForeignKey(d => d.TeacherId1)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TeacherCompatibilities_Teacher1");

            entity.HasOne(d => d.TeacherId2Navigation).WithMany(p => p.TeacherCompatibilityTeacherId2Navigations)
                .HasForeignKey(d => d.TeacherId2)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TeacherCompatibilities_Teacher2");
        });

        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.HasKey(e => e.TimeSlotId).HasName("PK__TimeSlot__41CC1F325A3121DD");

            entity.HasIndex(e => e.SlotNumber, "UQ_TimeSlots_SlotNumber").IsUnique();

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.SlotName).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C9BC35529");

            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.HasIndex(e => e.Username, "UQ_Users_Username").IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("PK__UserRole__3D978A3594C1EE28");

            entity.HasIndex(e => new { e.UserId, e.RoleId }, "UQ_UserRoles_UserId_RoleId").IsUnique();

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Roles");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
