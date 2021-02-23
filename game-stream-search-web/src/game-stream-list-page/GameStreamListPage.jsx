import React from 'react';
import _get from 'lodash/get';
import InfiniteScroll from 'react-infinite-scroller';
import { useStreamService } from '../providers/StreamServiceProvider';
import { useTelemetryTracker } from '../providers/TelemetryTrackerProvider';
import useEventBus from '../event-bus/eventBus';
import { notifyApplicationIsOffline } from '../notifications/events';
import useInfiniteStreamLoader from './hooks/useInfiniteStreamLoader';
import GameStreamPageView from './GameStreamPageView';
import GameSearchInput  from './components/GameSearchInput';
import GameStreamGrid from './components/GameStreamGrid';
import NoStreamsFound from './components/NoStreamsFound';
import ChannelsSideBar from './components/channels-side-bar/ChannelsSideBar';

const GameStreamListPage = () => {
  const { getStreams } = useStreamService();
  const { dispatchEvent } = useEventBus();
  const { trackStreamOpened, trackStreamSearch } = useTelemetryTracker();

  const streams = useInfiniteStreamLoader(getStreams, notifyApplicationIsOffline(dispatchEvent));

  const filterStreams = (gameName) => {
    streams.filterStreams({ gameName });
    trackStreamSearch({ gameName });
  }

  return (
    <GameStreamPageView
      searchBar={<GameSearchInput onGameChange={filterStreams} />}
      leftSideBar={<ChannelsSideBar />}
      notFoundNotice={<NoStreamsFound searchTerm={streams.filters.gameName} />}
      streamList={
        <div style={{ overflow: 'visible' }}>
          <InfiniteScroll
            pageStart={0}
            loadMore={streams.loadMoreStreams}
            hasMore={streams.hasMoreStreams}
          >
            <GameStreamGrid
              streams={streams.streams}
              isLoading={streams.isLoading}
              numberOfLoadingTiles={6}
              onStreamOpened={trackStreamOpened}
            />
          </InfiniteScroll>
        </div>
      }
      numberOfStreams={streams.streams.length}
      isLoadingStreams={streams.isLoading}
    />
  )
}

export default GameStreamListPage;