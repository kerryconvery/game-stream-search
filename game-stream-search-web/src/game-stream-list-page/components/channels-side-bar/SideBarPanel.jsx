import React from 'react'
import { node, string } from 'prop-types';
import { makeStyles } from '@material-ui/core/styles';

const useStyles = makeStyles(() => ({
  list: {
    fontFamily: 'Helvetica',
    backgroundColor: 'white',
    height: 'auto',
  },
  titleBar: {
    display: 'flex',
    flexDirection: 'row',
    alignItems: 'center',
  },
  title: {
    paddingLeft: '1rem',
    fontWeight: 'bold',
    marginTop: '0.1rem'
  },
  action: {
    marginLeft: '0.5rem',
  }
}));

const SideBarPanel = ({ children, title, action }) => {
  const classes = useStyles();

  return (
    <div className={classes.list}>
      <div className={classes.titleBar}>
        <span className={classes.title}>
          {title.toUpperCase()}
        </span>
        <div className={classes.action}>
          {action}
        </div>
      </div>
      {children}
    </div>
  )
}

SideBarPanel.propTypes = {
  title: string.isRequired,
  action: node,
  children: node.isRequired,
}

export default SideBarPanel;