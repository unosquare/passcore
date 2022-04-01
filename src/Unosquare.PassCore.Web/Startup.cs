using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Unosquare.PassCore.Web.Models;
#if DEBUG
using Unosquare.PassCore.Web.Helpers;
#elif PASSCORE_LDAP_PROVIDER
using Zyborg.PassCore.PasswordProvider.LDAP;
using Microsoft.Extensions.Logging;
#else
using Unosquare.PassCore.PasswordProvider;
#endif

namespace Unosquare.PassCore.Web;

/// <summary>
/// Represents this application's main class.
/// </summary>
public class Startup
{
    private const string AppSettingsSectionName = "AppSettings";

    /// <summary>
    /// Initializes a new instance of the <see cref="Startup" /> class.
    /// This class gets instantiated by the Main method. The hosting environment gets provided via DI.
    /// </summary>
    /// <param name="config">The configuration.</param>
    public Startup(IConfiguration config) => Configuration = config;

    /// <summary>
    /// Gets or sets the configuration.
    /// </summary>
    /// <value>
    /// The configuration.
    /// </value>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// Application's entry point.
    /// </summary>
    /// <param name="args">The arguments.</param>
    public static async Task Main(string[] args) => await WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().Build().RunAsync();

    /// <summary>
    /// Creates the web host builder.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>The web host builder.</returns>
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
        services.Configure<ClientSettings>(Configuration.GetSection(nameof(ClientSettings)));
        services.Configure<WebSettings>(Configuration.GetSection(nameof(WebSettings)));
#if DEBUG
        services.Configure<IAppSettings>(Configuration.GetSection(AppSettingsSectionName));
        services.AddSingleton<IPasswordChangeProvider, DebugPasswordChangeProvider>();
#elif PASSCORE_LDAP_PROVIDER
        services.Configure<LdapPasswordChangeOptions>(Configuration.GetSection(AppSettingsSectionName));
        services.AddSingleton<IPasswordChangeProvider, LdapPasswordChangeProvider>();
        services.AddSingleton<ILoggerFactory, LoggerFactory>();
        services.AddSingleton(typeof(ILogger), sp =>
        {
            var loggerFactory = sp.GetService<ILoggerFactory>();
            return loggerFactory.CreateLogger("PassCoreLDAPProvider");
        });
#else
        services.Configure<PasswordChangeOptions>(Configuration.GetSection(AppSettingsSectionName));
        services.AddSingleton<IPasswordChangeProvider, PasswordChangeProvider>();
#endif

        // Add framework services.
        services.AddControllers();
    }

    /// <summary>
    /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    /// All arguments are provided through dependency injection.
    /// </summary>
    /// <param name="app">The application.</param>
    /// <param name="settings">The settings.</param>
    public void Configure(IApplicationBuilder app, IOptions<WebSettings> settings)
    {
        if (settings.Value.EnableHttpsRedirect)
            app.UseHttpsRedirection();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
