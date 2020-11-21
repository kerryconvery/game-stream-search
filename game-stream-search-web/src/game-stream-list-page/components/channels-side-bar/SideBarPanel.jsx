import React from 'react'
import { node, string } from 'prop-types';
import { makeStyles, styled } from '@material-ui/core/styles';

const useStyles = makeStyles(() => ({
  list: {
    fontFamily: 'Helvetica',
    backgroundColor: 'white',
    height: 'auto',
  },
}));

const Title = styled('span')({
  paddingLeft: '1rem',
  fontWeight: 'bold',
});

const SideBarPanel = ({ children, title }) => {
  const classes = useStyles();

  return (
    <div className={classes.list}>
      <Title>{title.toUpperCase()}</Title>
      {children}
    </div>
  )
}

SideBarPanel.propTypes = {
  title: string.isRequired,
  children: node.isRequired,
}

export default SideBarPanel;