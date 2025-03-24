using House.API.Filter;
using House.API.Middleware;
using House.API.ServiceDI;
using House.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace House.API
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
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            //Filter 全域設定
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(APIActionFilter));
                //options.Filters.Add(typeof(APIExceptionFilter));
            });

            services.AddPolicy();

            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true); //讓Attribute驗證在Action才觸發

            services.AddControllers().AddNewtonsoftJson();

            services.AddCorsByEnv();

            #region Swagger
            //services.AddEndpointsApiExplorer();
            //services.AddSwaggerGen(options =>
            //{
            //    options.ExampleFilters(); //顯示 Example
            //    options.SwaggerDoc("v1", new OpenApiInfo { Title = "House API", Version = "v1" });//版本下拉分類
            //    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"; //顯示 Summary 註解
            //    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            //});
            //services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly()); //顯示 Example
            #endregion


            // Add Hangfire services.
            //services.AddHangfire(configuration => configuration
            //  .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            //  .UseSimpleAssemblyNameTypeSerializer()
            //  .UseRecommendedSerializerSettings());

            // Add the processing server as IHostedService
            //services.AddHangfireServer();

            services.AddScoped<APIAuthorizationAttribute>();
            services.AddScoped<ChecksumFilter>();

            services.AddHouseDao();
            services.AddHouseServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //ConfigureSwagger(app, env);

            // 使用Dashboard功能
            //app.UseHangfireDashboard();

            app.UseHttpsRedirection();

            app.UseLogMiddleware();

            //app.UseCookieMiddleware();

            app.UseRouting();

            app.UseAuthorization();

            ConfigureCors(app, env);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureSwagger(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var environmentName = env.EnvironmentName;
            if (env.IsEnvironment("Local"))
            {
                app.UseDeveloperExceptionPage();
                UseSwagger(app);
            }
            else if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                UseSwagger(app);
            }
            else if (env.IsStaging())
            {
                app.UseDeveloperExceptionPage();
                UseSwagger(app);
            }
            else
            {
                return;
            }

            void UseSwagger(IApplicationBuilder app)
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "House API V1");
                    c.RoutePrefix = "swagger";
                });
            }
        }

        private void ConfigureCors(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsEnvironment("Local"))
            {
                app.UseCors("Local");
            }
            else if (env.IsDevelopment())
            {
                app.UseCors("Development");
            }
            else if (env.IsStaging())
            {
                app.UseCors("Staging");
            }
            else
            {
                app.UseCors("Production");
            }
        }
    }
}
