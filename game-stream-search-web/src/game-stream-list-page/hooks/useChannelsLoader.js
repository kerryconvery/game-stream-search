import { useReducer, useEffect } from 'react';

const reducer = (state, action) => {
  switch (action.type) {
    case 'CHANNELS_LOADED': {
      return {
        ...state,
        channels: action.channels,
        isLoading: false
      }
    }
  }
}

const initialState = {
  channels: [],
  isLoading: true,
}

const useChannelsLoader = (onLoadChannels, onLoadError) => {
  const [ state, dispatch ] = useReducer(reducer, initialState);

  useEffect(() => {
    onLoadChannels()
      .then(channels => dispatch({ type: 'CHANNELS_LOADED', channels }))
      .catch(onLoadError)
  }, [])

  return state;
}

export default useChannelsLoader;