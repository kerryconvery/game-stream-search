const trackStreamOpened = ({ streamTitle, streamerName, streamPlatformName, views }) => {

}

const trackStreamSearch = ({ gameName }) => {

}

const trackFeaturedChannelOpened = ({ streamerName, streamPlatformName }) => {

}

export const useTelemetryTrackerApi = () => {
  return {
    trackStreamOpened,
    trackStreamSearch,
    trackFeaturedChannelOpened,
  }
}