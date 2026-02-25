using InventoryCRM.Components;
using InventoryCRM.Components.Login;
using InventoryCRM.Data;
using InventoryCRM.Services;
using InventoryCRM.Services.UnitServices;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<TodoService>();
builder.Services.AddScoped<UnitService>();
builder.Services.AddScoped<DepositService>();
builder.Services.AddScoped<WorkerService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<OrderService>();


builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

// This is the ONE main registration you need. 
// Note: We use ApplicationUser here to match your Account components.
builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();


//builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, SmtpEmailSender>();
//builder.Services.AddTransient<IEmailSender<ApplicationUser>, SmtpEmailSender>();


builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("Advanced", policy =>
            policy.RequireRole("Admin", "Manager"));
    });
builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("All", policy =>
            policy.RequireRole("Admin", "Manager", "Deposit", "User"));
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanMoveUnits", policy =>
        policy.RequireRole("Admin", "Manager", "Deposit"));
});


var app = builder.Build();

// Seed Roles and Admin User
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    
    string[] roleNames = { "Admin", "Manager", "Deposit", "User" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Seed Admin User
    string adminEmail = "admin@example.com";
    string adminPassword = "Admin@123456";
    
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var newAdminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        
        var result = await userManager.CreateAsync(newAdminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdminUser, "Admin");
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}


app.MapStaticAssets();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapIdentityApi<ApplicationUser>();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapPost("Account/Logout", async (
    ClaimsPrincipal user,
    SignInManager<ApplicationUser> signInManager,
    [FromForm] string returnUrl) =>
{
    await signInManager.SignOutAsync();
    return TypedResults.LocalRedirect($"~/{returnUrl}");
});

app.Run();
