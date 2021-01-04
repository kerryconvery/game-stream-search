import React from 'react';
import { render, waitFor, screen, fireEvent } from '@testing-library/react';
import nock from 'nock';
import { ConfigurationProvider } from '../../providers/ConfigurationProvider';
import App from '../../app';
import { TelemetryTrackerProvider } from '../../providers/TelemetryTrackerProvider';
import { telemetryTrackerApiMocks } from '../../test-helpers/mocks';
import '@testing-library/jest-dom/extend-expect';

describe('Featured channels side bar', () => {
  beforeEach(() => {
    nock('http://localhost:5000')
    .defaultReplyHeaders({
      'access-control-allow-origin': '*',
      'access-control-allow-credentials': 'true' 
    })
    .get('/api/streams?pageSize=10')
    .reply(200, { items: [] });
  });

  const renderApplication = () => {
    return render(
      <ConfigurationProvider configuration={{ "streamSearchServiceUrl": "http://localhost:5000/api" }} >
        <TelemetryTrackerProvider telemetryTrackerApi={telemetryTrackerApiMocks}>
          <App />
        </TelemetryTrackerProvider>
      </ConfigurationProvider>
    )
  };

  it('should display a list of Featured channels on startup', async () => {
    const channelList = {
      items: [
        {
          channelName: 'testchannel',
          streamPlatformDisplayName: 'Twitch',
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

    renderApplication();

    await waitFor(() => screen.getByTestId('streams-not-found'));

    const featuredChannel = await waitFor(() => screen.getByText("testchannel"));

    expect(featuredChannel).toBeInTheDocument();
  });

  it('should trigger a stream channel opened telemetry event', async () => {
    const channelList = {
      items: [
        {
          channelName: 'testchannel',
          streamPlatformDisplayName: 'Twitch',
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

    renderApplication();

    await waitFor(() => screen.getByTestId('streams-not-found'));

    const featuredChannel = await waitFor(() => screen.getByText("testchannel"));

    fireEvent.click(featuredChannel);

    expect(telemetryTrackerApiMocks.trackFeaturedChannelOpened).toHaveBeenCalled();
  });
})
