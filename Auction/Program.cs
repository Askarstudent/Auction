using User.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration.GetConnectionString("DemoSeriLogDB");


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddLogging(); // ��������� ������ �����������


builder.Services.AddMvc().AddMvcLocalization(LanguageViewLocationExpanderFormat.Suffix);

// ������������ �����������
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var cultures = new[]
    {
        new CultureInfo("ru-Ru"),
        new CultureInfo("en-US"),
        new CultureInfo("kk-Kz")
    };
    options.DefaultRequestCulture = new RequestCulture(culture: "kk-Kz", uiCulture: "kk-Kz");
    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;
});
builder.Services.AddControllersWithViews();
builder.Services.AddLocalization(option =>
    option.ResourcesPath = "Resources");


builder.Services.AddDistributedMemoryCache();
builder.Services.Configure<CookieTempDataProviderOptions>(options =>
{
    options.Cookie.IsEssential = true;
    options.Cookie.Domain = "localhost:62882";
    options.Cookie.Expiration = TimeSpan.FromSeconds(160);
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.Name = ".My.Session";
});
// ��������� �������������� ����� Cookie
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Home/Login"; // ��������� ���� � ������ ��� ������
        options.LogoutPath = "/Home/Index";
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();
var localOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(localOptions.Value);

app.UseSession();
// ����������� ������������� ��������������
app.UseAuthentication(); // ��� ������ ���� ����� UseAuthorization
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
