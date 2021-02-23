import React from 'react';
import { render } from '@testing-library/react';
import ChannelListView from '../ChannelListView';

describe('Channel List', () => {
  it('should render a list of channels without loading tiles', () => {
    const channels = [
      {
        channelName: 'test channel 1',
        platformName: 'YouTube',
        avatarUrl: '',
        channelUrl: '',
      },
      {
        channelName: 'test channel 2',
        platformName: 'DLive',
        avatarUrl: '',
        channelUrl: '',
      }
    ];

    const { container } = render(<ChannelListView channels={channels} />);

    expect(container.firstChild).toMatchSnapshot();
  });

  it('should render a list of loading tiles', () => {
    const { container } = render(<ChannelListView channels={[]} isLoading />);

    expect(container.firstChild).toMatchSnapshot();
  })
})