import React from 'react';
import { render, fireEvent, waitFor, screen, act, getByTestId } from '@testing-library/react';
import nock from 'nock';
import { ConfigurationProvider } from '../providers/ConfigurationProvider';
import App from '../app';
import '@testing-library/jest-dom/extend-expect';

describe('Application', () => {
  it('should render streams without errors', async () => {
    const streams = {
      items: [{
        streamTitle: 'fake stream 1',
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
      .get('/api/streams?pageSize=25')
      .reply(200, streams);

      const { rerender } = render(
        <ConfigurationProvider configuration={{ "streamSearchServiceUrl": "http://localhost:5000/api" }} >
          <App />
        </ConfigurationProvider>
      )

    await waitFor(() => screen.getByText('fake stream 1'));
    
    expect(screen.getByText('fake stream 1')).toBeInTheDocument();
    expect(screen.queryByRole('alert')).not.toBeInTheDocument();
  });

  it('should display the searched for game stream', async () => {
    const streams = {
      items: [{
        streamTitle: 'fake stream 1',
        streamThumbnailUrl: 'http://fake.stream1.thumbnail',
        streamUrl: 'fake.stream1.url',
        streamerName: 'fake steamer',
        streamerAvatarUrl: 'http://fake.channel1.url',
        platformName: 'fake platform',
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
        platformName: 'fake platform',
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
    .get('/api/streams?pageSize=25')
    .reply(200, streams);

    nock('http://localhost:5000')
      .defaultReplyHeaders({
        'access-control-allow-origin': '*',
        'access-control-allow-credentials': 'true' 
      })
      .get('/api/streams?game=testGame&pageSize=25')
      .reply(200, foundStreams);

    const { rerender } = render(
      <ConfigurationProvider configuration={{ "streamSearchServiceUrl": "http://localhost:5000/api" }} >
        <App />
      </ConfigurationProvider>
    )

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

    await waitFor(() => screen.getByText('fake stream 2'));

    expect(screen.queryByText('fake stream 1')).not.toBeInTheDocument();
    expect(screen.getByText('fake stream 2')).toBeInTheDocument();
  });

  it('should display an error alerts when there is an error getting the streams', async () =>{
    nock('http://localhost:5000')
      .defaultReplyHeaders({
        'access-control-allow-origin': '*',
        'access-control-allow-credentials': 'true' 
      })
      .get('/api/streams?pageSize=25')
      .reply(500);

      const { rerender } = render(
        <ConfigurationProvider configuration={{ "streamSearchServiceUrl": "http://localhost:5000/api" }} >
          <App />
        </ConfigurationProvider>
      )

    await waitFor(() => screen.getByRole('alert'));
    
    expect(screen.getByRole('alert')).toBeInTheDocument();
  })
})