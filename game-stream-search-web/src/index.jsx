import React from 'react';
import ReactDOM from 'react-dom';
import App from './app';
import config from '../config.json';
import { ConfigurationProvider } from './providers/ConfigurationProvider';
import { TelemetryTrackerProvider } from './providers/TelemetryTrackerProvider';
import { getTelemetryTrackerApi } from './api/telemetryTrackerApi';

/* eslint-disable-next-line no-undef */
const configuration = config.env[process.env.APP_ENV];

ReactDOM.render(
  <ConfigurationProvider configuration={configuration} >
    <TelemetryTrackerProvider telemetryTrackerApi={getTelemetryTrackerApi(configuration)} >
      <App />
    </TelemetryTrackerProvider>
  </ConfigurationProvider>,
  document.getElementById('app')
);
