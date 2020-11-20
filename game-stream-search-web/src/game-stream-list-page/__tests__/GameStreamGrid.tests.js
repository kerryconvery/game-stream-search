import React from 'react';
import { render, waitFor, screen } from '@testing-library/react';
import GameStreamGrid from '../components/GameStreamGrid';

describe('Game stream grid', () => {
  it('should render stream tiles without loading tiles', async () => {
    const streams = [
      {
        streamTitle: 'test stream A',
        streamerName: 'test streamer',
        streamThumbnailUrl: 'http://test.stream.thumbnail.url',
        streamUrl: 'http://test.stream.url',
        streamerAvatarUrl: 'http://test.channel.thumbnail.url',
        streamPlatformName: 'test platform',
        views: 100
      },
      {
        streamTitle: 'test stream B',
        streamerName: 'test streamer',
        streamThumbnailUrl: 'http://test.stream.thumbnail.url',
        streamUrl: 'http://test.stream.url',
        streamerAvatarUrl: 'http://test.channel.thumbnail.url',
        streamPlatformName: 'test platform',
        views: 10
      },
    ]

    const { container } = render(<GameStreamGrid streams={streams} numberOfLoadingTiles={1} />)

    const streamTiles = await waitFor(() => screen.getAllByText('test streamer'));
    const loadingTiles = screen.queryAllByTestId('loading-tile');
    
    expect(streamTiles.length).toEqual(2);
    expect(loadingTiles.length).toEqual(0);
    expect(container.firstChild).toMatchSnapshot();
  })

  it('should render loading tiles when there are no streams', async () => {
    const { container } = render(<GameStreamGrid isLoading numberOfLoadingTiles={2} />)

    const loadingTiles = await waitFor(() => screen.getAllByTestId('loading-tile'));
    
    expect(loadingTiles.length).toEqual(2);
    expect(container.firstChild).toMatchSnapshot();
  })

  it('should render stream and loading tiles', async () => {
    const streams = [
      {
        streamTitle: 'test stream A',
        streamerName: 'test streamer',
        streamThumbnailUrl: 'http://test.stream.thumbnail.url',
        streamUrl: 'http://test.stream.url',
        streamerAvatarUrl: 'http://test.channel.thumbnail.url',
        streamPlatformName: 'test platform',
        views: 100
      },
    ]

    const { container } = render(<GameStreamGrid streams={streams} isLoading numberOfLoadingTiles={1} />)

    const streamTiles = await waitFor(() => screen.getAllByText('test streamer'));
    const loadingTiles = screen.getAllByTestId('loading-tile');
    
    expect(streamTiles.length).toEqual(1);
    expect(loadingTiles.length).toEqual(1);
  })
})


