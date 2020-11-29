import React from 'react';
import { render, fireEvent, waitFor, screen } from '@testing-library/react';
import nock from 'nock';
import { ConfigurationProvider } from '../../providers/ConfigurationProvider';
import App from '../../app';
import '@testing-library/jest-dom/extend-expect';

describe('Game search page list', () => {
  beforeEach(() => {
    nock('http://localhost:5000')
    .defaultReplyHeaders({
      'access-control-allow-origin': '*',
      'access-control-allow-credentials': 'true' ,
    })
    .get('/api/channels')
    .reply(200, { items: [] });
  })
  
  const renderApplication = () => {
    return render(
      <ConfigurationProvider configuration={{ "streamSearchServiceUrl": "http://localhost:5000/api" }} >
        <App />
      </ConfigurationProvider>
    )
  }

  it('should render streams without errors', async () => {
    const streams = {
      items: [{
        streamTitle: 'fake stream',
        streamThumbnailUrl: 'http://fake.stream1.thumbnail',
        streamUrl: 'fake.stream1.url',
        streamerName: 'fake steamer',
        streamerAvatarUrl: 'http://fake.channel1.url',
        streamPlatformName: 'fake platform',
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
      .reply(200, streams);

    renderApplication();

    const fakeStream = await waitFor(() => screen.getByText('fake stream'));
    const loadingTiles = await waitFor(() => screen.queryByTestId('stream-loading-tile'));
    
    expect(fakeStream).toBeInTheDocument();
    expect(screen.queryByRole('alert')).not.toBeInTheDocument();
    expect(screen.queryByTestId('streams-not-found')).not.toBeInTheDocument();
    expect(loadingTiles).not.toBeInTheDocument();
  });

  it('should render loading tiles while loading streams', async () => {
    const streams = {
      items: [{
        streamTitle: 'fake stream',
        streamThumbnailUrl: 'http://fake.stream1.thumbnail',
        streamUrl: 'fake.stream1.url',
        streamerName: 'fake steamer',
        streamerAvatarUrl: 'http://fake.channel1.url',
        streamPlatformName: 'fake platform',
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
      .reply(200, streams);

    renderApplication();

    const loadingTiles = await waitFor(() => screen.getAllByTestId('stream-loading-tile'));
    
    expect(loadingTiles[0]).toBeInTheDocument();

    //Wait until screen has finished render to avoid unmount error
    await waitFor(() => screen.getAllByText('fake stream'));
  })

  it('should display the searched for game stream', async () => {
    const streams = {
      items: [{
        streamTitle: 'fake stream 1',
        streamThumbnailUrl: 'http://fake.stream1.thumbnail',
        streamUrl: 'fake.stream1.url',
        streamerName: 'fake steamer',
        streamerAvatarUrl: 'http://fake.channel1.url',
        streamPlatformName: 'fake platform',
        isLive: true,
        views: 100
      }],
      nextPageToken: null,
    }

    const foundStreams = {
      items: [{
        streamTitle: 'fake stream 2',
        streamThumbnailUrl: 'http://fake.stream2.thumbnail',
        streamUrl: 'fake.stream2.url',
        streamerName: 'fake steamer',
        streamerAvatarUrl: 'http://fake.channel1.url',
        streamPlatformName: 'fake platform',
        isLive: true,
        views: 100
      }],
      nextPageToken: null,
    }

    nock('http://localhost:5000')
    .defaultReplyHeaders({
      'access-control-allow-origin': '*',
      'access-control-allow-credentials': 'true' 
    })
    .get('/api/streams?pageSize=10')
    .reply(200, streams);

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
    
    rerender(      
      <ConfigurationProvider configuration={{ "streamSearchServiceUrl": "http://localhost:5000/api" }} >
        <App />
      </ConfigurationProvider>
    );

    const fakeStream2 = await waitFor(() => screen.getByText('fake stream 2'));

    expect(fakeStream2).toBeInTheDocument();
    expect(screen.queryByText('fake stream 1')).not.toBeInTheDocument();
  });

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

  it('should display a streams not found message when there are no streams matching the search criteria', async () => {
    nock('http://localhost:5000')
      .defaultReplyHeaders({
        'access-control-allow-origin': '*',
        'access-control-allow-credentials': 'true' 
      })
      .get('/api/streams?pageSize=10')
      .reply(200, { items: [] });

    renderApplication();

    const noStreamsFound = await waitFor(() => screen.getByTestId('streams-not-found'));
    
    expect(noStreamsFound).toBeInTheDocument();
  });
});
