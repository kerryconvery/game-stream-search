import React from 'react';
import { render, screen } from '@testing-library/react';
import InfiniteGameStreamGrid from '../components/InfiniteGameStreamGrid';
import '@testing-library/jest-dom/extend-expect';

describe('Game string list', () => {
  it('should render loading tiles when the streams are being fetched', () => {
    render(<InfiniteGameStreamGrid isLoadingStreams={true} hasMoreStreams={false} loadMoreStreams={jest.fn()} />);

    const loadingTitles = screen.getAllByTestId('loading-tile');

    expect(loadingTitles[0]).toBeInTheDocument();
  });

  it('should render streams after the steams have been fetched and no loading tiles', () => {
    const streams = [
      {
          streamTitle: 'test stream',
          streamerName: 'test streamer',
          streamThumbnailUrl: 'http://test.stream.thumbnail.url',
          streamUrl: 'http://test.stream.url',
          streamerAvatarUrl: 'http://test.channel.thumbnail.url',
          streamPlatformName: 'test platform',
          isLive: true,
          views: 100
      }
    ]

    render(
      <InfiniteGameStreamGrid
        streams={streams}
        isLoadingStreams={false}
        hasMoreStreams={false}
        loadMoreStreams={jest.fn()}
      />
    );

    expect(screen.getByText('test stream')).toBeInTheDocument();
    expect(screen.queryByTestId('loading-tile')).not.toBeInTheDocument();
  });
});