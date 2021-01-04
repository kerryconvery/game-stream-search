const trackStreamClick = (streamTitle, streamerName, streamPlatformName, views) => {

}

const trackStreamSearch = (searchTerm) => {

}

const trackFeaturedChannelClick = (streamerName, streamPlatformName) => {

}

export const useTelemetryTrackerApi = () => {
  return {
    trackStreamClick,
    trackStreamSearch,
    trackFeaturedChannelClick,
  }
}