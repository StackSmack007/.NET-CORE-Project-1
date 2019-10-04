namespace Junjuria.App
{
    using AutoMapper;
    using Junjuria.AutomapperConfig.AutoMapperConfiguration;
    using Junjuria.Infrastructure.Data;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Services.InitialSeed;
    using Junjuria.Services.Services;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
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

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<AppUser, IdentityRole>(opt =>
             {
                 opt.Password.RequireDigit = false;
                 opt.Password.RequireNonAlphanumeric = false;
                 opt.Password.RequireLowercase = false;
                 opt.Password.RequireUppercase = false;
                 opt.Password.RequiredLength = 4;
                 opt.Password.RequiredUniqueChars = 2;
             })
                .AddRoles<IdentityRole>()
                //.AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                /// mc.AddMaps(Assembly.GetAssembly(typeof(Product)).FullName,Assembly.GetAssembly(typeof(PurchaseItemDto)).FullName);
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

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
                //opt.CallbackPath = "/localhost:5001/signin-facebook";
            });


            services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, UserClaimsPrincipalFactory<AppUser, IdentityRole>>();

            services.AddMvc(
                opt => { opt.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
                    opt.EnableEndpointRouting = false; })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSingleton<Random>();
            services.AddScoped(typeof(IRepository<>), typeof(DbRepository<>));
            services.AddScoped<DataBaseSeeder>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IManufacturerService, ManufacturerService>();
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
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseAuthentication();
            //  app.UseMiddleware<SeederMiddleware>();
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