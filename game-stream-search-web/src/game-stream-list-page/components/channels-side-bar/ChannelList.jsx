import React from 'react';
import { number, shape, string, bool, arrayOf } from 'prop-types';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemText from '@material-ui/core/ListItemText';

const ChannelTile = ({ name , streamPlatformDisplayName }) => (
  <ListItem>
    <ListItemText
      primary={name}
      secondary={<span>{streamPlatformDisplayName}</span>}
    />
  </ListItem>
)

ChannelTile.propTypes = {
  channel: shape({
    name: string.isRequired,
    streamPlatformDisplayName: string.isRequired,
  })
}

const LoadingTile = () => (
  <div />
)

export const ChannelList = ({ channels, isLoading, numberOfLoadingTiles }) => {
  const channelTitles = channels.map((channel, index) => (
    <ChannelTile key={index} {...channel} />
  ));

  const loadingTiles = [];

  if (isLoading)
  {
    for (let index = 0; index < numberOfLoadingTiles; index++) {
      loadingTiles.push(<LoadingTile key={index} />);
    }
  }

  return (
    <List>
      {channelTitles}
      {loadingTiles}
    </List>
  )
}

ChannelList.propTypes = {
  channels: arrayOf(shape({
    name: string.isRequired,
    streamPlatformDisplayName: string.isRequired,
  })),
  isLoading: bool.isRequired,
  numberOfLoadingTiles: number.isRequired,
};

ChannelList.defaultProps = {
  streams: [],
};

export default ChannelList;