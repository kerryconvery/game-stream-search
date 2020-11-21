import React from 'react';
import { render } from '@testing-library/react';
import ChannelList from '../ChannelList';

describe('Channel List', () => {
  it('should render a list of channels without loading tiles', () => {
    const channels = [
      {
        name: 'test channel 1',
        streamPlatformDisplayName: 'YouTube',
        channelAvatarUrl: '',
        channelUrl: '',
      },
      {
        name: 'test channel 2',
        streamPlatformDisplayName: 'DLive',
        channelAvatarUrl: '',
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