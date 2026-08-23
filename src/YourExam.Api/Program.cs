using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using YourExam.Api.Services;
using YourExam.Application.Interfaces;
using YourExam.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Supabase JWT Authentication
var supabaseJwtSecret = builder.Configuration["Supabase:JwtSecret"]
    ?? throw new InvalidOperationException("Supabase:JwtSecret is not configured.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(supabaseJwtSecret)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

// Current User
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Configure MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(YourExam.Application.DTOs.QuestionTemplates.QuestionTemplateDto).Assembly));

// Register Services
builder.Services.AddScoped<YourExam.Application.Common.Rules.IFallbackRuleProvider, YourExam.Application.Common.Rules.FallbackRuleProvider>();
builder.Services.AddScoped<YourExam.Application.Interfaces.IMathEvaluatorService, YourExam.Application.Services.MathEvaluatorService>();
builder.Services.AddScoped<YourExam.Application.Interfaces.IVariableGeneratorService, YourExam.Application.Services.VariableGeneratorService>();
builder.Services.AddSingleton<YourExam.Application.Interfaces.ITextVariableGeneratorService, YourExam.Infrastructure.Services.TextVariableGeneratorService>();
builder.Services.AddScoped<YourExam.Application.Interfaces.IDocumentGeneratorService, YourExam.Infrastructure.Services.WordDocumentGeneratorService>();

// Register Question Generation Strategy
builder.Services.AddScoped<YourExam.Application.Services.QuestionGeneration.IQuestionGeneratorStrategy, YourExam.Application.Services.QuestionGeneration.Strategies.Grade1.MathGrade1CalculationStrategy>();
builder.Services.AddScoped<YourExam.Application.Services.QuestionGeneration.IQuestionGeneratorStrategy, YourExam.Application.Services.QuestionGeneration.Strategies.Grade1.MathGrade1ComparisonStrategy>();
builder.Services.AddScoped<YourExam.Application.Services.QuestionGeneration.IQuestionGeneratorStrategy, YourExam.Application.Services.QuestionGeneration.Strategies.Grade1.MathGrade1FillInTheBlankStrategy>();
builder.Services.AddScoped<YourExam.Application.Services.QuestionGeneration.IQuestionGeneratorStrategy, YourExam.Application.Services.QuestionGeneration.Strategies.Grade1.MathGrade1WordProblemStrategy>();
builder.Services.AddScoped<YourExam.Application.Services.QuestionGeneration.IQuestionGeneratorFactory, YourExam.Application.Services.QuestionGeneration.QuestionGeneratorFactory>();

// Register Literature (Tiếng Việt) — Singleton vì dữ liệu tĩnh, load 1 lần lúc startup
builder.Services.AddSingleton<YourExam.Application.Interfaces.ILiteratureDictionaryService, YourExam.Infrastructure.Services.LiteratureDictionaryService>();
builder.Services.AddScoped<YourExam.Application.Services.QuestionGeneration.IQuestionGeneratorStrategy, YourExam.Application.Services.QuestionGeneration.Strategies.Grade1.LiteratureGrade1Strategy>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure Entity Framework Core with Postgres
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<YourExam.Domain.Interfaces.IUnitOfWork, YourExam.Infrastructure.Data.UnitOfWork>();
builder.Services.AddScoped<YourExam.Domain.Interfaces.IQuestionTemplateRepository, YourExam.Infrastructure.Repositories.QuestionTemplateRepository>();

var app = builder.Build();

// Run Migration (Update DB) when Server startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else 
{
    // Trên Production (nếu server tự quản lý SSL), bạn có thể bật lại HTTPS Redirection
    // Tuy nhiên nếu dùng Nginx/Cloudflare/Railway thì họ tự cấu hình SSL rồi, nên đoạn này cũng thường không cần.
    app.UseHttpsRedirection(); 
}

app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();


