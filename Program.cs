using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Buoi2.Data;
using Products.Data;
using Buoi2.Models;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");

// Create roles and default admin account
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var roles = new[] { "Admin", "Client" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(role));
            logger.LogInformation($"Created role {role}: {result.Succeeded}");
        }
    }

    // Create default admin account
    string adminEmail = "Admin@gmail.com.vn";
    string adminPassword = "Admin123@";

    var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
    if (existingAdmin == null)
    {
        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "User",
            EmailConfirmed = true 
        };

        var result = await userManager.CreateAsync(admin, adminPassword);
        if (result.Succeeded)
        {
            logger.LogInformation("Admin account created successfully");
            await userManager.AddToRoleAsync(admin, "Admin");
            logger.LogInformation("Admin role assigned successfully");
        }
        else
        {
            logger.LogError($"Failed to create admin account: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
    else
    {
        logger.LogInformation("Admin account already exists");
    }
}

// cau hinh dinh tuyen
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "Default",
        pattern: "{controller=Product}/{action=Index}/{id?}"
    );
});

app.Run();
