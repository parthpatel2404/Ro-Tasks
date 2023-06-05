using CI_Platform.Controllers;
using CIPlatform.Entities.Data;
using CIPlatform.Repository.Interface;
using CIPlatform.Repository.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MySql.EntityFrameworkCore.Extensions;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthentication();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                var token = context.Request.Cookies["token"]?.ToString(); //Store in httponly cookie
                if (token == null)
                {
                    context.Response.Redirect("/User/Index"); // Replace "/Account/Login" with your actual login page URL
                    context.HandleResponse(); // Suppress the default challenge response
                }
                return Task.CompletedTask;
            }
        };
        //options.SecurityTokenValidators.Clear();
        //options.SecurityTokenValidators.Add(new CustomSecurityTokenValidator());

    });



// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<CIPlatformDbContext>();
//My Sql.................
builder.Services.AddDbContext<RoTaskDbContext>();
builder.Services.AddEntityFrameworkMySQL().AddDbContext<RoTaskDbContext>(options => {
    options.UseMySQL(builder.Configuration.GetConnectionString("MySqlConnection"));
});


//builder.Services.AddSingleton<ISecurityTokenValidator, CustomSecurityTokenValidator>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
builder.Services.AddScoped<IStoryRepository, StoryRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
//builder.Services.AddScoped<BaseController>((provider) =>
//{
//    // Get the JWT token from the user's session or local storage
//    var httpContextAccessor = provider.GetService<IHttpContextAccessor>();

//    // Get the JWT token from the user's session or local storage
//    string token = httpContextAccessor.HttpContext.Request.Headers["Authorization"];
//    return new BaseController(token);
//});
builder.Services.AddTransient<BaseController>();
builder.Services.AddHttpContextAccessor();

//builder.Services.AddDbContext<CIPlatformDbContext>(options => options.UseSqlServer(
//  builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSession();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.Use(async (context, next) =>
{
    var token = context.Request.Cookies["token"]?.ToString(); //Store in httponly cookie
    if (!string.IsNullOrWhiteSpace(token))
    {
        context.Request.Headers.Add("Authorization", "Bearer " + token);
    }
    await next();
});
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Index}/{id?}");

app.Run();
