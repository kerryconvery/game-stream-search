import React from 'react';
import { node, bool, number } from 'prop-types';
import { makeStyles } from '@material-ui/core/styles';
import Grid from '@material-ui/core/Grid';
import StandardPageTemplate from '../templates/StandardPageTemplate';

const useStyles = makeStyles(() => ({
  root: {
    display: 'flex',
    flexWrap: 'nowrap',
    flexDirection: 'row',
  },
  toobar: {
    margin: 0,
    padding: 0,
  },
  content: {
    backgroundColor: '#F8F9F9',
  },
  gridContainer: {
    display: 'grid',
    gridTemplateColumns: '280px auto',
    gridTemplateRows: 'auto',
    gap: '0 25px'
  },
  sideBar: {
    gridColumnStart: 1,
    gridColumnEnd: 1,
    gridRowStart: 1,
    gridRowEnd: 1,
  },
  mainContent: {
    gridColumnStart: 2,
    gridColumnEnd: 2,
    gridRowStart: 1,
    gridRowEnd: 1,
    paddingTop: '20px'
  },
}));

const GameStreamPageTemplate = ({
  children,
  searchBar,
  leftSideBar,
  notFoundNotice,
  numberOfStreams,
  isLoadingStreams
}) => {
  const classes = useStyles();
  const hasStreams = numberOfStreams > 0 || isLoadingStreams;

  return (
    <StandardPageTemplate toolBar={searchBar} >
      {/* <Grid className={classes.root} direction="row" container justify="center" > */}
        <div className={classes.gridContainer}>
          <div className={classes.sideBar}>
            {leftSideBar}
          </div>
          <div className={classes.mainContent}>
            {!hasStreams && notFoundNotice}
            {hasStreams && children}
          </div>
        </div>
      {/* </Grid> */}
    </StandardPageTemplate>
  )
}

GameStreamPageTemplate.propTypes = {
  children: node.isRequired,
  searchBar: node.isRequired,
  leftSideBar: node.isRequired,
  notFoundNotice: node.isRequired,
  numberOfStreams: number.isRequired,
  isLoadingStreams: bool.isRequired,
}

export default GameStreamPageTemplate;