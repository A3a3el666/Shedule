using Microsoft.EntityFrameworkCore;
using Schedule.Data;
using Schedule.Models;
using Schedule.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = "Server=localhost;Port=5432;Database=schedule;User Id=postgres;Password=123;";

// Add services to the container.
builder.Services.AddDbContext<SchoolContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ClassRepository>();
builder.Services.AddScoped<SubjectRepository>();
builder.Services.AddScoped<TeacherRepository>();
builder.Services.AddScoped<TeacherSubjectRepository>();

builder.Services.AddScoped<ScheduleService>(); // Регистрация сервиса расписания

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
