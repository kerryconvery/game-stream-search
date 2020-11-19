import { useReducer, useEffect } from 'react';

const reducer = (state, action) => {
  switch (action.type) {
    case 'STREAMS_LOADED': {
      return {
        ...state,
        items: state.items.concat(action.data.items),
        nextPageToken: action.data.nextPageToken,
        isLoading: false,
      }
    }
    case 'FILTER_STREAMS': {
      return {
        ...state,
        items: [],
        filters: {...state.filters, ...action.filters },
        nextPageToken: null,
        currentPageToken: null,
        isLoading: true,
      }
    }
    case 'LOAD_MORE_STREAMS': {
      return {
        ...state,
        currentPageToken: state.nextPageToken,
        isLoading: true,
      }
    }
    default: throw new Error(`useInfiniteStreamLoader - unknown action ${action.type}`);
  }
}

const initialState = {
  items: [],
  filters: {},
  nextPageToken: null,
  currentPageToken: null,
  isLoading: true
};

const useInfiniteStreamLoader = (onLoadStreams, onLoadError, initialFilters = {}) => {
  const [ state, dispatch ] = useReducer(reducer, { ...initialState, filters: initialFilters });

  const hasMoreStreams = state.nextPageToken !== null && !state.isLoading;

  const loadMoreStreams = () => dispatch({ type: 'LOAD_MORE_STREAMS' });
  const filterStreams = filters => dispatch({ type: 'FILTER_STREAMS', filters });
  
  useEffect(() => {
    onLoadStreams(state.filters, state.currentPageToken)
      .then(data => dispatch({ type: 'STREAMS_LOADED', data }))
      .catch(onLoadError);
  }, [state.filters, state.currentPageToken]);
  
  return {
    items: state.items,
    isLoading: state.isLoading,
    filters: state.filters,
    hasMoreStreams,
    loadMoreStreams,
    filterStreams,
  };
}

export default useInfiniteStreamLoader;