import React from 'react';
import { number, shape, string, bool, arrayOf } from 'prop-types';
import { makeStyles, styled } from '@material-ui/core/styles';
import Avatar from '@material-ui/core/Avatar';
import Skeleton from '@material-ui/lab/Skeleton';
import Link from '../Link';

const useChannelTileStyles = makeStyles({
  hover: {
    '&:hover > *': {
      backgroundColor: '#E4E3E3',
    }
  },
  channelTile: {
    display: 'flex',
    flexDirection: 'row',
    padding: '0.5rem',
    paddingLeft: '1rem',
  },
  channelDetails: {
    paddingTop: '0.25rem',
    paddingLeft: '0.5rem',
    '& > div': {
      paddingBottom: '0.2rem'
    }
  },
})

const StreamPlatformName = styled('span')({
  color: '#606060',
  fontSize: '14px',
});

const ChannelName = styled('div')({
  fontWeight: 'bold'
})

const ChannelTile = ({ name , streamPlatformDisplayName, channelAvatarUrl, channelUrl }) => {
  const classes = useChannelTileStyles();

  return (
    <Link href={channelUrl} target='_blank'>
      <div className={classes.hover}>
        <div className={classes.channelTile}>
          <Avatar src={channelAvatarUrl} />
          <div className={classes.channelDetails}>
            <ChannelName>{name}</ChannelName>
            <StreamPlatformName>{streamPlatformDisplayName}</StreamPlatformName>
          </div>
        </div>
      </div>
    </Link>
  )
}

ChannelTile.propTypes = {
  channel: shape({
    name: string.isRequired,
    streamPlatformDisplayName: string.isRequired,
    channelAvatarUrl: string.isRequired,
    channelUrl: string.isRequired,
  })
}

const LoadingTile = () => {
  const classes = useChannelTileStyles();

  return (
    <div data-testid='channel-loading-tile'>
      <div className={classes.channelTile}>
        <Skeleton variant='circle' width={50} height={50} animation='wave' />
        <div className={classes.channelDetails}>
          <Skeleton variant='text' width={120} animation='wave' />
          <Skeleton variant='text' width={80} animation='wave' />
        </div>
      </div>
    </div>
  )
}

const useChannelListStyles = makeStyles({
  channelList: {
    display: 'grid',
    gridTemplateColumns: 'auto',
    gridAutoFlow: 'row',
  },
})

export const ChannelList = ({ channels, isLoading, numberOfLoadingTiles }) => {
  const classes = useChannelListStyles();

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
    <div className={classes.channelList}>
      {channelTitles}
      {loadingTiles}
    </div>
    </div>
  )
}

ChannelList.propTypes = {
  channels: arrayOf(shape({
    name: string.isRequired,
    streamPlatformDisplayName: string.isRequired,
    channelAvatarUrl: string.isRequired,
    channelUrl: string.isRequired,
  })),
  isLoading: bool,
  numberOfLoadingTiles: number.isRequired,
};

ChannelList.defaultProps = {
  streams: [],
  isLoading: false,
};

export default ChannelList;