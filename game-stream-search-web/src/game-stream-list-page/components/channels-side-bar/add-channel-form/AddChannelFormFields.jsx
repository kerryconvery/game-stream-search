import React from 'react';
import { string, func, shape } from 'prop-types';
import _trim from 'lodash/trim';
import _get from 'lodash/get';
import _isNil from 'lodash/isNil';
import TextField from '@material-ui/core/TextField';
import Select from '@material-ui/core/Select';
import MenuItem from '@material-ui/core/MenuItem';
import InputLabel from '@material-ui/core/InputLabel';
import FormControl from '@material-ui/core/FormControl';
import FormGroup from '@material-ui/core/FormGroup';

const AddChannelFormFields = ({ formValues, errors, onChange }) => {
  const onFormChange = field => (event) => {
    onChange({
      ...formValues,
      [field]: event.target.value,
    })
  }

  return (
    <FormGroup>
      <TextField
        id='channel-name'
        label='Channel name'
        defaultValue={_get(formValues, 'channelName')}
        autoFocus
        onChange={onFormChange('channelName')}
        helperText={_get(errors, 'channelName')}
        error={!_isNil(_get(errors, 'channelName'))}
      />
      <FormControl margin='normal'>
        <InputLabel>Streaming platform</InputLabel>
        <Select value={_get(formValues, 'streamPlatform')} onChange={onFormChange('streamPlatform')}>
          <MenuItem value='twitch'>Twitch</MenuItem>
          <MenuItem value='youtube'>YouTube</MenuItem>
          <MenuItem value='dlive'>DLive</MenuItem>
        </Select>
      </FormControl>
    </FormGroup>
  )
}

AddChannelFormFields.propTypes = {
  formValues: shape({
    channelName: string,
    streamPlatform: string,
  }),
  errors: shape({
    channelName: string,
  }),
  onChange: func.isRequired,
}

export const validateForm = ({ channelName }) => {
  const errors = {};

  if (_trim(channelName) === '')
  {
    errors['channelName'] = 'Please enter a channel name';
  }

  return errors;
}

export default AddChannelFormFields;