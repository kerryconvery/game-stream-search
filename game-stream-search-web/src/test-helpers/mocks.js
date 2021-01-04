export const telemetryTrackerApiMocks = {
  trackStreamOpened: jest.fn(),
  trackStreamSearch: jest.fn(),
  trackFeaturedChannelOpened: jest.fn(),
}

export const automockObject  = (object) => {
  Object.keys(object).forEach(key => {
    object[key] = jest.fn();
  })
}