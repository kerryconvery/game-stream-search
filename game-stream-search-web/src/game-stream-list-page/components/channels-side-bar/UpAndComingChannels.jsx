import React from 'react';
import IconButton from '@material-ui/core/IconButton';
import Add from '@material-ui/icons/Add';
import Tooltip from '@material-ui/core/Tooltip';
import ChannelList from './ChannelList';
import SideBarPanel from './SideBarPanel';
import { useGameStreamApi } from '../../../api/gameStreamApi';
import useChannelsLoader from '../../hooks/useChannelsLoader';

const UpAndComingChannels = () => {
  const { getStreamChannels } = useGameStreamApi();
  const { channels, isLoading } = useChannelsLoader(getStreamChannels, () => {});

  return (
    <SideBarPanel
      title='Rising channels'
      action={(
        <Tooltip title='Add a new hannel to the list'>
          <IconButton color='primary' size='small' >
            <Add />
          </IconButton>
        </Tooltip>
      )}
    >
      <ChannelList
        channels={channels}
        isLoading={isLoading}
        numberOfLoadingTiles={3}
      />
    </SideBarPanel>
  )
}

export default UpAndComingChannels;