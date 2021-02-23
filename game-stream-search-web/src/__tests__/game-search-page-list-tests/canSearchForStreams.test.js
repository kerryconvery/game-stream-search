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

describe('Can search for streams', () => {
  it('should display the searched for game stream and trigger the stream searched telemetry event', async () => {
    const initialStreams = {
      streams: [{
        streamTitle: 'fake stream 1',
        streamThumbnailUrl: 'http://fake.stream1.thumbnail',
        streamUrl: 'fake.stream1.url',
        streamerName: 'fake steamer',
        streamerAvatarUrl: 'http://fake.channel1.url',
        platformName: 'fake platform',
        isLive: true,
        views: 100
      }],
      nextPageToken: "",
    }

    const foundStreams = {
      streams: [{
        streamTitle: 'fake stream 2',
        streamThumbnailUrl: 'http://fake.stream2.thumbnail',
        streamUrl: 'fake.stream2.url',
        streamerName: 'fake steamer',
        streamerAvatarUrl: 'http://fake.channel1.url',
        platformName: 'fake platform',
        isLive: true,
        views: 100
      }],
      nextPageToken: "",
    }

    nock('http://localhost:5000')
    .defaultReplyHeaders({
      'access-control-allow-origin': '*',
      'access-control-allow-credentials': 'true' 
    })
    .get('/api/streams?pageSize=10')
    .reply(200, initialStreams);

    nock('http://localhost:5000')
      .defaultReplyHeaders({
        'access-control-allow-origin': '*',
        'access-control-allow-credentials': 'true' 
      })
      .get('/api/streams?game=testGame&pageSize=10')
      .reply(200, foundStreams);

    const { rerender } = renderApplication();

    await waitFor(() => screen.getByText('fake stream 1'));

    const searchInput = screen.getByPlaceholderText('Search');
    const searchButton = screen.getByRole('button', { name: 'search' });

    fireEvent.change(searchInput, { target: { value: 'testGame' } });
    fireEvent.click(searchButton, { button: 1 })
    
    rerender(<Application />);

    const fakeStream2 = await waitFor(() => screen.getByText('fake stream 2'));

    expect(fakeStream2).toBeInTheDocument();
    expect(screen.queryByText('fake stream 1')).not.toBeInTheDocument();
    expect(telemetryTrackerApiMock.trackStreamSearch).toHaveBeenCalled();
  });

  it('should display a streams not found message when there are no streams matching the search criteria', async () => {
    nock('http://localhost:5000')
      .defaultReplyHeaders({
        'access-control-allow-origin': '*',
        'access-control-allow-credentials': 'true' 
      })
      .get('/api/streams?pageSize=10')
      .reply(200, { streams: [] });

    renderApplication();

    const noStreamsFound = await waitFor(() => screen.getByTestId('streams-not-found'));
    
    expect(noStreamsFound).toBeInTheDocument();
  });

  beforeEach(() => {
    nock('http://localhost:5000')
    .defaultReplyHeaders({
      'access-control-allow-origin': '*',
      'access-control-allow-credentials': 'true' ,
    })
    .get('/api/channels')
    .reply(200, { items: [] });
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
