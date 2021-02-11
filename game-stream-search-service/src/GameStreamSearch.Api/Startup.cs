using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GameStreamSearch.Application.Services;
using GameStreamSearch.StreamProviders;
using GameStreamSearch.Application.Providers;
using Newtonsoft.Json.Converters;
using GameStreamSearch.Application;
using GameStreamSearch.Repositories;
using GameStreamSearch.Application.Commands;
using GameStreamSearch.StreamProviders.Gateways;
using GameStreamSearch.StreamProviders.Mappers;
using GameStreamSearch.Repositories.Dto;

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

            services.AddControllers()
                .AddNewtonsoftJson(opts => opts.SerializerSettings.Converters.Add(new StringEnumConverter()));

            services.AddScoped(service =>
            {
                return new ProviderAggregationService()
                    .RegisterStreamProvider(new TwitchStreamProvider(
                            new TwitchKrakenGateway(Configuration["Twitch:ApiUrl"], Configuration["Twitch:ClientId"]),
                            new TwitchMapper()
                    ))
                    .RegisterStreamProvider(new YouTubeStreamProvider(
                            new YouTubeV3Gateway(Configuration["YouTube:ApiUrl"], Configuration["YouTube:ApiKey"]),
                            new YouTubeMapper(Configuration["YouTube:WebUrl"])
                    ))
                    .RegisterStreamProvider(new DLiveStreamProvider(
                            new DLiveGraphQLGateway(Configuration["DLive:Apiurl"]),
                            new DLiveMapper(Configuration["DLive:WebUrl"])
                    ));
            });

            services.AddScoped<IStreamService>(x => x.GetRequiredService<ProviderAggregationService>());
            services.AddScoped<IChannelService>(x => x.GetRequiredService<ProviderAggregationService>());

            services.AddScoped<ICommandHandler<RegisterOrUpdateChannelCommand, RegisterOrUpdateChannelCommandResult>, RegisterOrUpdateChannelCommandHandler>();
            
            services.AddScoped<ITimeProvider, UtcTimeProvider>();

            services.AddSingleton<AwsDynamoDbGateway<DynamoDbChannelDto>, AwsDynamoDbGateway<DynamoDbChannelDto>>();
            services.AddSingleton<IChannelRepository, ChannelRepository>();
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
