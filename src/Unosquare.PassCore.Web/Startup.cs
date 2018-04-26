namespace Unosquare.PassCore.Web
{
    using Helpers;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Rewrite;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Models;

    /// <summary>
    /// Represents this application's main class
    /// </summary>
    public class Startup
    {
        #region Constant Definitions

        private const string AppSettingsJsonFilename = "appsettings.json";
        private const string LoggingSectionName = "Logging";

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

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        /// <summary>
        /// Application's entry point
        /// </summary>
        /// <param name="args">The arguments.</param>

        #endregion

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        #region Methods

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .Build();

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// All arguments are provided through dependency injection
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            // Register the IConfiguration instance which MyOptions binds against.
            services.Configure<AppSettings>(Configuration.GetSection("AppSettingsSectionName"));
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddMvc();

            services.AddSingleton<IPasswordChangeProvider, PasswordChangeProvider>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// All arguments are provided through dependency injection
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The environment.</param>
        /// <param name="log">The logger factory.</param>
        /// <param name="settings">The settings.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory log, IOptions<AppSettings> settings)
        {
            // Logging
            log.AddConsole(Configuration.GetSection(LoggingSectionName));
            log.AddDebug();

            if (settings.Value.EnableHttpsRedirect)
            {
                var options = new RewriteOptions()
                    .AddRedirectToHttps();

                app.UseRewriter(options);
            }
            
            // Enable static files
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // The default route for all non-api routes is the Home controller which in turn, simply outputs the contents of the root
            // index.html file. This makes the SPA always get back to the index route.
            // https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-2.0
            app.UseMvcWithDefaultRoute();
        }

        #endregion
    }
}