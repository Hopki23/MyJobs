using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using MyJobs.Infrastructure.Data;
using MyJobs.Infrastructure.Data.Models.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
IServiceCollection serviceCollection = builder.Services.AddDbContext<MyJobsDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>
    (options =>
    {
        options.SignIn.RequireConfirmedEmail = false;
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 4;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<MyJobsDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
