import React from 'react';
import _get from 'lodash/get';
import InfiniteScroll from 'react-infinite-scroller';
import { useGameStreamApi } from '../api/gameStreamApi';
import { useTelemetryTrackerApi } from '../providers/TelemetryTrackerProvider';
import useEventBus from '../event-bus/eventBus';
import { postNotificationEvent, buildOfflineAlertEvent } from '../notifications/events';
import useInfiniteStreamLoader from './hooks/useInfiniteStreamLoader';
import GameStreamPageTemplate from './GameStreamPageTemplate';
import GameStreamSearchBar from './components/GameStreamSearchBar';
import GameStreamGrid from './components/GameStreamGrid';
import NoStreamsFound from './components/NoStreamsFound';
import ChannelsSideBar from './components/channels-side-bar/ChannelsSideBar';

const GameStreamListPage = () => {
  const { getStreams } = useGameStreamApi();
  const { dispatchEvent } = useEventBus();
  const { trackStreamOpened, trackStreamSearch } = useTelemetryTrackerApi();

  const showErrorAlert = () => postNotificationEvent(dispatchEvent, buildOfflineAlertEvent());

  const streams = useInfiniteStreamLoader(getStreams, showErrorAlert);

  const filterStreams = (gameName) => {
    streams.filterStreams({ gameName });
    trackStreamSearch({ gameName });
  }

  return (
    <GameStreamPageTemplate
      searchBar={<GameStreamSearchBar onGameChange={filterStreams} />}
      leftSideBar={<ChannelsSideBar />}
      notFoundNotice={<NoStreamsFound searchTerm={streams.filters.gameName} />}
      numberOfStreams={streams.items.length}
      isLoadingStreams={streams.isLoading}
    >
      <div style={{ overflow: 'visible' }}>
        <InfiniteScroll
          pageStart={0}
          loadMore={streams.loadMoreStreams}
          hasMore={streams.hasMoreStreams}
        >
          <GameStreamGrid
            streams={streams.items}
            isLoading={streams.isLoading}
            numberOfLoadingTiles={6}
            afterStreamOpened={trackStreamOpened}
          />
        </InfiniteScroll>
      </div>
    </GameStreamPageTemplate>
  )
}

export default GameStreamListPage;