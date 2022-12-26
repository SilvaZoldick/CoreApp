using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Data;
using Data.Models;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace AuthApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            DataContext.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DataConnection")));            

            services.AddControllersWithViews();

            var key = Encoding.ASCII.GetBytes(Settings.Secret);
            services.AddAuthentication(x =>
                {                    
                    x.DefaultScheme = "JWT_OR_COOKIE";
                    x.DefaultChallengeScheme = "JWT_OR_COOKIE";
                })
                .AddJwtBearer(x =>
                    {
                        x.RequireHttpsMetadata = false;
                        x.SaveToken = true;
                        x.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(key),                            
                            ValidateIssuer = true,
                            ValidIssuer = "https://localhost:5001",
                            ValidateAudience = true,
                            ValidAudience = "https://localhost:5001"

                        };
                    }).AddCookie(options =>
                        {
                            options.LoginPath = "/Account/Login";
                    }).AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
                    {
                        // runs on each request
                        options.ForwardDefaultSelector = context =>
                        {
                            // filter by auth type
                            string authorization = context.Request.Headers[HeaderNames.Authorization];
                            if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                                return JwtBearerDefaults.AuthenticationScheme;

                            // otherwise always check for cookie auth
                            return CookieAuthenticationDefaults.AuthenticationScheme;
                        };                        
                    });

            services.AddScoped<DataContext>();
            services.AddDefaultIdentity<User>().AddEntityFrameworkStores<DataContext>();
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");                
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}").RequireAuthorization();
            });
        }
    }
}
