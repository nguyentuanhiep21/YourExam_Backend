using Microsoft.EntityFrameworkCore;
using YourExam.Domain.Entities;

namespace YourExam.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<QuestionTemplate> QuestionTemplates { get; set; } = null!;
    public DbSet<ExamBlueprint> ExamBlueprints { get; set; } = null!;
    public DbSet<BlueprintRule> BlueprintRules { get; set; } = null!;
    public DbSet<GeneratedExam> GeneratedExams { get; set; } = null!;
    public DbSet<Vote> Votes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Cấu hình bảng User
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id); // Khai báo Khóa Chính rõ ràng
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(255);
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
            
            // Khai báo Khóa Ngoại trỏ về User (Tác giả)
            entity.HasOne<User>()
                  .WithMany()
                  .HasForeignKey(e => e.AuthorId)
                  .OnDelete(DeleteBehavior.SetNull); // Nếu xóa User, Blueprint vẫn giữ nhưng Author = null
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
            
            // Khai báo Khóa Ngoại trỏ về Tác giả (User)
            entity.HasOne(e => e.Author)
                  .WithMany(u => u.GeneratedExams)
                  .HasForeignKey(e => e.AuthorId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            // Khai báo Khóa Ngoại trỏ về Khung đề (ExamBlueprint)
            entity.HasOne(e => e.Blueprint)
                  .WithMany(b => b.GeneratedExams)
                  .HasForeignKey(e => e.BlueprintId)
                  .OnDelete(DeleteBehavior.Restrict); 
        });

        // 6. Cấu hình bảng Vote (Khóa chính kép - Composite Key)
        modelBuilder.Entity<Vote>(entity =>
        {
            entity.ToTable("Votes");
            
            // Khai báo KHÓA CHÍNH KÉP (Gồm cả ExamId và UserId)
            entity.HasKey(e => new { e.ExamId, e.UserId });
            
            // Khai báo Khóa Ngoại trỏ về Exam
            entity.HasOne(e => e.Exam)
                  .WithMany(ex => ex.Votes)
                  .HasForeignKey(e => e.ExamId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            // Khai báo Khóa Ngoại trỏ về User
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Votes)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
