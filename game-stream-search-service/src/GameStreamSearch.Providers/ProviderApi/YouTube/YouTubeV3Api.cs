﻿using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.StreamProviders.ProviderApi.YouTube.Dto.YouTubeV3;
using GameStreamSearch.StreamProviders.ProviderApi.YouTube.Interfaces;
using RestSharp;

namespace GameStreamSearch.StreamProviders.ProviderApi.YouTube
{
    public enum VideoEventType
    {
        Completed,
        Live,
        Upcoming
    }

    public class YouTubeV3Api : IYouTubeV3Api
    {
        private readonly string googleApiUrl;
        private readonly string googleApiKey;

        public YouTubeV3Api(string googleApiUrl, string googleApiKey)
        {
            this.googleApiUrl = googleApiUrl;
            this.googleApiKey = googleApiKey;
        }

        public async Task<YouTubeVideoSearchDto> SearchVideos(string query, VideoEventType eventType, string pageToken)
        {
            var client = new RestClient(this.googleApiUrl);

            var request = new RestRequest("/youtube/v3/search", Method.GET);

            request.AddParameter("part", "snippet");
            request.AddParameter("eventType", eventType.ToString().ToLower());
            request.AddParameter("q", query);
            request.AddParameter("type", "video");
            request.AddParameter("videoCategoryId", 20);
            request.AddParameter("key", googleApiKey);
            request.AddParameter("pageToken", pageToken);

            request.AddHeader("Accept", "application/json");

            var response = await client.ExecuteAsync<YouTubeVideoSearchDto>(request);

            return response.Data;
        }

        public async Task<YouTubeLiveStreamDetailsDto> GetLiveStreamDetails(IEnumerable<string> videoIds)
        {

            var client = new RestClient(this.googleApiUrl);

            var request = new RestRequest("/youtube/v3/videos", Method.GET);


            request.AddParameter("part", "id");
            request.AddParameter("part", "liveStreamingDetails");

            foreach (var id in videoIds)
            {
                request.AddParameter("id", id);
            }

            request.AddParameter("key", googleApiKey);

            request.AddHeader("Accept", "application/json");

            var response = await client.ExecuteAsync<YouTubeLiveStreamDetailsDto>(request);

            return response.Data;
        }

        public async Task<YouTubeVideoStatisticsPartDto> GetVideoStatistics(IEnumerable<string> videoIds)
        {

            var client = new RestClient(this.googleApiUrl);

            var request = new RestRequest("/youtube/v3/videos", Method.GET);


            request.AddParameter("part", "id");
            request.AddParameter("part", "statistics");

            foreach (var id in videoIds)
            {
                request.AddParameter("id", id);
            }

            request.AddParameter("key", googleApiKey);

            request.AddHeader("Accept", "application/json");

            var response = await client.ExecuteAsync<YouTubeVideoStatisticsPartDto>(request);

            return response.Data;
        }

        public async Task<YouTubeChannelsDto> GetChannels(IEnumerable<string> channelIds)
        {
            var client = new RestClient(this.googleApiUrl);

            var request = new RestRequest("/youtube/v3/channels", Method.GET);


            request.AddParameter("part", "id");
            request.AddParameter("part", "snippet");

            foreach (var id in channelIds)
            {
                request.AddParameter("id", id);
            }

            request.AddParameter("key", googleApiKey);

            request.AddHeader("Accept", "application/json");

            var response = await client.ExecuteAsync<YouTubeChannelsDto>(request);

            return response.Data;
        }
    }
}