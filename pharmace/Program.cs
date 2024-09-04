using HUc.Helper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using pharmace;
using pharmace.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<PharmacyContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var cookiePolicyOptions = new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.None,
};

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
if (!app.Environment.IsDevelopment())
{
    // Photos
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(@"D:\MEDIA\StudentPhoto"),
        RequestPath = "/StudentPhoto"
    });
}
    app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapDefaultControllerRoute();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


using (var scope = app.Services.CreateScope())
{
    var _context = scope.ServiceProvider.GetRequiredService<PharmacyContext>();

    var adminuser = new userpermations
    {
        Email = "admin@gmail.com",
        Password = CommonMethods.ConvertToEncrypt("Admin1234@"),
        Username = "MainAdmin5",
        fname = "Admin",
        lname = "Admin",
        Address = "Al-Mansoura",
        PhoneNumber = "01060062211",
        Role = "admin",
    };


    var user = new userpermations
    {
        Email = "user@gmail.com",
        Password = CommonMethods.ConvertToEncrypt("User1234@"),
        Username = "User",
        Role = "user",
    };

   

    var MainAdminExist = _context.userpermations.Any(u => u.Username == adminuser.Username);
    var UserExist = _context.userpermations.Any(u => u.Username == user.Username);


    if (!MainAdminExist)
        await _context.userpermations.AddAsync(adminuser);
    if (!UserExist)
        await _context.userpermations.AddAsync(user);


    if (!UserExist || !MainAdminExist )
        await _context.SaveChangesAsync();
}

app.Run();
