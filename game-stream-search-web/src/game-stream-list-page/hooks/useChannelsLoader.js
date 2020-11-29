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
    case 'APPEND_CHANNEL': {
      return {
        ...state,
        channels: state.channels.concat(action.channel),
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

  const appendChannel = channel => dispatch({ type: 'APPEND_CHANNEL', channel });

  useEffect(() => {
    onLoadChannels()
      .then(channels => dispatch({ type: 'CHANNELS_LOADED', channels }))
      .catch(onLoadError)
  }, [])

  return {
    channels: state.channels,
    isLoading: state.isLoading,
    appendChannel,
  };
}

export default useChannelsLoader;