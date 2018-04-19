namespace Unosquare.PassCore.Web
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Models;
    using System;
    using System.Diagnostics;
    using System.IO;
    using Unosquare.PassCore.Web.Helpers;

    /// <summary>
    /// Represents this application's main class
    /// </summary>
    public class Startup
    {
        #region Constant Definitions

        private const string AppSettingsJsonFilename = "appsettings.json";
        private const string LoggingSectionName = "Logging";
        private const string DevelopmentEnvironmentName = "Development";

        #endregion
        
        #region Constructors and Initializers

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup" /> class.
        /// This class gets instantiatied by the Main method. The hosting environment gets provided via DI
        /// </summary>
        /// <param name="environment">The environment.</param>
        public Startup(IHostingEnvironment environment)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder().AddJsonFile(AppSettingsJsonFilename, false, true);

            if (environment.IsEnvironment(DevelopmentEnvironmentName))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        /// <summary>
        /// Application's entry point
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args) => new WebHostBuilder()
            .UseKestrel()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseIISIntegration()
            .UseStartup<Startup>()
            .Build()
            .Run();

        #endregion

        #region Methods

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// All arguments are provided through dependency injection
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            // Register the IConfiguration instance which MyOptions binds against.
            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddMvc();

            services.AddSingleton<IPasswordChangeProvider, PasswordChangeProvider>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// All arguments are provided through dependency injection
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="settings">The settings.</param>
        public void Configure(
            IApplicationBuilder application, 
            IHostingEnvironment environment,
            ILoggerFactory loggerFactory, 
            IOptions<AppSettings> settings)
        {
            loggerFactory.AddConsole(Configuration.GetSection(LoggingSectionName));
            loggerFactory.AddDebug();

            application.Use(async (context, next) =>
            {
                if (context.Request.IsHttps || Debugger.IsAttached || settings.Value.EnableHttpsRedirect == false)
                {
                    await next();
                }
                else
                {
                    var secureRedirectUrl =
                        $"{Uri.UriSchemeHttps}{Uri.SchemeDelimiter}{context.Request.Host}{context.Request.Path}";
                    context.Response.Redirect(secureRedirectUrl);
                }
            });

            application.UseDefaultFiles();
            application.UseStaticFiles();

            // The default route for all non-api routes is the Home controller which in turn, simply outputs the contents of the root 
            // index.html file. This makes the SPA always get back to the index route.
            application.UseMvc(options =>
            {
                options.MapRoute(
                    name: "default", 
                    template: "{*url}",
                    defaults: new {controller = "Home", action = "Index"});
            });
        }

        #endregion
    }
}