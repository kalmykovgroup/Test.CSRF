
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Test.CSRF
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;

                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; 
            });


            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
             .AddCookie(options =>
             {
                 options.Cookie.Name = "auth";
                 options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // access duration for particular cookie
                 options.LoginPath = "/api/login"; // Specify your login path
                 options.AccessDeniedPath = "/api/access-denied"; // Specify your access denied path
             });




            var MyPolicyCorce = "MyPolicyCorce";



            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyPolicyCorce, policy =>
                {
                    policy
                    .WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();  // Enable authentication
            app.UseAuthorization();


            app.UseSession();
            app.UseCors(MyPolicyCorce);
            //  app.UseCors(MyAllowSpecificOrigins);

            app.MapControllers();

            app.Run();
        }
    }
}
