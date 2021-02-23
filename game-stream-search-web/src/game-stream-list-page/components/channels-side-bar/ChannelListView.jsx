import React from 'react';
import { shape, string, bool, arrayOf, func } from 'prop-types';
import { makeStyles, styled } from '@material-ui/core/styles';
import Avatar from '@material-ui/core/Avatar';
import Skeleton from '@material-ui/lab/Skeleton';
import Link from '../../../shared-components/Link';

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

const ChannelTile = ({ channelName , platformName, avatarUrl, channelUrl, onChannelOpened }) => {
  const classes = useChannelTileStyles();

  return (
    <Link href={channelUrl} onClick={onChannelOpened} target='_blank'>
      <div className={classes.hover}>
        <div className={classes.channelTile}>
          <Avatar src={avatarUrl} />
          <div className={classes.channelDetails}>
            <ChannelName>{channelName}</ChannelName>
            <StreamPlatformName>{platformName}</StreamPlatformName>
          </div>
        </div>
      </div>
    </Link>
  )
}

ChannelTile.propTypes = {
  channel: shape({
    channelName: string.isRequired,
    platformName: string.isRequired,
    avatarUrl: string.isRequired,
    channelUrl: string.isRequired,
  }),
  onChannelOpened: func,
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

export const ChannelListView = ({ channels, isLoading, onChannelOpened }) => {
  const classes = useChannelListStyles();

  const channelTitles = channels.map((channel, index) => (
    <ChannelTile key={index} {...channel} onChannelOpened={() => onChannelOpened(channel)} />
  ));

  const loadingTiles = [];

  if (isLoading)
  {
    for (let index = 0; index < 3; index++) {
      loadingTiles.push(<LoadingTile key={index} />);
    }
  }

  return (
    <div className={classes.channelList}>
      {channelTitles}
      {loadingTiles}
    </div>
  )
}

ChannelListView.propTypes = {
  channels: arrayOf(shape({
    channelName: string.isRequired,
    platformName: string.isRequired,
    avatarUrl: string.isRequired,
    channelUrl: string.isRequired,
  })),
  isLoading: bool,
  onChannelOpened: func,
};

ChannelListView.defaultProps = {
  channels: [],
  isLoading: false,
};

export default ChannelListView;