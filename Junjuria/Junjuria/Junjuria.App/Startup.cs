﻿namespace Junjuria.App
{
    using Abp.Net.Mail;
    using AutoMapper;
    using Junjuria.App.Hubs;
    using Junjuria.AutomapperConfig.AutoMapperConfiguration;
    using Junjuria.Common;
    using Junjuria.Infrastructure.Data;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Services.InitialSeed;
    using Junjuria.Services.Services;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.IO;

    public class Startup
    {
        private readonly IWebHostEnvironment env;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            this.env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //services.AddDataProtection()
            //.SetApplicationName("junjuria-store")
            //.PersistKeysToFileSystem(new DirectoryInfo("/Keys"));


            if (env.EnvironmentName == "Development")
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseMySql(Configuration.GetConnectionString("DevelopmentMySql"))
                    );

                //services.AddDbContext<ApplicationDbContext>(options =>
                //options.UseSqlServer(Configuration.GetConnectionString("DevelopmentSql"))
                //);
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("ProductionMySql")));
            }

            services.AddIdentity<AppUser, IdentityRole>(opt =>
             {
                 opt.Password.RequireDigit = false;
                 opt.Password.RequireNonAlphanumeric = false;
                 opt.Password.RequireLowercase = false;
                 opt.Password.RequireUppercase = false;
                 opt.Password.RequiredLength = 4;
                 opt.Password.RequiredUniqueChars = 2;
                 opt.User.RequireUniqueEmail = true;
                 opt.Lockout.MaxFailedAccessAttempts = 10;
                 opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
             })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.ConfigureApplicationCookie(options => options.LoginPath = "/Account/LogIn");

            var profile = new MappingProfile();
            var mappingConfig = new MapperConfiguration(mc =>
            {
                /// mc.AddMaps(Assembly.GetAssembly(typeof(Product)).FullName,Assembly.GetAssembly(typeof(PurchaseItemDto)).FullName);
                mc.AddProfile(profile);
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            services.AddResponseCompression(opt => opt.EnableForHttps = true);

            services.AddDistributedMemoryCache();
            services.AddSession(opt =>
            {
                opt.Cookie.HttpOnly = true;
                opt.IdleTimeout = TimeSpan.FromMinutes(90);
            });

            //services.Configure<CookiePolicyOptions>(options =>{ options.CheckConsentNeeded = context => false; options.MinimumSameSitePolicy = SameSiteMode.None;});

            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = this.Configuration["FacebookAuthentication:AppId"];
                facebookOptions.AppSecret = this.Configuration["FacebookAuthentication:AppSecret"];
                //  if(env.EnvironmentName == "Production") facebookOptions.CallbackPath = "https://www.junjuria.nsh7.tk/signin-facebook";
            });


            services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, UserClaimsPrincipalFactory<AppUser, IdentityRole>>();
            services.AddMvc(
                opt =>
                {
                    opt.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
                    opt.EnableEndpointRouting = false;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddRazorRuntimeCompilation();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });


            services.AddSingleton<EmailSender>(new EmailSender(this.Configuration["SMTP:AppKey"]));
            
            services.AddSignalR();

            services.AddSingleton<Random>();
            services.AddScoped(typeof(IRepository<>), typeof(DbRepository<>));
            services.AddScoped<DataBaseSeeder>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IStatisticService, StatisticService>();
            services.AddScoped<IManufacturersService, ManufacturersService>();
            services.AddSingleton<ICloudineryService, CloudineryService>();
            services.AddScoped<IViewRenderService, ViewRenderService>();
            services.AddApplicationInsightsTelemetry();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName.StartsWith("Development"))
            {
                app.UseDeveloperExceptionPage();
                //  app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseResponseCompression();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseAuthentication();

            app.Use((context, next) =>
            {
                if (context.Request.Headers["x-forwarded-proto"] == "https")
                {
                    context.Request.Scheme = "https";
                }
                return next();
            });

            app.Use((context, next) =>
            {
                var path = context.Request.Path.ToString();
                if (path.StartsWith("/Account"))
                {
                    context.Request.Path = new PathString("/Identity" + path);
                }
                return next();
            });

            app.UseMiddleware<Middlewares.SeederMiddleware>();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapHub<ChatHub>("/chatHub");
            //});

            app.UseSignalR(opt => opt.MapHub<ChatHub>(GlobalConstants.ChatUrlHub));

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                               name: "areas",
                               template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}