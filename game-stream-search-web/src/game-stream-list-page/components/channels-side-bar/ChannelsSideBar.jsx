import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import UpAndComingChannels from './UpAndComingChannels';

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

  return (
    <div className={classes.sidebarContent}>
    <UpAndComingChannels />
    </div>
  )
}

export default ChannelsSideBar;

