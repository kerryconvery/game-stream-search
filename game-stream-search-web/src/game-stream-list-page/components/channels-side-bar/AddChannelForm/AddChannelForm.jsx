import React, { useReducer } from 'react';
import { func } from 'prop-types';
import _isEmpty from 'lodash/isEmpty';
import Button from '@material-ui/core/Button';
import FormTemplate, { SubmitButton } from '../../../../templates/FormTemplate';
import AddChannelFormFields, { validateForm } from './AddChannelFormFields';
import { useGameStreamApi } from '../../../../api/gameStreamApi';

const reducer = (state, action) => {
  switch (action.type) {
    case 'FIELD_CHANGED': {
      return {
        ...state,
        formValues: action.formValues,
      }
    }
    case 'ERRORS': {
      return {
        ...state,
        errors: action.errors
      }
    }
    case 'SAVING': {
      return {
        ...state,
        submitted: true,
        isSaving: true,
      }
    }
    case 'SAVE_FAILED': {
      return {
        ...state,
        errors: action.errors,
        isSaving: false,
      }
    }
    case 'SAVE_SUCCESS': {
      return {
        ...state,
        errors: action.errors,
        isSaving: false,
      }
    }
  }
}

const initialState = {
  formValues: { streamingPlatform: 'twitch' },
  errors: {},
  isSaving: false,
  submitted: false,
}

const AddChannelForm = ({ onCancel }) => {
  const [ state, dispatch ] = useReducer(reducer, initialState)
  const { registerChannel } = useGameStreamApi();

  const onSave = async () => {
    dispatch({ type: 'SAVING' });

    const errors = validateForm(state.formValues);

    if (_isEmpty(errors)) {
      const result = await registerChannel(state.formValues);

      if (result.errors) {
        dispatch({ type: 'SAVE_FAILED', errors: result.errors });
      } else {
        dispatch({ type: 'SAVE_SUCCESS' })
      }
    } else {
      dispatch({ type: 'SAVE_FAILED', errors })
    }
  };

  const onChange = values => {
    dispatch({ type: 'FIELD_CHANGED', formValues: values });

    if (state.submitted) {
      dispatch({ type: 'ERRORS', errors: validateForm(values) });
    }
  }

  return (
    <FormTemplate
      title='Add Channel'
      content={
        <AddChannelFormFields
          formValues={state.formValues}
          errors={state.errors}
          onChange={onChange} 
        />
      }
    >
      <SubmitButton submitting={state.isSaving} onClick={onSave}>Save</SubmitButton>
      <Button onClick={onCancel}>Cancel</Button>
    </FormTemplate>
  )
}

AddChannelForm.propTypes = {
  onCancel: func.isRequired,
}

export default AddChannelForm;