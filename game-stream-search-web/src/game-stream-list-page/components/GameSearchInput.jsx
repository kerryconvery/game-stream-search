import React, { useState } from 'react';
import { func } from 'prop-types';
import _has from 'lodash/has';
import _trim from 'lodash/trim';
import { makeStyles } from '@material-ui/core/styles';
import Paper from '@material-ui/core/Paper';
import InputBase from '@material-ui/core/InputBase';
import SearchIcon from '@material-ui/icons/Search';
import IconButton from '@material-ui/core/IconButton';

const useStyles = makeStyles((theme) => ({
  root: {
    padding: '2px 4px',
    display: 'flex',
    alignItems: 'center',
    width: '500px',
    margin: 'auto',
  },
  input: {
    marginLeft: theme.spacing(1),
    flex: 1,
    fontFamily: 'Helvetica'
  },
  iconButton: {
    padding: 1,
  },
}));

const GameStreamInput = ({ onGameChange }) => {
  const [ input, setInput ] = useState('');
  
  const classes = useStyles();

  const onInputChange = (e) => {
    if (_has(e, 'target.value')) {
      setInput(e.target.value);
    }
  }
  const onKeyPress = (e) => {
    if(e.keyCode === 13 && _has(e, 'target.value')) {
      onClick(e.target.value)
    }
  }

  const onClick = (value) => {
    onGameChange(_trim(value) !== '' ? value : null);
  }

  return (
    <Paper className={classes.root} variant='outlined'>
      <InputBase
        className={classes.input}
        variant='outlined'
        placeholder="Search"
        inputProps={{ 'aria-label': 'search' }}
        onChange={onInputChange}
        onKeyDown={onKeyPress}
      />
      <IconButton className={classes.iconButton} aria-label="search" onClick={() => onGameChange(input)}>
        <SearchIcon />
      </IconButton>
    </Paper>
  );
};

GameStreamInput.propTypes = {
  onGameChange: func.isRequired,
}

export default GameStreamInput;
