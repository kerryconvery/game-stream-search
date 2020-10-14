﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.StreamProviders.ProviderApi.YouTube.Dto.YouTubeV3;

namespace GameStreamSearch.StreamProviders.ProviderApi.YouTube.Interfaces
{
    public interface IYouTubeV3Api
    {
        Task<YouTubeSearchDto> SearchVideos(string query, VideoEventType eventType, string pageToken);
        Task<YouTubeChannelsDto> GetChannels(string[] channelIds);
        Task<YouTubeVideosDto> GetVideos(string[] videoIds);
    }
}
