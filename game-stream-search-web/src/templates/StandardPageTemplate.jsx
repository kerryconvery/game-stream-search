import React from 'react';
import { node } from 'prop-types';
import { makeStyles } from '@material-ui/core/styles';
import Grid from '@material-ui/core/Grid';

const useStyles = makeStyles(() => ({
  gridLayoutContainer: {
    display: 'grid',
    gridTemplateColumns: 'auto',
    gridTemplateRows: 'auto auto',
    justifyItems: 'center',
  },
  toobar: {
    gridColumnStart: 1,
    gridColumnEnd: 1,
    gridRowStart: 1,
    gridRowEnd: 1,
  },
  content: {
    gridColumnStart: 1,
    gridColumnEnd: 1,
    gridRowStart: 2,
    gridRowEnd: 2,
    backgroundColor: '#F8F9F9',
    width: '100%',
  },
}));

const StandardPageTemplate = ({ toolBar, children }) => {
  const classes = useStyles();

  return (
    <div className={classes.gridLayoutContainer}>
      <div className={classes.toolBar}>
        {toolBar}
      </div>
      <div className={classes.content}>
        {children}
      </div>
    </div>
  )
}

StandardPageTemplate.propTypes = {
  toolBar: node,
  children: node.isRequired,
}

export default StandardPageTemplate;
