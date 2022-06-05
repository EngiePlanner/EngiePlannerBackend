using AutoMapper;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Services;
using BusinessObjectLayer.Dtos;
using BusinessObjectLayer.Entities;
using BusinessObjectLayer.Helpers;
using BusinessObjectLayer.Validators;
using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Text;

namespace EngiePlanner
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

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    var keyByteArray = Encoding.ASCII.GetBytes(Configuration["JwtKey"]);
                    var signingKey = new SymmetricSecurityKey(keyByteArray);
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = signingKey,
                        ValidAudience = Configuration["JwtAudience"],
                        ValidIssuer = Configuration["JwtIssuer"],
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            
            var mapperConfig = new MapperConfiguration(mc => {
                mc.AddProfile(new Mappers());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddDbContext<EngiePlannerContext>();

            services.AddCors();

            services.AddMvcCore();

            var context = new CustomAssemblyLoadContext();
            context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "libwkhtmltox.dll"));

            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            services.AddControllers()
                    .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.AddHttpContextAccessor();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Jwt", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                });
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();

            services.AddScoped<IValidator<TaskDto>, TaskValidator>();
            services.AddScoped<IValidator<UserEntity>, UserValidator>();
            services.AddScoped<IValidator<AvailabilityEntity>, AvailabilityValidator>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IAspSolverService, AspSolverService>();

            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EngiePlanner", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VybmFtZSI6IlJPQzJDTEoiLCJOYW1lIjoiQ3Jpc3RpYW4gUm90YXIiLCJEaXNwbGF5TmFtZSI6IkZJWEVELVRFUk0gUm90YXIgQ3Jpc3RpYW4gKFBTLUVDL0VUWDEpIiwiRW1haWwiOiJmaXhlZC10ZXJtLkNyaXN0aWFuLlJvdGFyQHJvLmJvc2NoLmNvbSIsIlJvbGUiOiJBZG1pbiIsIkxlYWRlclVzZXJuYW1lIjoiUkJBNUNMSiIsIkxlYWRlck5hbWUiOiJSYXp2YW4gQmFybGVhIiwiR3JvdXBzIjoiUFMtRUMvRVRYMSIsIkRlcGFydG1lbnRzIjoiUFMtRUMvRVRYIiwibmJmIjoxNjUxNjgwNTMxLCJleHAiOjE2NTE3NjY5MzEsImlhdCI6MTY1MTY4MDUzMSwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo0NDMwMSIsImF1ZCI6IkF1ZGllbmNlIn0.FiPjA8WZnOX496LI8jCBHadTdsV9QLM_AIMfaxXCo8w"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EngiePlanner v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(x =>
                   x.AllowAnyMethod());

            app.UseAuthentication();
            
            app.UseAuthorization();

            app.UseMiddleware<NtlmAndAnonymousSetupMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            string baseDir = env.ContentRootPath;
            AppDomain.CurrentDomain.SetData("AspDataDirectory", Path.Combine(baseDir, "AspData"));
        }
    }
}
