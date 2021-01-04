import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import useFeaturedChannelsController from './featured-channels/useFeaturedChannelsController';
import FeaturedChannelsView from './featured-channels/FeaturedChannelsView';
import { telemetryTrackerApi } from '../../../api/telemetryTrackerApi';

const useStyles = makeStyles({
  sidebarContent: {   
    position: 'fixed',
    width: '280px',
    height: 'calc(100vh - 75px)',
    overflow: 'hidden',
    backgroundColor: 'inherited',
    '&:hover': {
      overflowY: 'auto',
    },
  }
});

const ChannelsSideBar = () => {
  const classes = useStyles();
  const featuredChannelsController = useFeaturedChannelsController();
  const { trackFeaturedChannelOpened } = telemetryTrackerApi();

  return (
    <div className={classes.sidebarContent}>
      <FeaturedChannelsView
        {...featuredChannelsController}
        afterChannelOpened={trackFeaturedChannelOpened} 
      />
    </div>
  )
}

export default ChannelsSideBar;

