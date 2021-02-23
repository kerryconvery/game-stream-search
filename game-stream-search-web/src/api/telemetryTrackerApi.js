const trackStreamOpened = ({ streamTitle, streamerName, platformName, views }) => {

}

const trackStreamSearch = ({ gameName }) => {

}

const trackFeaturedChannelOpened = ({ channelName, streamPlatformDisplayName }) => {

}

export const getTelemetryTrackerApi = (telemetryTrackerServiceUrl, telemetryTrackerServiceKey) => {
  return {
    trackStreamOpened,
    trackStreamSearch,
    trackFeaturedChannelOpened,
  }
}