﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using System;
using System.Reflection;
using System.IO;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using TimeHelper.Data;
using Newtonsoft.Json;


namespace TimeHelper.Api
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
            // This is required to be instantiated before the OpenIdConnectOptions starts getting configured.
            // By default, the claims mapping will map claim names in the old format to accommodate older SAML applications.
            // 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role' instead of 'roles'
            // This flag ensures that the ClaimsIdentity claims collection will be built from the claims in the token
            // JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            // Adds Microsoft Identity platform (AAD v2.0) support to protect this Api
            services.AddMicrosoftIdentityWebApiAuthentication(Configuration)
            .EnableTokenAcquisitionToCallDownstreamApi()
                .AddInMemoryTokenCaches();
            services.AddControllers().AddNewtonsoftJson(
              options => {
                  options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
              }); ;
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Time Helper Api",
                    Description = "A simple Api for Time Management",
                    TermsOfService = new Uri(Configuration["TermsUri"]),
                    Contact = new OpenApiContact
                    {
                        Name = Configuration["ContactName"],
                        Email = Configuration["ContactEmail"],
                        Url = new Uri(Configuration["ContactUri"]),
                    },
                    License = new  OpenApiLicense
                    {
                        Name = Configuration["LicenseType"],
                        Url = new Uri(Configuration["LicenseUri"]),
                    }
                });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);       
            });

            services.AddDbContext<TimeHelperDataContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("TimeHelperDataContext")));

            services.AddApplicationInsightsTelemetry(Configuration);


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TimeHelperDataContext dataContext)
        {
            if (env.IsDevelopment())
            {
                // Since IdentityModel version 5.2.1 (or since Microsoft.AspNetCore.Authentication.JwtBearer version 2.2.0),
                // PII hiding in log files is enabled by default for GDPR concerns.
                // For debugging/development purposes, one can enable additional detail in exceptions by setting IdentityModelEventSource.ShowPII to true.
                Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            dataContext.Database.Migrate();
            app.UseHttpsRedirection();
            
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "GKS Time Assistant API V1.0");
                c.RoutePrefix = string.Empty;
            });
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }
    }
}