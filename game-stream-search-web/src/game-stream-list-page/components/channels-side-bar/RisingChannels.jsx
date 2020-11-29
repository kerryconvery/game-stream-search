import React from 'react';
import IconButton from '@material-ui/core/IconButton';
import Add from '@material-ui/icons/Add';
import Tooltip from '@material-ui/core/Tooltip';
import Popover from '@material-ui/core/Popover';
import ChannelList from './ChannelList';
import SideBarPanel from './SideBarPanel';
import AddChannelForm from './add-channel-form/AddChannelForm';
import { useGameStreamApi } from '../../../api/gameStreamApi';
import useChannelsLoader from '../../hooks/useChannelsLoader';

const RisingChannels = () => {
  const { getChannels } = useGameStreamApi();
  const { channels, isLoading, appendChannel } = useChannelsLoader(getChannels, () => {});
  const [ anchorEl, setAnchorEl ] = React.useState(null);

  const addButtonClick = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const handleChannelAdded = (channel) => {
    setAnchorEl(null);
    appendChannel(channel);
  }

  const open = Boolean(anchorEl);

  return (
    <SideBarPanel
      title='Rising channels'
      action={(
        <Tooltip title='Add a new channel to the list'>
          <IconButton color='primary' size='small' onClick={addButtonClick}>
            <Add />
          </IconButton>
        </Tooltip>
      )}
    >
      <Popover
        open={open}
        anchorEl={anchorEl}
        onClose={handleClose}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'center',
        }}
        transformOrigin={{
          vertical: 'top',
          horizontal: 'center',
        }}
      >
        <AddChannelForm onCancel={handleClose} afterChannelAdded={handleChannelAdded} />
      </Popover>
      <ChannelList
        channels={channels}
        isLoading={isLoading}
        numberOfLoadingTiles={3}
      />
    </SideBarPanel>
  )
}

export default RisingChannels;