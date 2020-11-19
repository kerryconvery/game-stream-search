import React from 'react';
import _get from 'lodash/get';
import InfiniteScroll from 'react-infinite-scroller';
import { useGameStreamApi } from '../api/gameStreamApi';
import useEventBus from '../event-bus/eventBus';
import { dispatchAlert, getErrorAlert } from '../notifications/alerts';
import useInfiniteStreamLoader from './hooks/useInfiniteStreamLoader';
import GameStreamPageTemplate from './GameStreamPageTemplate';
import GameStreamSearchBar from './components/GameStreamSearchBar';
import GameStreamGrid from './components/GameStreamGrid';
import NoStreamsFound from './components/NoStreamsFound';
import ChannelsSideBar from './components/channels-side-bar/ChannelsSideBar';

const GameStreamListPage = () => {
  const { getStreams } = useGameStreamApi();
  const { dispatchEvent } = useEventBus();

  const showErrorAlert = () => dispatchAlert(dispatchEvent, getErrorAlert());

  const streams = useInfiniteStreamLoader(getStreams, showErrorAlert);

  return (
    <GameStreamPageTemplate
      searchBar={<GameStreamSearchBar onGameChange={gameName => streams.filterStreams({ gameName })} />}
      leftSideBar={<ChannelsSideBar />}
      // notFoundNotice={<NoStreamsFound searchTerm={streams.filters.gameName} />}
      numberOfStreams={streams.items.length}
      isLoadingStreams={streams.isLoading}
    >
      <InfiniteScroll
        pageStart={0}
        loadMore={streams.loadMoreStreams}
        hasMore={streams.hasMoreStreams}
      >
        <GameStreamGrid
          streams={streams.items}
          isLoading={streams.isLoading}
          numberOfLoadingTiles={6}
        />
      </InfiniteScroll>
    </GameStreamPageTemplate>
  )
}

export default GameStreamListPage;