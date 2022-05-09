using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TruckRecoveryWebApplication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//����� ������ �� ����� �������
string connection = builder.Configuration.GetConnectionString("DefaultConnection");

//��� �������� �������� ������
builder.Services.AddDbContext<Context>(options => options.UseSqlServer(connection));

// �������������� � ������� ����
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options => {
        options.LoginPath = "/SystemUsers/Login";
        options.AccessDeniedPath = "/SystemUsers/Login";
        });


//��� ���� ������� ����� ������
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin", builder =>
    {
        builder.RequireClaim(ClaimTypes.Role, "admin");
    });
    options.AddPolicy("uchet", builder =>
    {
        builder.RequireClaim(ClaimTypes.Role, "uchet");
    });
    options.AddPolicy("user", builder =>
    {
        builder.RequireClaim(ClaimTypes.Role, "user");
    });
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);//����� ��� �������������.
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();   // ���������� middleware �������������� 
app.UseAuthorization();   // ���������� middleware ����������� 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Orders}/{action=Index}/{id?}");

app.Run();
