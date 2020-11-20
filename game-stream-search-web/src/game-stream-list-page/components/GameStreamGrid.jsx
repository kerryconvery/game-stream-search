import React from 'react';
import { shape, string, number, bool, arrayOf } from 'prop-types';
import { makeStyles } from '@material-ui/core/styles';
import Skeleton from '@material-ui/lab/Skeleton';
import IconButton from '@material-ui/core/IconButton';
import PlayCircleOutlineIcon from '@material-ui/icons/PlayCircleOutline';
import Avatar from '@material-ui/core/Avatar';
import Tooltip from '@material-ui/core/Tooltip';

const useStreamTileStyles = makeStyles(() => ({
  root: {
    'font-family': 'Helvetica',
    color: '#231E1D',
    width:'20rem',
    height:'18rem',
    position: 'relative',
    paddingLeft: '10px',
    paddingRight: '10px',
  },
  streamTitle: {
    display: '-webkit-box',
    '-webkit-box-orient': 'vertical',
    '-webkit-line-clamp': '2',
    fontSize: '16px',
    textOverflow: 'ellipsis',
    overflow: 'hidden',
    whiteSpace: 'normal',
    marginTop: 0,
    marginBottom: '0.25rem',
  },
  link: {
    textDecoration: 'none',
    color: 'inherit',
    '&:hover > button': {
      opacity: 0.7
    },
  },
  detailsContainer: {
    display: 'flex',
    flexDirection: 'row',
    paddingTop: '0.5rem',
  },
  streamDetails: {
    paddingLeft: '0.5rem',
  },
  streamSubDetails: {
    color: '#606060',
    fontSize: '14px',
  },
  playButton: {
    color: 'white',
    opacity: 0,
    position: 'absolute',
    left: 0, 
    right: 0,
    top: '10%',
    marginLeft: 'auto',
    marginRight: 'auto', 
    width: '100px', /* Need a specific value to work */
    transition: 'opacity 0.5s ease',
  },
  playButtonIcon: {
    fontSize: 100,
    color: 'white',
  },
}));


const StreamTile = ({
  streamTitle,
  streamThumbnailUrl,
  streamUrl,
  streamerName,
  streamerAvatarUrl,
  streamPlatformName,
  views }) => {

  const classes = useStreamTileStyles();

  return (
    <div className={classes.root}>
      <a href={streamUrl} target='_blank' className={classes.link}>
        <IconButton size="medium" className={classes.playButton}>
          <PlayCircleOutlineIcon className={classes.playButtonIcon} />
        </IconButton >
        <img src={streamThumbnailUrl} width={320} height={180} />
        <div className={classes.detailsContainer}>
          <Avatar src={streamerAvatarUrl} />
          <div className={classes.streamDetails}>
            <Tooltip title={streamTitle}>
              <h3 className={classes.streamTitle}>{streamTitle}</h3>
            </Tooltip>
            <div className={classes.streamSubDetails}>
              <div>{streamerName}</div>
              <span>{streamPlatformName}</span>
              <span> • </span>
              <span>{`${views} viewers`}</span>
            </div>
          </div>
        </div>
      </a>
    </div>
  )
}

StreamTile.propTypes = {
  streamTitle: string.isRequired,
  streamerName: string.isRequired,
  streamThumbnailUrl: string.isRequired,
  streamUrl: string.isRequired,
  streamerAvatarUrl: string.isRequired,
  streamPlatformName: string.isRequired,
  views: number.isRequired,
}

const useLoadingTileStyles = makeStyles(() => ({
  root: {
    width:'20rem',
    height:'19rem',
    paddingLeft: '10px',
    paddingRight: '10px',
  },
  detailsContainer: {
    display: 'flex',
    flexDirection: 'row',
    paddingTop: '0.5rem',
    width: '100%',
  },
  details: {
    paddingLeft: '0.5rem',
    width: '80%',
    height: '80%',
  },
  subDetails: {
    width: '50%',
  },
}));

const LoadingTile = () => {
  const classes = useLoadingTileStyles();

  return (
    <div data-testid='loading-tile' className={classes.root}>
      <Skeleton variant='rect' height='60%' animation='wave' />
      <div className={classes.detailsContainer}>
        <Skeleton variant='circle' width={50} height={50} animation='wave' />
        <div className={classes.details}>
          <Skeleton variant='text' animation='wave' />
          <div className={classes.subDetails}>
            <Skeleton variant='text' animation='wave' />
            <Skeleton variant='text' animation='wave' />
          </div>
        </div>
      </div>
    </div>
  )
}

const useGridTileStyles = makeStyles(() => ({
  gridLayout: {
    display: 'flex',
    flexWrap: 'wrap',
    flexDirection: 'row',
    justifyContent: 'flex-start',
    overflow: 'hidden',
    width: '100%'
  },
}));

const GameStreamGrid = ({ streams, isLoading, numberOfLoadingTiles }) => {
  const classes = useGridTileStyles();

  const streamTitle = streams.map((stream, index) => <StreamTile key={index} {...stream} />);

  const loadingTiles = [];

  if (isLoading) {
    for (let index = 0; index < numberOfLoadingTiles; index++) {
      loadingTiles.push(
        <LoadingTile key={index} />
      )
    }
  }

  return (
    <div className={classes.gridLayout}>
      {streamTitle}
      {loadingTiles}
    </div>
  )
}

GameStreamGrid.propTypes = {
  streams: arrayOf(shape({
    streamTitle: string.isRequired,
    streamerName: string.isRequired,
    streamThumbnailUrl: string.isRequired,
    streamUrl: string.isRequired,
    streamerAvatarUrl: string.isRequired,
    streamPlatformName: string.isRequired,
    views: number.isRequired,
  })),
  isLoading: bool,
  numberOfLoadingTiles: number.isRequired,
}

GameStreamGrid.defaultProps = {
  streams: [],
  isLoading: false,
}

export default GameStreamGrid;
