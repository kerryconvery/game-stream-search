const trackStreamOpened = ({ streamTitle, streamerName, platformName, views }) => {

}

const trackStreamSearch = ({ gameName }) => {

}

const trackFeaturedChannelOpened = ({ channelName, platformName }) => {

}

export const getTelemetryTrackerApi = (telemetryTrackerServiceUrl, telemetryTrackerServiceKey) => {
  return {
    trackStreamOpened,
    trackStreamSearch,
    trackFeaturedChannelOpened,
  }
}