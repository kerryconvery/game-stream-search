import React from 'react';
import { node, string, number } from 'prop-types';
import { makeStyles } from '@material-ui/core/styles';
import GridList from '@material-ui/core/GridList';
import GridListTile from '@material-ui/core/GridListTile';
import Skeleton from '@material-ui/lab/Skeleton';
import IconButton from '@material-ui/core/IconButton';
import PlayCircleOutlineIcon from '@material-ui/icons/PlayCircleOutline';
import Avatar from '@material-ui/core/Avatar';
import Tooltip from '@material-ui/core/Tooltip';

const useStreamTileStyles = makeStyles(() => ({
  root: {
    'font-family': 'Helvetica',
    color: '#231E1D',
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
    }
  },
  detailsContainer: {
    display: 'flex',
    flexDirection: 'row',
    paddingTop: '1rem',
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
  detailsContainer: {
    display: 'flex',
    flexDirection: 'row',
    paddingTop: '1rem',
    width: '100%',
    height: '100%',
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
    <>
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
    </>
  )
}

export const getLoadingTiles = (size) => {
  const loadingTiles = [];

  for (let index = 0; index < size; index++) {
    loadingTiles.push(
      <GridListTile key={index} data-testid='loading-tile'>
        <LoadingTile />
      </GridListTile>
    )
  }

  return loadingTiles;
}

export const getStreamTiles = (streams) => (
  streams.map((stream, index) => (
    <GridListTile key={index}>  
      <StreamTile {...stream} />
    </GridListTile>
  )
))

const GameStreamGrid = ({ children }) => (
  <GridList cols={4} cellHeight={300} spacing={20}>
    {children}
  </GridList>
)

GameStreamGrid.propTypes = {
  children: node.isRequired,
}

export default GameStreamGrid;
