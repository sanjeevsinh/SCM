﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SCM.Data;
using SCM.Models;
using SCM.Models.ViewModels;
using SCM.Models.ServiceModels;
using SCM.Models.NetModels.AttachmentNetModels;
using SCM.Models.NetModels.IpVpnNetModels;
using SCM.Services;
using SCM.Services.SCMServices;
using AutoMapper;
using Microsoft.AspNetCore.Rewrite;

namespace SCM
{
    public class Startup
    {

        private MapperConfiguration MapperConfiguration { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            MapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperServiceModelProfileConfiguration());
                cfg.AddProfile(new AutoMapperViewModelProfileConfiguration());;
                cfg.AddProfile(new AutoMapperAttachmentServiceProfileConfiguration());
                cfg.AddProfile(new AutoMapperIpVpnServiceProfileConfiguration());
            });
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // DB context for our Sigma repository
            services.AddDbContext<SigmaContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddSignalR(options =>
            {
                options.Hubs.EnableDetailedErrors = true;
            });

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPortService, PortService>();
            services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<IDeviceService, DeviceService>();
            services.AddScoped<IVrfService, VrfService>();
            services.AddScoped<IContractBandwidthPoolService, ContractBandwidthPoolService>();
            services.AddScoped<IVpnService, VpnService>();
            services.AddScoped<IRouteTargetService, RouteTargetService>();
            services.AddScoped<IAttachmentSetService, AttachmentSetService>();
            services.AddScoped<IAttachmentSetVrfService, AttachmentSetVrfService>();
            services.AddScoped<IVpnAttachmentSetService, VpnAttachmentSetService>();
            services.AddScoped<IBgpPeerService, BgpPeerService>();
            services.AddScoped<ITenantNetworkService, TenantNetworkService>();
            services.AddScoped<ITenantCommunityService, TenantCommunityService>();
            services.AddScoped<IVpnTenantNetworkService, VpnTenantNetworkService>();
            services.AddScoped<IVpnTenantCommunityService, VpnTenantCommunityService>();
            services.AddScoped<IAttachmentService, AttachmentService>();
            services.AddScoped<IVifService, VifService>();
            services.AddScoped<INetworkSyncService, NetworkSyncService>();

            services.AddSingleton<IMapper>(sp => MapperConfiguration.CreateMapper());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, SigmaContext context)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseWebSockets();
            app.UseSignalR();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Initialise the DB

            DbInitializer.Initialize(context);
        }
    }
}
