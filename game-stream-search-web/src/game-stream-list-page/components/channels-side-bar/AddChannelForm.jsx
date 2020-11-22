import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import TextField from '@material-ui/core/TextField';
import Select from '@material-ui/core/Select';
import MenuItem from '@material-ui/core/MenuItem';
import InputLabel from '@material-ui/core/InputLabel';
import FormControl from '@material-ui/core/FormControl';
import FormGroup from '@material-ui/core/FormGroup';
import Card from '@material-ui/core/Card';
import CardHeader from '@material-ui/core/CardHeader';
import CardActions from '@material-ui/core/CardActions';
import CardContent from '@material-ui/core/CardContent';
import Button from '@material-ui/core/Button';

const useStyles = makeStyles({
  card: {
    '& *': {
      fontFamily: 'Helvetica',
    }
  },
  title: {
    paddingBottom: '0.5rem',
  },
  formContent: {
    paddingTop: 0,
  }
});

const AddChannelForm = () => {
  const classes = useStyles();

  return (
    <Card className={classes.card}>
      <CardHeader className={classes.title} title='Add Channel'/>
      <CardContent className={classes.formContent}>
        <FormGroup>
            <TextField label='Channel name' autoFocus />
          <FormControl margin='normal'>
            <InputLabel>Stream platform</InputLabel>
            <Select value='twitch'>
              <MenuItem value='twitch'>Twitch</MenuItem>
              <MenuItem value='youtube'>YouTube</MenuItem>
              <MenuItem value='dlive'>DLive</MenuItem>
            </Select>
          </FormControl>
        </FormGroup>
      </CardContent>
      <CardActions>
        <Button>Save</Button>
        <Button>Cancel</Button>
      </CardActions>
    </Card>
  )
}

export default AddChannelForm;