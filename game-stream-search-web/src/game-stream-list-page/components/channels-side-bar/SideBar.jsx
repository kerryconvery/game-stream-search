import React from 'react'
import { node } from 'prop-types';
import { makeStyles } from '@material-ui/core/styles';

const useStyles = makeStyles(() => ({
  root: {
    'font-family': 'Helvetica',
    backgroundColor: '#bfbfbf',
    height: '100%',
  },
}));

const SideBarPanel = ({ children }) => {
  const classes = useStyles();

  return <div className={classes.root}>{children}</div>
}

const SideBar = ({ children }) => <div>{children}</div>

SideBarPanel.propTypes = {
  children: node.isRequired,
}

SideBar.propTypes = {
  children: node.isRequired,
}

export {
  SideBarPanel,
  SideBar,
}