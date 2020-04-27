using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjetoPraticoCodenation.ConfigStartup;
using ProjetoPraticoCodenation.Models;
using ProjetoPraticoCodenation.Services;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace ProjetoPraticoCodenation
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public StartupIdentityServer IdentitServerStartup { get; }
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;

            if (!environment.IsEnvironment("Testing"))
                IdentitServerStartup = new StartupIdentityServer(environment);
        }


        public void ConfigureServices(IServiceCollection services)
        {
            //add politica para user Ingrid
            services.AddMvcCore()
           .AddAuthorization(opt => {
               opt.AddPolicy("Admin", policy => policy.RequireClaim(ClaimTypes.Email, "agatha@gmail.com"));
           })
           .AddJsonFormatters()
           .AddApiExplorer()
           .AddVersionedApiExplorer(p =>
           {
               p.GroupNameFormat = "'v'VVV";
               p.SubstituteApiVersionInUrl = true;
           });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddDbContext<ProjetoPraticoContext>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<ILogErroService, LogErroService>();
            services.AddAutoMapper(typeof(Startup));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            if (IdentitServerStartup != null)
                IdentitServerStartup.ConfigureServices(services);


            // config versionamento
            services.AddApiVersioning(p =>
            {
                p.DefaultApiVersion = new ApiVersion(1, 0);
                p.ReportApiVersions = true;
                p.AssumeDefaultVersionWhenUnspecified = true;
            });

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityRequirement(
                    new Dictionary<string, IEnumerable<string>> {
                            { "Bearer", new string[] { } }
                });

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey",
                    Description = "Insira o token JWT desta maneira: Bearer {seu token}"
                });

                /*{ x.SwaggerDoc("V1", new Info
                      {
                          Title = "API Projeto Final Codenation - Squad 4",
                          Version = "V1",
                          Description = "Projeto final desenvolvido pelo squad 4 do curso de aceleração em c# promovido pela Codenation. Neste projeto vamos implementar uma web API para centralizar registros de erros de aplicações.",
                          Contact = new Contact() { Name = "", Email = "" }
                      });
                  })*/
            });


            // config autenticação para API - jwt bearer 
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "http://localhost:5000";
                    options.RequireHttpsMetadata = false;
                    options.Audience = "codenation_projetoFinal";
                });


            services.Configure<ApiBehaviorOptions>(opt =>
            {
                opt.SuppressModelStateInvalidFilter = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //identityServer
            app.UseAuthentication();
            app.UseIdentityServer();

            if (IdentitServerStartup != null)
                IdentitServerStartup.Configure(app, env);

            // swagger
            app.UseSwagger();

            // swagger UI
            app.UseSwaggerUI(options =>
            {
                //s.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
                }

                options.DocExpansion(DocExpansion.List);
            });


            app.UseMvcWithDefaultRoute();
        }
    }
}
