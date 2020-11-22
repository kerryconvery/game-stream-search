import { renderHook, act } from '@testing-library/react-hooks'
import useInfiniteStreamLoader from '../useInfiniteStreamLoader';

describe('Use game stream data hook', () => {
  it('should start loading game streams when initially invoked', () => {
    const streamData = {
      items: [],
      nextPageToken: null,
    }

    const loadStreamsStub = () => new Promise(() => streamData);

    const { result } = renderHook(() => useInfiniteStreamLoader(loadStreamsStub, jest.fn()))

    expect(result.current.isLoading).toBeTruthy();
  });

  it('should return streams after the streams are loaded', async () => {
    const streamData = {
      items: [{},{}],
      nextPageToken: null,
    }
    
    const loadStreamsStub = () => new Promise(resolve => resolve(streamData));

    const { result } = renderHook(() => useInfiniteStreamLoader(loadStreamsStub, jest.fn()))

    await act(loadStreamsStub);
 
    expect(result.current.items.length).toEqual(2);
    expect(result.current.isLoading).toBeFalsy();
  });

  it('should start to load new streams when more streams are requested', async () => {
    const streams = {
      items: [{},{}],
      nextPageToken: 'next page token',
    }
    
    const loadStreamsStub = () => new Promise(resolve => resolve(streams));

    const { rerender, result } = renderHook(() => useInfiniteStreamLoader(loadStreamsStub, jest.fn()))

    await act(loadStreamsStub);

    rerender(new Promise(() => streams), jest.fn());

    act(() => { result.current.loadMoreStreams() });

    expect(result.current.isLoading).toBeTruthy();

    await act(loadStreamsStub);
  });

  it('should return new and existing streams when more streams are loaded', async () => {
    const streams = {
      items: [{},{}],
      nextPageToken: 'next page token',
    }
    
    const loadStreamsStub = () => new Promise(resolve => resolve(streams));

    const { result } = renderHook(() => useInfiniteStreamLoader(loadStreamsStub, jest.fn()))

    await act(loadStreamsStub);
    
    act(() => { result.current.loadMoreStreams() });

    await act(loadStreamsStub);

    expect(result.current.items.length).toEqual(4);
    expect(result.current.isLoading).toBeFalsy();
  });

  it('should return streams filtered by game when the filter changes', async () => {
    const streams = {
      testGameA: {
        items: [{},{}],
        nextPageToken: 'next page token',
      },
      testGameB: {
        items: [{},{},{}],
        nextPageToken: 'next page token',
      }
    }

    const loadGameStreamsStub = filters => new Promise(resolve => resolve(streams[filters.gameName]));

    const { result } = renderHook(() => {
      return useInfiniteStreamLoader(loadGameStreamsStub, jest.fn(), { gameName: 'testGameA' });
    });

    await act(() => loadGameStreamsStub({ gameName: 'testGameA' }));

    act(() => result.current.filterStreams({ gameName: 'testGameB' }));
 
    await act(() => loadGameStreamsStub({ gameName: 'testGameB' }));

    expect(result.current.items.length).toEqual(3);
    expect(result.current.isLoading).toBeFalsy();
  });

  it('should report that more streams are available when there is a next page token', async () => {
    const streamData = {
      items: [{},{}],
      nextPageToken: 'next page token',
    }
    
    const loadStreamsStub = () => new Promise(resolve => resolve(streamData));

    const { result } = renderHook(() => useInfiniteStreamLoader(loadStreamsStub, jest.fn()))

    await act(loadStreamsStub);
 
    expect(result.current.hasMoreStreams).toBeTruthy();
  });

  it('should report that there are no more streams are available when there is no next page token', async () => {
    const streamData = {
      items: [{},{}],
      nextPageToken: null,
    }
    
    const loadStreamsStub = () => new Promise(resolve => resolve(streamData));

    const { result } = renderHook(() => useInfiniteStreamLoader(loadStreamsStub, jest.fn()))

    await act(loadStreamsStub);
 
    expect(result.current.hasMoreStreams).toBeFalsy();
  });

  it('should report an error when when there is an error loading channels', async () => {
    const onError = jest.fn();
    const loadStreamsStub = () => new Promise((resolve, reject) => reject(new Error('test exception')));

    renderHook(() => useInfiniteStreamLoader(loadStreamsStub, onError));

    try {
      await act(loadStreamsStub);
    } catch{
      expect(onError).toHaveBeenCalled();
    }
  })
})