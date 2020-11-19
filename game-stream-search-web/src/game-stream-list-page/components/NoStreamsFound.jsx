import React from 'react';
import { string } from 'prop-types';
import { makeStyles } from '@material-ui/core/styles';

const useStyles = makeStyles(() => ({
  root: {
    textAlign: 'center',
  },
  notFoundMessage: {
    'font-family': 'Helvetica',
    color: '#231E1D',
  }
}));

const NoStreamsFound = ({ searchTerm }) => {
  const classess = useStyles();

  return  (
    <div className={classess.root} data-testid='streams-not-found'>
      <h3 className={classess.notFoundMessage}>{`No streams were found for ${searchTerm}`}</h3>
    </div>
  )
}

NoStreamsFound.propTypes = {
  searchTerm: string,
}

NoStreamsFound.defaultProps = {
  searchTerm: '',
}

export default NoStreamsFound;