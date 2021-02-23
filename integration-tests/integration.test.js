import axios from 'axios';
import config from './config.json';

const baseUrl = config.env[process.env.APP_ENV];

describe.only('Live streams', () => {
  it('should return http status code 200 and one stream from each provider', async () => {
    const response = await axios({
      url: `${baseUrl}/streams?pageSize=1`,
      method: 'get',
      headers: { 'Content-Type': 'application/json' },
    });

    expect(response.status).toEqual(200);
    expect(response.data.streams.length).toEqual(3);
    expect(response.data.streams[0].platformName).toEqual("Twitch");
    expect(response.data.streams[1].platformName).toEqual("YouTube");
    expect(response.data.streams[2].platformName).toEqual("DLive");
    expect(response.data.streams[0].views > response.data.streams[1].views).toBeTruthy();
    expect(response.data.streams[1].views > response.data.streams[2].views).toBeTruthy();
  });

  it('should return two pages of streams', async () => {
    const firstPageResponse = await axios({
      url: `${baseUrl}/streams?pageSize=1`,
      method: 'get',
      headers: { 'Content-Type': 'application/json' },
    });

    const secondPageResponse = await axios({
      url: `${baseUrl}/streams?pageSize=1&pageToken=${firstPageResponse.data.nextPageToken}`,
      method: 'get',
      headers: { 'Content-Type': 'application/json' },
    });

    expect(secondPageResponse.status).toEqual(200);
    expect(secondPageResponse.data.streams.length).toEqual(3);
    expect(secondPageResponse.data.streams[0]).not.toEqual(firstPageResponse.data.streams[0]);
    expect(secondPageResponse.data.streams[1]).not.toEqual(firstPageResponse.data.streams[1]);
    expect(secondPageResponse.data.streams[2]).not.toEqual(firstPageResponse.data.streams[2]);
  });

  it('should return streams filtered by game and only for platforms that support filtering by game', async () => {
    const response = await axios({
      url: `${baseUrl}/streams?pageSize=1&game=fort`,
      method: 'get',
      headers: { 'Content-Type': 'application/json' },
    });

    expect(response.status).toEqual(200);
    expect(response.data.streams.length).toEqual(2);
    expect(response.data.streams[0].platformName).toEqual("Twitch");
    expect(response.data.streams[1].platformName).toEqual("YouTube");
  });
});

describe('Channels', () => {
  it('should add a new channel and return 201 with a link to the new channel', async () => {
    const putResponse = await axios({
      url: `${baseUrl}/channels/twitch/christopherodd`,
      method: 'put',
      headers: { 'Content-Type': 'application/json' },
    });

    expect(putResponse.status).toEqual(201);
    expect(putResponse.config.url).toEqual("http://localhost:5000/api/channels/twitch/christopherodd");
  });

  it('should update an existing channel and return 200', async () => {
    const response = await axios({
      url: `${baseUrl}/channels/twitch/christopherodd`,
      method: 'put',
      headers: { 'Content-Type': 'application/json' },
    });

    expect(response.status).toEqual(204);
  });

  it('should return all channels', async () => {
    const response = await axios({
      url: `${baseUrl}/channels`,
      method: 'get',
      headers: { 'Content-Type': 'application/json' },
    });

    expect(response.status).toEqual(200);
    expect(response.data.items.length).toEqual(1);
  });

  it('should return a single channel', async () => {
    const response = await axios({
      url: `${baseUrl}/channels/Twitch/ChristopherOdd`,
      method: 'get',
      headers: { 'Content-Type': 'application/json' },
    });

    expect(response.status).toEqual(200);
    expect(response.data.channelName).toEqual('ChristopherOdd');
    expect(response.data.streamPlatformId).toEqual('Twitch');
  });
});
