using Abp.Zero.EntityFrameworkCore;
using KTGKMD.Authorization.Roles;
using KTGKMD.Authorization.Users;
using KTGKMD.Entities;
using KTGKMD.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace KTGKMD.EntityFrameworkCore;

public class KTGKMDDbContext : AbpZeroDbContext<Tenant, Role, User, KTGKMDDbContext>
{
    /* Define a DbSet for each entity of the application */
    public virtual DbSet<Exam> Exams { get; set; }
    public virtual DbSet<Passage> Passages { get; set; }
    public virtual DbSet<Question> Questions { get; set; }
    public virtual DbSet<QuestionOption> QuestionOptions { get; set; }
    public virtual DbSet<ExamAttempt> ExamAttempts { get; set; }
    public virtual DbSet<AttemptAnswer> AttemptAnswers { get; set; }

    public KTGKMDDbContext(DbContextOptions<KTGKMDDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Question>()
            .HasOne(q => q.Passage)
            .WithMany(p => p.Questions)
            .HasForeignKey(q => q.PassageId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AttemptAnswer>()
            .HasOne(a => a.Question)
            .WithMany()
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AttemptAnswer>()
            .HasOne(a => a.SelectedOption)
            .WithMany()
            .HasForeignKey(a => a.SelectedOptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
