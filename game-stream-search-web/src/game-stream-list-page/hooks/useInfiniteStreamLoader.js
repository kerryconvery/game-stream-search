import { useEffect } from 'react';
import useReducers from '../../shared-components/hooks/useReducers';

const createReducers = state => ({
  streamsLoaded: (streams, nextPageToken) => (
    { ...state, streams: state.streams.concat(streams), nextPageToken, isLoading: false }
  ),
  filterStreams: (filters) => (
    {
      ...state,
      streams: [],
      filters: { ...state.filters, ...filters },
      nextPageToken: null, 
      currentPageToken: null,
      isLoading: true,
    }
  ),
  loadMoreStreams: () => (
    { ...state, currentPageToken: state.nextPageToken, isLoading: true }
  ),
})

const initialState = {
  streams: [],
  filters: {},
  nextPageToken: null,
  currentPageToken: null,
  isLoading: true
};

const useInfiniteStreamLoader = (onLoadStreams, onLoadError, initialFilters = {}) => {
  const [
    state,
    streamsLoaded,
    filterStreams,
    loadMoreStreams,
  ] = useReducers(createReducers, { ...initialState, filters: initialFilters });

  console.log('state.nextPageToken', state.nextPageToken);
  const hasMoreStreams = state.nextPageToken !== "" && !state.isLoading;
  
  useEffect(() => {
    onLoadStreams(state.filters, state.currentPageToken)
      .then(data => streamsLoaded(data.streams, data.nextPageToken))
      .catch(onLoadError);
  }, [state.filters, state.currentPageToken]);
  
  return {
    streams: state.streams,
    isLoading: state.isLoading,
    filters: state.filters,
    hasMoreStreams,
    loadMoreStreams: loadMoreStreams,
    filterStreams: filterStreams,
  };
}

export default useInfiniteStreamLoader;