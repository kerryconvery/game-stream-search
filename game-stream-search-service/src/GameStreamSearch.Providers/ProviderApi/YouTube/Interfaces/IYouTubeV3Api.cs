﻿using System;
using System.Threading.Tasks;
using GameStreamSearch.StreamProviders.ProviderApi.YouTube.Dto.YouTubeV3;

namespace GameStreamSearch.StreamProviders.ProviderApi.YouTube.Interfaces
{
    public interface IYouTubeV3Api
    {
        Task<YouTubeVideoSearchDto> SearchVideos(string query, VideoEventType eventType);
    }
}
