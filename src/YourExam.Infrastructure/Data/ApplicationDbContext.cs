using Microsoft.EntityFrameworkCore;
using YourExam.Domain.Entities;

namespace YourExam.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Profile> Profiles { get; set; } = null!;
    public DbSet<QuestionTemplate> QuestionTemplates { get; set; } = null!;
    public DbSet<ExamBlueprint> ExamBlueprints { get; set; } = null!;
    public DbSet<BlueprintRule> BlueprintRules { get; set; } = null!;
    public DbSet<GeneratedExam> GeneratedExams { get; set; } = null!;
    public DbSet<GeneratedExamQuestion> GeneratedExamQuestions { get; set; } = null!;
    public DbSet<ExamVote> ExamVotes { get; set; } = null!;
    public DbSet<ExamDownload> ExamDownloads { get; set; } = null!;
    public DbSet<Topic> Topics { get; set; } = null!;
    public DbSet<TopicComment> TopicComments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Cấu hình bảng Profile
        modelBuilder.Entity<Profile>(entity =>
        {
            entity.ToTable("Profiles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
        });

        // 2. Cấu hình bảng QuestionTemplate
        modelBuilder.Entity<QuestionTemplate>(entity =>
        {
            entity.ToTable("QuestionTemplates");
            entity.HasKey(e => e.Id);
            // Ép kiểu VariablesConfig và DistractorLogic thành jsonb trong Postgres
            entity.Property(e => e.VariablesConfig).HasColumnType("jsonb");
            entity.Property(e => e.DistractorLogic).HasColumnType("jsonb");
        });

        // 3. Cấu hình bảng ExamBlueprint
        modelBuilder.Entity<ExamBlueprint>(entity =>
        {
            entity.ToTable("ExamBlueprints");
            entity.HasKey(e => e.Id);
            
            // Khai báo Khóa Ngoại trỏ về Profile (Tác giả)
            entity.HasOne(e => e.Author)
                  .WithMany()
                  .HasForeignKey(e => e.AuthorId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // 4. Cấu hình bảng BlueprintRule
        modelBuilder.Entity<BlueprintRule>(entity =>
        {
            entity.ToTable("BlueprintRules");
            entity.HasKey(e => e.Id);
            
            // Khai báo Quan hệ 1-Nhiều rõ ràng với ExamBlueprint
            entity.HasOne(e => e.Blueprint)
                  .WithMany(b => b.Rules)
                  .HasForeignKey(e => e.BlueprintId)
                  .OnDelete(DeleteBehavior.Cascade); // Nếu xóa Blueprint, xóa luôn các Rule bên trong
        });

        // 5. Cấu hình bảng GeneratedExam
        modelBuilder.Entity<GeneratedExam>(entity =>
        {
            entity.ToTable("GeneratedExams");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Subject).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TotalScore).HasPrecision(5, 2);

            // Khai báo Khóa Ngoại trỏ về Tác giả (Profile)
            entity.HasOne(e => e.Author)
                  .WithMany(u => u.GeneratedExams)
                  .HasForeignKey(e => e.AuthorId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Khai báo Khóa Ngoại trỏ về Khung đề (ExamBlueprint)
            entity.HasOne(e => e.Blueprint)
                  .WithMany(b => b.GeneratedExams)
                  .HasForeignKey(e => e.BlueprintId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Navigation property to Questions
            entity.HasMany(e => e.Questions)
                  .WithOne(q => q.GeneratedExam)
                  .HasForeignKey(q => q.GeneratedExamId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 6. Cấu hình bảng GeneratedExamQuestion
        modelBuilder.Entity<GeneratedExamQuestion>(entity =>
        {
            entity.ToTable("GeneratedExamQuestions");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.QuestionContent).IsRequired();
            entity.Property(e => e.MultipleChoiceOptions).HasColumnType("jsonb");
            entity.Property(e => e.CorrectAnswer).HasMaxLength(10);
            entity.Property(e => e.Score).HasPrecision(5, 2);

            // Foreign Key to QuestionTemplate (nullable - câu hỏi có thể gen từ template hoặc tạo thủ công)
            entity.HasOne(e => e.QuestionTemplate)
                  .WithMany()
                  .HasForeignKey(e => e.QuestionTemplateId)
                  .OnDelete(DeleteBehavior.SetNull);

            // Index for ordering questions
            entity.HasIndex(e => new { e.GeneratedExamId, e.OrderIndex });
        });

        // 7. Cấu hình bảng ExamVote (Khóa chính kép - Composite Key)
        modelBuilder.Entity<ExamVote>(entity =>
        {
            entity.ToTable("ExamVotes");

            // Khai báo KHÓA CHÍNH KÉP (Gồm cả ExamId và UserId)
            entity.HasKey(e => new { e.ExamId, e.UserId });

            // Khai báo Khóa Ngoại trỏ về Exam
            entity.HasOne(e => e.Exam)
                  .WithMany(ex => ex.ExamVotes)
                  .HasForeignKey(e => e.ExamId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Khai báo Khóa Ngoại trỏ về Profile
            entity.HasOne(e => e.User)
                  .WithMany(u => u.ExamVotes)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 8. Cấu hình bảng ExamDownload (Khóa chính kép - Composite Key)
        modelBuilder.Entity<ExamDownload>(entity =>
        {
            entity.ToTable("ExamDownloads");

            // Khai báo KHÓA CHÍNH KÉP (Gồm cả ExamId và UserId)
            entity.HasKey(e => new { e.ExamId, e.UserId });

            // Khai báo Khóa Ngoại trỏ về Exam
            entity.HasOne(e => e.Exam)
                  .WithMany(ex => ex.Downloads)
                  .HasForeignKey(e => e.ExamId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Khai báo Khóa Ngoại trỏ về Profile
            entity.HasOne(e => e.User)
                  .WithMany(u => u.ExamDownloads)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 9. Cấu hình bảng Topic
        modelBuilder.Entity<Topic>(entity =>
        {
            entity.ToTable("Topics");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Content).IsRequired();

            entity.HasOne(e => e.Author)
                  .WithMany(u => u.Topics)
                  .HasForeignKey(e => e.AuthorId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 10. Cấu hình bảng TopicComment
        modelBuilder.Entity<TopicComment>(entity =>
        {
            entity.ToTable("TopicComments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired();

            entity.HasOne(e => e.Topic)
                  .WithMany(t => t.Comments)
                  .HasForeignKey(e => e.TopicId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Author)
                  .WithMany(u => u.TopicComments)
                  .HasForeignKey(e => e.AuthorId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
