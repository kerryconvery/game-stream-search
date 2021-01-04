import React, { createContext, useContext } from 'react';
import { node, object } from 'prop-types';

const StreamServiceContext = createContext({});

export const StreamServiceProvider = ({ streamServiceApi, children }) => (
  <StreamServiceContext.Provider value={streamServiceApi}>
    {children}
  </StreamServiceContext.Provider>
);

StreamServiceProvider.propTypes = {
  streamServiceApi: object.isRequired,
  children: node.isRequired,
}

export const useStreamService = () => useContext(StreamServiceContext);
