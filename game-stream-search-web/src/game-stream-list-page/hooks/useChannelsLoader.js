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
    case 'RELOAD_CHANNELS': {
      return {
        ...state,
        time: new Date().toLocaleTimeString(),
        isLoading: true,
      }
    }
  }
}

const initialState = {
  channels: [],
  time: new Date().toLocaleTimeString(),
  isLoading: true,
}

const useChannelsLoader = (onLoadChannels, onLoadError) => {
  const [ state, dispatch ] = useReducer(reducer, initialState);

  const reloadChannels = () => {
    dispatch({ type: 'RELOAD_CHANNELS' });
  }

  useEffect(() => {
    onLoadChannels()
      .then(channels => dispatch({ type: 'CHANNELS_LOADED', channels }))
      .catch(onLoadError)
  }, [state.time])

  return {
    channels: state.channels,
    isLoading: state.isLoading,
    reloadChannels,
  };
}

export default useChannelsLoader;