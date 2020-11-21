import React from 'react';
import ChannelList from './ChannelList';
import SideBarPanel from './SideBarPanel';
import { useGameStreamApi } from '../../../api/gameStreamApi';
import useChannelsLoader from '../../hooks/useChannelsLoader';

const UpAndComingChannels = () => {
  const { getStreamChannels } = useGameStreamApi();
  const { channels, isLoading } = useChannelsLoader(getStreamChannels, () => {});

  return (
    <SideBarPanel title='Up and coming channels'>
      <ChannelList
        channels={channels}
        isLoading={isLoading}
        numberOfLoadingTiles={5}
      />
    </SideBarPanel>
  )
}

export default UpAndComingChannels;