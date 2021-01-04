import React, { createContext, useContext } from 'react';
import { node, object } from 'prop-types';

const TelemetryTrackerContext = createContext({});

export const TelemetryTrackerProvider = ({ telemetryTrackerApi, children }) => (
  <TelemetryTrackerContext.Provider value={ telemetryTrackerApi }>
    {children}
  </TelemetryTrackerContext.Provider>
);

TelemetryTrackerProvider.propTypes = {
  telemetryTrackerApi: object.isRequired,
  children: node.isRequired,
}

export const useTelemetryTracker = () => useContext(TelemetryTrackerContext);
