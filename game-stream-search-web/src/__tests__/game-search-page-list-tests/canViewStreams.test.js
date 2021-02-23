import React from 'react';
import { render, fireEvent, waitFor, screen } from '@testing-library/react';
import nock from 'nock';
import App from '../../app';
import { StreamServiceProvider } from '../../providers/StreamServiceProvider';
import { TelemetryTrackerProvider } from '../../providers/TelemetryTrackerProvider';
import { getStreamServiceApi } from '../../api/streamServiceApi';
import { getTelemetryTrackerApi } from '../../api/telemetryTrackerApi';
import autoMockObject from '../../test-utils/autoMockObject';
import '@testing-library/jest-dom/extend-expect';

describe('When viewing streams', () => {
  it('should render streams without errors', async () => {
    const responseData = {
      streams: [{
        streamTitle: 'fake stream',
        streamThumbnailUrl: 'http://fake.stream1.thumbnail',
        streamUrl: 'fake.stream1.url',
        streamerName: 'fake steamer',
        streamerAvatarUrl: 'http://fake.channel1.url',
        platformName: 'fake platform',
        isLive: true,
        views: 100
      }],
      nextPageToken: 'nextPage',
    }

    nock('http://localhost:5000')
      .defaultReplyHeaders({
        'access-control-allow-origin': '*',
        'access-control-allow-credentials': 'true' 
      })
      .get('/api/streams?pageSize=10')
      .reply(200, responseData);

    renderApplication();

    const fakeStream = await waitFor(() => screen.getByText('fake stream'));
    const loadingTiles = await waitFor(() => screen.queryByTestId('stream-loading-tile'));
    
    expect(fakeStream).toBeInTheDocument();
    expect(screen.queryByRole('alert')).not.toBeInTheDocument();
    expect(screen.queryByTestId('streams-not-found')).not.toBeInTheDocument();
    expect(loadingTiles).not.toBeInTheDocument();
  });

  it('should render loading tiles while loading streams', async () => {
    const responseData = {
      streams: [{
        streamTitle: 'fake stream',
        streamThumbnailUrl: 'http://fake.stream1.thumbnail',
        streamUrl: 'fake.stream1.url',
        streamerName: 'fake steamer',
        streamerAvatarUrl: 'http://fake.channel1.url',
        platformName: 'fake platform',
        isLive: true,
        views: 100
      }],
      nextPageToken: 'nextPage',
    }

    nock('http://localhost:5000')
      .defaultReplyHeaders({
        'access-control-allow-origin': '*',
        'access-control-allow-credentials': 'true' 
      })
      .get('/api/streams?pageSize=10')
      .reply(200, responseData);

    renderApplication();

    const loadingTiles = await waitFor(() => screen.getAllByTestId('stream-loading-tile'));
    
    expect(loadingTiles[0]).toBeInTheDocument();

    //Wait until screen has finished render to avoid unmount error
    await waitFor(() => screen.getAllByText('fake stream'));
  })

  it('should display an error alerts when there is an error getting the streams', async () =>{
    nock('http://localhost:5000')
      .defaultReplyHeaders({
        'access-control-allow-origin': '*',
        'access-control-allow-credentials': 'true' 
      })
      .get('/api/streams?pageSize=10')
      .reply(500);

    renderApplication();

    const alert = await waitFor(() => { 
      return screen.getByText('The application is currently offline. Please try back later.');
    });
    
    expect(alert).toBeInTheDocument();
  });

  it('should trigger a stream opened telemetry event when a stream is clicked on', async () => {
    const responseData = {
      streams: [{
        streamTitle: 'fake stream',
        streamThumbnailUrl: 'http://fake.stream1.thumbnail',
        streamUrl: 'fake.stream1.url',
        streamerName: 'fake steamer',
        streamerAvatarUrl: 'http://fake.channel1.url',
        platformName: 'fake platform',
        isLive: true,
        views: 100
      }],
      nextPageToken: 'nextPage',
    }

    nock('http://localhost:5000')
      .defaultReplyHeaders({
        'access-control-allow-origin': '*',
        'access-control-allow-credentials': 'true' 
      })
      .get('/api/streams?pageSize=10')
      .reply(200, responseData);

    renderApplication();

    const stream = await waitFor(() => screen.getByText('fake stream'));
    
    fireEvent.click(stream);
    
    expect(telemetryTrackerApiMock.trackStreamOpened).toHaveBeenCalled();
  });

  beforeEach(() => {
    nock('http://localhost:5000')
    .defaultReplyHeaders({
      'access-control-allow-origin': '*',
      'access-control-allow-credentials': 'true' ,
    })
    .get('/api/channels')
    .reply(200, { channels: [] });
  });
  
  const telemetryTrackerApiMock = autoMockObject(getTelemetryTrackerApi({}));

  const Application = () => (
    <StreamServiceProvider streamServiceApi={getStreamServiceApi("http://localhost:5000/api")} >
      <TelemetryTrackerProvider telemetryTrackerApi={telemetryTrackerApiMock}>
        <App />
      </TelemetryTrackerProvider>
    </StreamServiceProvider>
  );

  const renderApplication = () => {
    return render(<Application />);
  }
});
