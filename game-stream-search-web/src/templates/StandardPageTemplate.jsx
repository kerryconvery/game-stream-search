import React from 'react';
import { node } from 'prop-types';
import { makeStyles } from '@material-ui/core/styles';

const useStyles = makeStyles(() => ({
  toolBar: {
    position: 'sticky',
    top: 0,
    width: '100%',
    zIndex: 1,
    background: 'white',
    paddingTop: '0.5rem',
    paddingBottom: '0.5rem',
    backgroundColor: 'white',
    borderBottom: '1px outset white',
  },
  content: {
    backgroundColor: '#F8F9F9',
    width: '100%',
  },
}));

const StandardPageTemplate = ({ toolBar, children }) => {
  const classes = useStyles();

  return (
    <>
      <div className={classes.toolBar}>
        {toolBar}
      </div>
      <div className={classes.content}>
        {children}
      </div>
    </>
  )
}

StandardPageTemplate.propTypes = {
  toolBar: node,
  children: node.isRequired,
}

export default StandardPageTemplate;
