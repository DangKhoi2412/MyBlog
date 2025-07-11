using ASP.NET_Razor_Final.models;
using ASP.NET_Razor_Final.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using MyApp.Security.Requirements;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
        builder.Services.AddRazorPages();
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("MyBlogContext"));
        });

        builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        //Cau hinh Identity
        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false; // Không bắt phải có số
            options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
            options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
            options.Password.RequireUppercase = false; // Không bắt buộc chữ in
            options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
            options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

            // Cấu hình Lockout - khóa user
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
            options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 5 lầ thì khóa
            options.Lockout.AllowedForNewUsers = true;

            // Cấu hình về User.
            options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;  // Email là duy nhất

            // Cấu hình đăng nhập.
            options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
            options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
            options.SignIn.RequireConfirmedAccount = true;
        });

        //Đăng kí dịch vụ EmailSender
        builder.Services.AddTransient<IEmailSender, EmailSender>();
        builder.Services.ConfigureApplicationCookie(option =>
        {
            option.LoginPath = "/login";
            option.LogoutPath = "/logout";
            option.AccessDeniedPath = "/access-denied.html";
        });
        builder.Services.AddAuthentication()
                        .AddGoogle(option =>
                        {
                            option.ClientId = builder.Configuration["Google:ClientId"];
                            option.ClientSecret = builder.Configuration["Google:ClientSecret"];
                            option.CallbackPath = "/dang-nhap-tu-google";
                        });
        //Cau hinh authorize
        builder.Services.AddAuthorization(option =>
        {
            option.AddPolicy("AllowEditRole", policyBuilder =>
            {
                //Dieu kien policy
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireClaim("can-edit", "user", "update");
            });
            option.AddPolicy("InGenZ", policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.Requirements.Add(new GenZRequirement());
            });
            option.AddPolicy("ShowAdminMenu", policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireRole("Admin", "Admin+Vip");
            });
            option.AddPolicy("CanUpdateArticle", policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.Requirements.Add(new ArticleUpdateRequirement());
            });
        });
        builder.Services.AddTransient<IAuthorizationHandler, AppAuthorizationHandle>();



        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapRazorPages()
           .WithStaticAssets();
        app.Run();
    }
}