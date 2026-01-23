using InventoryCRM.Components;
using InventoryCRM.Components.Login;
using InventoryCRM.Data;
using InventoryCRM.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
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
builder.Services.AddScoped<UserService>();

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
builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();


var app = builder.Build();

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
