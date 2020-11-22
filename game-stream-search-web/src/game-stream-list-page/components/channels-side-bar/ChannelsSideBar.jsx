import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import RisingChannels from './RisingChannels';

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
      <RisingChannels />
    </div>
  )
}

export default ChannelsSideBar;

