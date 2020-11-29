import { renderHook, act } from '@testing-library/react-hooks'
import useChannelsLoader from '../useChannelsLoader';

describe('Channels loader hook', () => {
  it('should return loading true when fetching channels', () => {
    const loadChannelsStub = () => new Promise(() => []);

    const { result } = renderHook(() => useChannelsLoader(loadChannelsStub,jest.fn()));

    expect(result.current.isLoading).toBeTruthy();
  });

  it('should return a list of channels and loading false after successfully loading channels', async () => {
    const channels = [
      {
        name: 'test channel 1',
        streamPlatformDisplayName: 'YouTube',
        channelAvatarUrl: '',
        channelUrl: '',
      },
      {
        name: 'test channel 2',
        streamPlatformDisplayName: 'DLive',
        channelAvatarUrl: '',
        channelUrl: '',
      }
    ];

    const loadChannelsStub = () => new Promise((resolve) => resolve(channels));
    const { result } = renderHook(() => useChannelsLoader(loadChannelsStub,jest.fn()));

    await act(loadChannelsStub);

    expect(result.current.isLoading).toBeFalsy();
    expect(result.current.channels).toEqual(channels);  
  });

  it('should report an error when when there is an error loading channels', async () => {
    const onError = jest.fn();
    const loadChannelsStub = () => new Promise((resolve, reject) => reject(new Error('test exception')));

    renderHook(() => useChannelsLoader(loadChannelsStub, onError));

    try {
      await act(loadChannelsStub);
    } catch{
      expect(onError).toHaveBeenCalled();
    }
  })

  it('should replace channels with a new list of channels', async () => {
    const channels = [
      {
        channelName: 'test channel 1',
        streamPlatformDisplayName: 'YouTube',
        avatarUrl: '',
        channelUrl: '',
      },
    ];

    const newChannels = [
      {
        channelName: 'test channel 1',
        streamPlatformDisplayName: 'YouTube',
        avatarUrl: 'updated url',
        channelUrl: '',
      },
      {
        channelName: 'test channel 2',
        streamPlatformDisplayName: 'DLive',
        avatarUrl: '',
        channelUrl: '',
      }
    ];

    const loadChannelsStub = () => new Promise((resolve) => resolve(channels));
    const { result } = renderHook(() => useChannelsLoader(loadChannelsStub,jest.fn()));

    await act(loadChannelsStub);

    const updateChannels = result.current.updateChannels;

    act(() => {
      updateChannels(newChannels);
    });
  
    expect(result.current.channels.length).toEqual(2);  
    expect(result.current.channels[0].channelName).toEqual(channels[0].channelName);
    expect(result.current.channels[0].avatarUrl).toEqual(newChannels[0].avatarUrl);
    expect(result.current.channels[1].channelName).toEqual(newChannels[1].channelName);    
  })
})