import { useReducer } from 'react';
import _isEmpty from 'lodash/isEmpty';

const reducer = (state, action) => {
  switch (action.type) {
    case 'FIELD_CHANGED': return { ...state, formValues: action.formValues, errors: action.errors }
    case 'SAVING': return { ...state, submitted: true, isSaving: true }
    case 'SAVE_FAILED': return { ...state, errors: action.errors, isSaving: false }
    case 'SAVE_SUCCESS': return { ...state, errors: action.errors, isSaving: false }
  }
}

const initialState = {
  isSaving: false,
  submitted: false,
}

const useFormController = (onValidateForm, onSaveForm, onSendSuccessNotification, onSaveSuccess) => {
  const [ state, dispatch ] = useReducer(reducer, initialState)

  const onSave = async (formValues) => {
    dispatch({ type: 'SAVING' });

    const errors = onValidateForm(formValues);

    if (!_isEmpty(errors)) {
      return dispatch({ type: 'SAVE_FAILED', errors });
    }

    const result = await onSaveForm(formValues);
    
    if (!result.success) {
      return dispatch({ type: 'SAVE_FAILED', errors: result.errors });
    }

    onSendSuccessNotification(result, formValues);

    onSaveSuccess(result, formValues);

    dispatch({ type: 'SAVE_SUCCESS' });
  };

  const onChange = (formValues) => {
    if (state.submitted) {
      dispatch({ type: 'FIELD_CHANGED', formValues, errors: onValidateForm(formValues) });
    } else {
      dispatch({ type: 'FIELD_CHANGED', formValues, errors: state.errors });
    }
  }

  return ({
    formValues: state.formValues,
    errors: state.errors,
    isSaving: state.isSaving,
    onChange,
    onSave,
  })
}

export default useFormController;