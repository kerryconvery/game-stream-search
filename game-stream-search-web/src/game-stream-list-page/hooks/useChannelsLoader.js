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
    case 'UPDATE_CHANNELS': {
      return {
        ...state,
        channels: action.channels,
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

  const updateChannels = channels => dispatch({ type: 'UPDATE_CHANNELS', channels });

  useEffect(() => {
    onLoadChannels()
      .then(channels => dispatch({ type: 'CHANNELS_LOADED', channels }))
      .catch(onLoadError)
  }, [])

  return {
    channels: state.channels,
    isLoading: state.isLoading,
    updateChannels,
  };
}

export default useChannelsLoader;