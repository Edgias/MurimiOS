using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Murimi.API.Extensions;
using Murimi.ApplicationCore.Interfaces;
using Murimi.ApplicationCore.SharedKernel;
using Murimi.Infrastructure.Data;
using Murimi.Infrastructure.Data.Repositories;
using Murimi.Infrastructure.Logging;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private readonly string _apiAllowSpecificOrigins = "_apiAllowSpecificOrigins";

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MurimiDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("MurimiDbConnection")));

            services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));
            services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));            
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddObjectMappers();

            services.AddCors(options =>
            {
                options.AddPolicy(_apiAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins("http://localhost:3000",
                                        "http://localhost:3001")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod();
                });
            });

            services.AddControllers().AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new StringToIntConverter())

                );
            services.AddControllers().AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new StringToDecimalConverter()));
            services.AddControllers().AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new StringToGuidConverter()));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(_apiAllowSpecificOrigins);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
