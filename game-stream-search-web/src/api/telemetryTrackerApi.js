const trackStreamOpened = ({ streamTitle, streamerName, streamPlatformName, views }) => {

}

const trackStreamSearch = ({ gameName }) => {

}

const trackFeaturedChannelOpened = ({ channelName, streamPlatformDisplayName }) => {

}

export const getTelemetryTrackerApi = ({ telemetryTrackerServiceUrl, telemetryTrackerServiceKey }) => {
  return {
    trackStreamOpened,
    trackStreamSearch,
    trackFeaturedChannelOpened,
  }
}