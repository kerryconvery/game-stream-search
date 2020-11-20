import React from 'react';
import { number, shape, string, bool, arrayOf } from 'prop-types';
import { makeStyles } from '@material-ui/core/styles';

const useTileStyles = makeStyles({
  container: {
    display: 'grid',
    gridTemplateColumns: 'auto',
    gridAutoFlow: 'row',
  },
})

const ChannelTile = ({ name , streamPlatformDisplayName }) => (
  <>
    <h3>{name}</h3>
    <span>{streamPlatformDisplayName}</span>
  </>
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
  const classes = useTileStyles();

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
    <div>
    <div className={classes.container}>
      {channelTitles}
      {/* {loadingTiles} */}
    </div>
    </div>
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