using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Softmax.XMessager.Data;
using Softmax.XMessager.Models;
using Softmax.XMessager.Services;
using AutoMapper;
using Microsoft.Extensions.FileProviders;
using Softmax.XMessager.Data.Entities;
using Softmax.XMessager.Data.Contracts;
using Softmax.XMessager.Data.Contracts.Services;
using Softmax.XMessager.Data.Contracts.Validations;
using Softmax.XMessager.Data.Repositories;
using Softmax.XMessager.Services.Validations;
using Softmax.XMessager.Utitities;


namespace Softmax.XMessager
{
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

           

            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            //services.AddSingleton<ILogger>();

            // Add application services.
            services.AddTransient<DbContext, ApplicationDbContext>();
            services.AddTransient<IEmailSender, EmailSender>();
            //services.AddTransient<ILogger, Logger>();
            services.AddTransient<IGenerator, Generator>();
            services.AddTransient<IMapper, Mapper>();

            services.AddTransient<IRepository<Client>, Repository<Client>>();
            services.AddTransient<IRepository<Gateway>, Repository<Gateway>>();
            services.AddTransient<IRepository<Application>, Repository<Application>>();
            services.AddTransient<IRepository<Request>, Repository<Request>>();

            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<IGatewayService, GatewayService>();
            services.AddTransient<IApplicationService, ApplicationService>();
            services.AddTransient<IRequestService, RequestService>();


            services.AddTransient<IClientValidation, ClientValidation>();
            services.AddTransient<IGatewayValidation, GatewayValidation>();
            services.AddTransient<IApplicationValidation, ApplicationValidation>();
            services.AddTransient<IRequestValidation, RequestValidation>();


            services.AddMvc();
            services.AddAutoMapper();

            services.AddOptions();
            services.Configure<PriceSettings>(Configuration.GetSection("PriceSettings"));


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

          CreateRoles(serviceProvider).Wait();
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            //adding custom roles
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string[] roleNames = { "Admin", "Manager", "Accountant", "Supervisor", "Officer" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                //creating the roles and seeding them to the database
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            //creating a super user who could maintain the web app
            var poweruser = new ApplicationUser
            {
                UserName = Configuration.GetSection("UserSettings")["UserEmail"],
                Email = Configuration.GetSection("UserSettings")["UserEmail"]
            };

            string UserPassword = Configuration.GetSection("UserSettings")["UserPassword"];
            var _user = await UserManager.FindByEmailAsync(Configuration.GetSection("UserSettings")["UserEmail"]);

            if (_user == null)
            {
                var createPowerUser = await UserManager.CreateAsync(poweruser, UserPassword);
                if (createPowerUser.Succeeded)
                {
                    //here we tie the new user to the "Admin" role 
                    await UserManager.AddToRoleAsync(poweruser, "Admin");

                }

                
            }
        }
    }
}
