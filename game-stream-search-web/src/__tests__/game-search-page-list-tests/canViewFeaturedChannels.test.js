import React from 'react';
import { render, waitFor, screen, fireEvent } from '@testing-library/react';
import nock from 'nock';
import App from '../../app';
import { StreamServiceProvider } from '../../providers/StreamServiceProvider';
import { TelemetryTrackerProvider } from '../../providers/TelemetryTrackerProvider';
import { getStreamServiceApi } from '../../api/streamServiceApi';
import { getTelemetryTrackerApi } from '../../api/telemetryTrackerApi';
import autoMockObject from '../../test-utils/autoMockObject';
import '@testing-library/jest-dom/extend-expect';

describe('Can view featured channels', () => {
  it('should display a list of Featured channels on startup', async () => {
    const channelList = {
      channels: [
        {
          channelName: 'testchannel',
          platformName: 'Twitch',
          avatarUrl: '',
          channelUrl: '',
        }
      ]
    };

    nock('http://localhost:5000')
      .defaultReplyHeaders({
        'access-control-allow-origin': '*',
        'access-control-allow-credentials': 'true' ,
      })
      .get('/api/channels')
      .reply(200, channelList);

    await renderApplication();

    const featuredChannel = await waitFor(() => screen.getByText("testchannel"));

    expect(featuredChannel).toBeInTheDocument();
  });

  it('should trigger a stream channel opened telemetry event', async () => {
    const channelList = {
      channels: [
        {
          channelName: 'testchannel',
          platformName: 'Twitch',
          avatarUrl: '',
          channelUrl: '',
        }
      ]
    };

    nock('http://localhost:5000')
      .defaultReplyHeaders({
        'access-control-allow-origin': '*',
        'access-control-allow-credentials': 'true' ,
      })
      .get('/api/channels')
      .reply(200, channelList);

    await renderApplication();

    const featuredChannel = await waitFor(() => screen.getByText("testchannel"));

    fireEvent.click(featuredChannel);

    expect(telemetryTrackerApiMock.trackFeaturedChannelOpened).toHaveBeenCalled();
  });
  
  beforeEach(() => {
    nock('http://localhost:5000')
    .defaultReplyHeaders({
      'access-control-allow-origin': '*',
      'access-control-allow-credentials': 'true' 
    })
    .get('/api/streams?pageSize=10')
    .reply(200, { streams: [] });
  });

  const telemetryTrackerApiMock = autoMockObject(getTelemetryTrackerApi({}));

  const renderApplication = () => {
    render(
      <StreamServiceProvider streamServiceApi={getStreamServiceApi("http://localhost:5000/api")} >
        <TelemetryTrackerProvider telemetryTrackerApi={telemetryTrackerApiMock}>
          <App />
        </TelemetryTrackerProvider>
      </StreamServiceProvider>
    )

    return waitFor(() => screen.getByTestId('streams-not-found'));
  };
})
