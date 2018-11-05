namespace Unosquare.PassCore.Web
{
    using Helpers;
    using Common;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Models;
#if !DEBUG
    using Microsoft.AspNetCore.Rewrite;
#endif
#if PASSCORE_LDAP_PROVIDER
    using Zyborg.PassCore.PasswordProvider.LDAP;
#else
    using PasswordProvider;
#endif

    /// <summary>
    /// Represents this application's main class.
    /// </summary>
    public class Startup
    {
        private const string AppSettingsJsonFilename = "appsettings.json";
        private const string LoggingSectionName = "Logging";

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup" /> class.
        /// This class gets instantiated by the Main method. The hosting environment gets provided via DI.
        /// </summary>
        public Startup()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile(AppSettingsJsonFilename, false, true)
                .AddEnvironmentVariables()
                .Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        /// <summary>
        /// Application's entry point.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args) => 
            CreateWebHostBuilder(args).Build().Run();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// All arguments are provided through dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<ClientSettings>(Configuration.GetSection(nameof(ClientSettings)));
            services.AddMvc();

#if DEBUG
            services.Configure<IAppSettings>(Configuration.GetSection(nameof(DebugAppSettings)));
            services.AddSingleton<IPasswordChangeProvider, DebugPasswordChangeProvider>();
#elif PASSCORE_LDAP_PROVIDER
            services.Configure<IAppSettings>(Configuration.GetSection(nameof(LdapPasswordChangeOptions)));
            services.AddSingleton<IPasswordChangeProvider, LdapPasswordChangeProvider>();
#else
            services.Configure<IAppSettings>(Configuration.GetSection(nameof(PasswordChangeOptions)));
            services.AddSingleton<IPasswordChangeProvider, PasswordChangeProvider>();
#endif
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// All arguments are provided through dependency injection.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The environment.</param>
        /// <param name="log">The logger factory.</param>
        /// <param name="settings">The settings.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory log, IOptions<ClientSettings> settings)
        {
            // Logging
            log.AddConsole(Configuration.GetSection(LoggingSectionName));

#if DEBUG
            log.AddDebug();
#else
            // HTTPS redirect
            var options = new RewriteOptions().AddRedirectToHttps();
            app.UseRewriter(options);
#endif

            // Enable static files
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // The default route for all non-api routes is the Home controller which in turn, simply outputs the contents of the root
            // index.html file. This makes the SPA always get back to the index route.
            // https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-2.0
            app.UseMvcWithDefaultRoute();
        }
    }
}