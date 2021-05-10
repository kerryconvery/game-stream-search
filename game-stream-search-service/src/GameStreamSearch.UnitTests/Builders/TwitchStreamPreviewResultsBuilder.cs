using System.Collections.Generic;
using GameStreamSearch.StreamProviders.Twitch.Gateways.Dto.Kraken;
using GameStreamSearch.Types;

namespace GameStreamSearch.UnitTests.Builders
{
    public class TwitchStreamPreviewResultsBuilder
    {
        private List<TwitchStreamDto> twitchStreams = new List<TwitchStreamDto>();

        public IEnumerable<TwitchStreamDto> Build()
        {
            return twitchStreams;
        }

        public TwitchStreamPreviewResultsBuilder Add(
            string streamThumbnailUrl,
            int numberOfViewers,
            string channelName,
            string channelLogo,
            string streamUrl,
            string streamName)
        {
            var twitchStream = new TwitchStreamDto
            {
                preview = new TwitchStreamPreviewDto
                {
                    medium = streamThumbnailUrl
                },
                viewers = numberOfViewers,
                channel = new TwitchChannelDto
                {
                    status = streamName,
                    display_name = channelName,
                    logo = channelLogo,
                    url = streamUrl,
                },
            };

            twitchStreams.Add(twitchStream);
           
            return this;
        }
    }
}
