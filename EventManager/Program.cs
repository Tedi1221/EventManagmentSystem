using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EventManagementSystem.Data;
using EventManagementSystem.Models;
using EventManagementSystem.Services;
using EventManagementSystem.Services.Contracts;

var builder = WebApplication.CreateBuilder(args);

// 1. Конфигурация на базата данни
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 2. Конфигурация на Identity системата
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 3. Добавяне на услуги (Dependency Injection)
builder.Services.AddScoped<IEventService, EventService>();

// 4. Добавяне на MVC и Razor Pages
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 5. Конфигуриране на HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 6. Извикване на Seeder-а за данни
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbSeeder.SeedRolesAndAdminAsync(services);
    await DbSeeder.SeedCategoriesAsync(services);
}

// 7. Конфигуриране на маршрути (Routes)
app.MapControllerRoute(
    name: "Admin",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
