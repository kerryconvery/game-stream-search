import React from 'react';
import ReactDOM from 'react-dom';
import App from './app';
import config from '../config.json';
import { StreamServiceProvider } from './providers/StreamServiceProvider';
import { TelemetryTrackerProvider } from './providers/TelemetryTrackerProvider';
import { getStreamServiceApi } from './api/streamServiceApi';
import { getTelemetryTrackerApi } from './api/telemetryTrackerApi';

/* eslint-disable-next-line no-undef */
const configuration = config.env[process.env.APP_ENV];
const streamServiceApi = getStreamServiceApi(configuration.streamServiceUrl);
const telemetryTrackerApi = getTelemetryTrackerApi();

ReactDOM.render(
  <StreamServiceProvider streamServiceApi={streamServiceApi} >
    <TelemetryTrackerProvider telemetryTrackerApi={telemetryTrackerApi} >
      <App />
    </TelemetryTrackerProvider>
  </StreamServiceProvider>,
  document.getElementById('app')
);
