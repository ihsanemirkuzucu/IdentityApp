using IdentityApp.Web.ClaimProviders;
using IdentityApp.Web.CustomExtensions;
using IdentityApp.Web.Models;
using IdentityApp.Web.OptionsModels;
using IdentityApp.Web.Permissions;
using IdentityApp.Web.Requirements;
using IdentityApp.Web.Seed_Datas;
using IdentityApp.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"));
});

builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IClaimsTransformation, UserClaimProvider>();
builder.Services.AddScoped<IAuthorizationHandler, ExchangeExpireRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ViolanceRequirementHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IstanbulPolicy", policy =>
    {
        policy.RequireClaim("city", "Ýstanbul");
    });

    options.AddPolicy("ExchangePolicy", policy =>
    {
        policy.AddRequirements(new ExchangeExpireRequirement());
    });
    options.AddPolicy("ViolancePolicy", policy =>
    {
        policy.AddRequirements(new ViolanceRequirement(){ThresholdAge = 18});
    });
    options.AddPolicy("OrderPermissionReadOrDelete", policy =>
    {
        //policy.RequireClaim("permission", Permission.Order.Read,Permission.Order.Delete, Permission.Stock.Delete);//Üçünden herhangi birinin olmasý yeterli

        #region Üç policy de olmak zorunda!!!
        policy.RequireClaim("permission", Permission.Stock.Delete);
        policy.RequireClaim("permission", Permission.Order.Read);
        policy.RequireClaim("permission", Permission.Order.Delete);
        #endregion

    });

    options.AddPolicy("OrderPermissionRead", policy =>
    {
        policy.RequireClaim("permission", Permission.Order.Read);
    });

    options.AddPolicy("OrderPermissionDelete", policy =>
    {
        policy.RequireClaim("permission", Permission.Order.Delete);
    });

    options.AddPolicy("StockPermissionDelete", policy =>
    {
        policy.RequireClaim("permission", Permission.Stock.Delete);
    });
});

builder.Services.AddIdentityWithExtension();
builder.Services.ConfigureApplicationCookie(options =>
{
    var cookieBuilder = new CookieBuilder();
    cookieBuilder.Name = "IdentityAppCookie";
    options.LoginPath = new PathString("/Home/SignIn");
    options.LogoutPath = new PathString("/Member/Logout");
    options.AccessDeniedPath = new PathString("/Member/AccessDenied");
    options.Cookie = cookieBuilder;
    options.ExpireTimeSpan = TimeSpan.FromDays(10);
    options.SlidingExpiration = true; //her giriþ yaptýðýnda cookkie ömrünü uzatýr.
});

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.FromMinutes(10); //securitystamp deðerini kontrol etme konfigürasyonu
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
    await PermissionSeedData.Seed(roleManager);
}

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

#region AreaAdmin
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
#endregion

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
