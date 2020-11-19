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

const getStreamChannelsRequest = (baseUrl) => () => (
  axios({
    url: `${baseUrl}/channels`,
    method: 'GET',
    params: {},
  }).then(res => res.data)
);

const getStreamChannelsStub = () => {
  const channels = [
    {
      name: 'test channel 1',
      streamPlatformDisplayName: 'YouTube'
    },
    {
      name: 'test channel 2',
      streamPlatformDisplayName: 'DLive'
    },
    {
      name: 'test channel 3',
      streamPlatformDisplayName: 'Twitch'
    },
  ]

  return Promise.resolve(channels);
}

export const useGameStreamApi = () => {
  const { streamSearchServiceUrl } = useConfiguration();

  return {
    getStreams: getStreamsRequest(streamSearchServiceUrl),
    getStreamChannels: getStreamChannelsStub, //getStreamChannelsRequest(streamSearchServiceUrl), 
  }
}