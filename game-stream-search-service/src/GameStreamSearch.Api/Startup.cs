using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GameStreamSearch.Application;
using Newtonsoft.Json.Converters;
using GameStreamSearch.Types;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Repositories;
using GameStreamSearch.DataAccess;
using GameStreamSearch.DataAccess.Dto;
using GameStreamSearch.Application.Services.StreamProvider;
using GameStreamSearch.StreamProviders.Twitch.Gateways;
using GameStreamSearch.StreamProviders.Twitch;
using GameStreamSearch.StreamProviders.Twitch.Mappers;
using GameStreamSearch.StreamProviders.YouTube;
using GameStreamSearch.StreamProviders.YouTube.Mappers.V3;
using GameStreamSearch.StreamProviders.YouTube.Gateways.V3;
using GameStreamSearch.StreamProviders.DLive;
using GameStreamSearch.StreamProviders.DLive.Gateways;
using GameStreamSearch.StreamProviders.DLive.Mappers;
using GameStreamSearch.Application.RegisterOrUpdateChannel;
using GameStreamSearch.Application.GetAllChannels;
using GameStreamSearch.Application.GetASingleChannel;
using GameStreamSearch.Application.GetStreams;
using GameStreamSearch.Application.GetStreams.Dto;

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
                return new StreamPlatformService()
                    .RegisterStreamProvider(new TwitchStreamProvider(
                            new TwitchKrakenGateway(Configuration["Twitch:ApiUrl"], Configuration["Twitch:ClientId"]),
                            new TwitchStreamMapper(),
                            new TwitchChannelMapper()
                    ))
                    .RegisterStreamProvider(new YouTubeStreamProvider(
                            new YouTubeV3Gateway(Configuration["YouTube:ApiUrl"], Configuration["YouTube:ApiKey"]),
                            new YouTubeStreamMapper(Configuration["YouTube:WebUrl"]),
                            new YouTubeChannelMapper(Configuration["YouTube:WebUrl"])
                    ))
                    .RegisterStreamProvider(new DLiveStreamProvider(
                            new DLiveGraphQLGateway(Configuration["DLive:Apiurl"]),
                            new DLiveStreamMapper(Configuration["DLive:WebUrl"]),
                            new DLiveChannelMapper(Configuration["DLive:WebUrl"])
                    ));
            });

            services.AddScoped<ICommandHandler<RegisterOrUpdateChannelCommand, RegisterOrUpdateChannelCommandResult>, RegisterOrUpdateChannelCommandHandler>();
            services.AddScoped<IQueryHandler<GetStreamsQuery, AggregatedStreamsDto>, GetStreamsQueryHandler>();
            services.AddScoped<IQueryHandler<GetAllChannelsQuery, ChannelListDto>, GetAllChannelsQueryHandler>();
            services.AddScoped<IQueryHandler<GetASingleChannelQuery, Maybe<ChannelDto>>, GetASingleChannelQueryHandler>();

            services.AddSingleton<AwsDynamoDbTable<ChannelTableDto>, AwsDynamoDbTable<ChannelTableDto>>();
            services.AddSingleton<ChannelRepository>();
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
