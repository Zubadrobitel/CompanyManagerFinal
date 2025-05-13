using CompanyManagerFinal.Core.Interfaces;
using CompanyManagerFinal.Infastructure.Data;
using CompanyManagerFinal.Infastructure.Repositories;
using CompanyManagerFinal.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole().SetMinimumLevel(LogLevel.Debug);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<ICompanyService, CompanyService>();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.UseHttpsRedirection();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Companies}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<DBContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Произошла ошибка при миграции!");
    }
}

app.Run();