using Gu.PaftaBulucu.Business.Services;
using Gu.PaftaBulucu.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddAWSService<Amazon.DynamoDBv2.IAmazonDynamoDB>();

            services.AddTransient<ISheetService, SheetService>();
            services.AddTransient<IProjectService, ProjectService>();
            services.AddTransient<ISheetRepository, SheetRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
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
