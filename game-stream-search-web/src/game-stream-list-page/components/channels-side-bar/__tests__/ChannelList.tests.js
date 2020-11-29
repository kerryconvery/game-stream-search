import React from 'react';
import { render } from '@testing-library/react';
import ChannelList from '../ChannelList';

describe('Channel List', () => {
  it('should render a list of channels without loading tiles', () => {
    const channels = [
      {
        channelName: 'test channel 1',
        streamPlatform: 'YouTube',
        avatarUrl: '',
        channelUrl: '',
      },
      {
        channelName: 'test channel 2',
        streamPlatform: 'DLive',
        avatarUrl: '',
        channelUrl: '',
      }
    ];

    const { container } = render(<ChannelList channels={channels} numberOfLoadingTiles={2} />);

    expect(container.firstChild).toMatchSnapshot();
  });

  it('should render a list of loading tiles', () => {
    const { container } = render(<ChannelList channels={[]} isLoading numberOfLoadingTiles={2} />);

    expect(container.firstChild).toMatchSnapshot();
  })
})