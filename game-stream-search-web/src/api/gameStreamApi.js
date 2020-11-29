import { useConfiguration } from '../providers/ConfigurationProvider';
import axios from 'axios';

const getStreamsRequest = (baseUrl) => (filters = {}, pageToken) => (
  axios({
    url: `${baseUrl}/streams`,
    method: 'GET',
    params: {
      game: filters.gameName,
      pageSize: 10,
      pageToken: pageToken,
    },
  }).then(res => res.data)
);

const addChannelRequest = (baseUrl) => (data) => {
  axios({
    url: `${baseUrl}/channels/${data.streamPlatform}/${data.channelName}`,
    method: 'PUT',
  })
}

const getChannelsRequest = (baseUrl) => () => (
  axios({
    url: `${baseUrl}/channels`,
    method: 'GET',
    params: {},
  }).then(res => res.data.items)
);

const getStreamChannelsStub = () => {
  const channels = [
    {
      name: 'test channel 1 aaaa',
      streamPlatformDisplayName: 'YouTube',
      channelAvatarUrl: 'https://yt3.ggpht.com/ytc/AAUvwngVV8TZXaMQz3lJakmL-6S4A5LrUBBzjYFWpjZOIg=s88-c-k-c0x00ffffff-no-rj',
      channelUrl: 'https://www.twitch.tv/christopherodd',
    },
    {
      name: 'test channel 2',
      streamPlatformDisplayName: 'DLive',
      channelAvatarUrl: "https://yt3.ggpht.com/ytc/AAUvwni-mN_VAh1jj1FFGhNKe1iQZLNqDiXAdiuyw4hz=s88-c-k-c0x00ffffff-no-rj",
      channelUrl: 'https://www.twitch.tv/christopherodd',
    },
    // {
    //   name: 'test channel 3',
    //   streamPlatformDisplayName: 'Twitch'
    // },
    // {
    //   name: 'test channel 1',
    //   streamPlatformDisplayName: 'YouTube'
    // },
    // {
    //   name: 'test channel 2',
    //   streamPlatformDisplayName: 'DLive'
    // },
    // {
    //   name: 'test channel 3',
    //   streamPlatformDisplayName: 'Twitch'
    // },    {
    //   name: 'test channel 1',
    //   streamPlatformDisplayName: 'YouTube'
    // },
    // {
    //   name: 'test channel 2',
    //   streamPlatformDisplayName: 'DLive'
    // },
    // {
    //   name: 'test channel 3',
    //   streamPlatformDisplayName: 'Twitch'
    // },    {
    //   name: 'test channel 1',
    //   streamPlatformDisplayName: 'YouTube'
    // },
    // {
    //   name: 'test channel 2',
    //   streamPlatformDisplayName: 'DLive'
    // },
    // {
    //   name: 'test channel 3',
    //   streamPlatformDisplayName: 'Twitch'
    // },    {
    //   name: 'test channel 1',
    //   streamPlatformDisplayName: 'YouTube'
    // },
    // {
    //   name: 'test channel 2',
    //   streamPlatformDisplayName: 'DLive'
    // },
    // {
    //   name: 'test channel 3',
    //   streamPlatformDisplayName: 'Twitch'
    // },    {
    //   name: 'test channel 1',
    //   streamPlatformDisplayName: 'YouTube'
    // },
    // {
    //   name: 'test channel 2',
    //   streamPlatformDisplayName: 'DLive'
    // },
    // {
    //   name: 'test channel 3',
    //   streamPlatformDisplayName: 'Twitch'
    // },    {
    //   name: 'test channel 1',
    //   streamPlatformDisplayName: 'YouTube'
    // },
    // {
    //   name: 'test channel 2',
    //   streamPlatformDisplayName: 'DLive'
    // },
    // {
    //   name: 'test channel 3',
    //   streamPlatformDisplayName: 'Twitch'
    // },    {
    //   name: 'test channel 1',
    //   streamPlatformDisplayName: 'YouTube'
    // },
    // {
    //   name: 'test channel 2',
    //   streamPlatformDisplayName: 'DLive'
    // },
    {
      name: 'test channel 3 eee',
      streamPlatformDisplayName: 'Twitch',
      channelAvatarUrl: "https://images.prd.dlivecdn.com/avatar/ea9793cd-a47b-11ea-b737-e2443572cd01",
      channelUrl: 'https://www.twitch.tv/christopherodd',
    },
  ]

  return Promise.resolve(channels);
}

export const useGameStreamApi = () => {
  const { streamSearchServiceUrl } = useConfiguration();

  return {
    getStreams: getStreamsRequest(streamSearchServiceUrl),
    addChannel: addChannelRequest(streamSearchServiceUrl),
    getChannels: getChannelsRequest(streamSearchServiceUrl), 
  }
}