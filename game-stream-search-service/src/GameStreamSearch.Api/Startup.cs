using AspNetCoreRateLimit;
using GameStreamSearch.Api.Infrastructor;
using GameStreamSearch.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GameStreamSearch.Application.Services;
using GameStreamSearch.StreamProviders;
using GameStreamSearch.StreamProviders.ProviderApi.Twitch;
using GameStreamSearch.StreamProviders.Builders;
using GameStreamSearch.StreamProviders.ProviderApi.YouTube;
using GameStreamSearch.StreamProviders.ProviderApi.DLive;

namespace GameStreamSearch.Api
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
            // needed to store rate limit counters and ip rules
            services.AddMemoryCache();

            //load general configuration from appsettings.json
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));

            // inject counter and rules stores
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            // https://github.com/aspnet/Hosting/issues/793
            // the IHttpContextAccessor service is not registered by default.
            // the clientId/clientIp resolvers use it.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // configuration (resolvers, counter key builders)
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            services.AddCors(options =>
            {
                options.AddPolicy("allowAll",
                builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            services.AddControllers();
            services.AddScoped<IPaginator, Paginator>();
            services.AddScoped<StreamAggregationService>(service =>
            {
                return new StreamAggregationService(service.GetService<IPaginator>())
                    .RegisterStreamProvider(new TwitchStreamProvider(
                        "Twitch",
                        new TwitchKrakenApi(Configuration["Twitch:ApiUrl"], Configuration["Twitch:ClientId"])
                    ))
                    .RegisterStreamProvider(new YouTubeStreamProvider(
                        "YouTube",
                        new YouTubeWatchUrlBuilder(Configuration["YouTube:WatchUrl"]),
                        new YouTubeV3Api(Configuration["YouTube:ApiUrl"], Configuration["YouTube:ApiKey"])
                    ))
                    .RegisterStreamProvider(new DLiveStreamProvider(
                        "DLive",
                        new DLiveWatchUrlBuilder(Configuration["DLive:WatchUrl"]),
                        new DLiveGraphQLApi(Configuration["DLive:Apiurl"])));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseIpRateLimiting();
            app.UseRouting();

            app.UseCors("allowAll");

            app.UseEndpoints(endpoints => endpoints.MapControllers());

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "text/plain";

                    await context.Response.WriteAsync("An internal server error has occurred");
                });
            });
        }
    }
}
