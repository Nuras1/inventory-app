using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using project_itransition.Data;
using project_itransition.Models.Entities;
using project_itransition.Services;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var google = builder.Configuration.GetSection("Authentication:Google");
var github = builder.Configuration.GetSection("Authentication:GitHub");
builder.Services.AddAuthentication().
    AddGoogle(options =>
    {
        options.ClientId = google["ClientId"]!;
        options.ClientSecret = google["ClientSecret"]!;
        options.CallbackPath = "/signin-google";
    })
.AddGitHub(options =>
{
    options.ClientId =
        github["ClientId"]!;

    options.ClientSecret =
        github["ClientSecret"]!;

    options.Scope.Add("user:email");
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

await SeedService.SeedDatabase(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseForwardedHeaders();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.Use(async (context, next) =>
{
    string cookie = string.Empty;
    if (context.Request.Cookies.TryGetValue("Language", out cookie))
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cookie);
        System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cookie);
    }
    else
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");
        System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
    }
    await next.Invoke();
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Inventory}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
