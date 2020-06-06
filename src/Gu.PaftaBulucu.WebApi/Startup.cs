using Gu.PaftaBulucu.Business.Services;
using Gu.PaftaBulucu.Data;
using Gu.PaftaBulucu.Data.Models;
using Gu.PaftaBulucu.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Npgsql;

namespace Gu.PaftaBulucu.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddControllers();

            services.AddAWSService<Amazon.S3.IAmazonS3>();

            services.AddDbContext<GuDbContext>(options => options.UseNpgsql(Configuration["postgres:connectionString"]).UseSnakeCaseNamingConvention());

            NpgsqlConnection.GlobalTypeMapper.UseJsonNet(settings: new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            services.AddTransient<ISheetService, SheetService>();
            services.AddTransient<IProjectService, ProjectService>();
            services.AddTransient<IMailChimpService, MailChimpService>();
            services.AddTransient<ISheetRepository, SheetRepository>();
            services.AddScoped<IDatabaseRepository<Project>, ProjectRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
