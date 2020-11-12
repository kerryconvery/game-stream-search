import React from 'react';
import { render } from '@testing-library/react';
import { getStreamTiles, getLoadingTiles } from '../components/GameStreamGrid';

describe('Game stream grid tile', () => {
  it('should match the snapshot', () => {
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

    const tiles = getStreamTiles(streams);
    const { container } = render(<div>{tiles}</div>);

    expect(tiles.length).toEqual(2);
    expect(container.firstChild).toMatchSnapshot();
  })
})


describe('Loading grid tile', () => {
  it('should match the snapshot', () => {
    const tiles = getLoadingTiles(2);
    const { container } = render(<div>{tiles}</div>);

    expect(tiles.length).toEqual(2);
    expect(container.firstChild).toMatchSnapshot();
  })
})


