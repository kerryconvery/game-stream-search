import axios from 'axios';

const StatusType = {
  Created: 'added',
  Updated: 'updated',
  Ok: 'ok',
  BadRequest: 'badRequest',
  NotFound: 'notFound',
  ServerError: 'serverError',
  FailedDependency: 'failedDependency',
}

const httpStatusToStatusType = (httpStatus) => {
  switch(httpStatus) {
    case 200: return StatusType.Ok;
    case 201: return StatusType.Created;
    case 400: return StatusType.BadRequest;
    case 500: return StatusType.ServerError;
    case 404: return StatusType.NotFound;
    case 424: return StatusType.FailedDependency;
  }
}

const getStreamsRequest = (baseUrl) => (filters = {}, pageToken) => (
  axios({
    url: `${baseUrl}/streams`,
    method: 'GET',
    params: {
      game: filters.gameName,
      pageSize: 10,
      pageToken: pageToken,
    },
  }).then(res => res.data)
);

const addChannelRequest = (baseUrl) => (data) => (
  axios({
    url: `${baseUrl}/channels/${data.streamPlatform}/${data.channelName}`,
    method: 'PUT',
  })
  .then(res => (
    {
      status: httpStatusToStatusType(res.status), 
      channel: res.data
    }
  ))
  .catch(error => {
    return (
    {
      status: httpStatusToStatusType(error.response.status),
      errors: error.response.data.errors,
    }
  )})
)

const getChannelsRequest = (baseUrl) => () => (
  axios({
    url: `${baseUrl}/channels`,
    method: 'GET',
    params: {},
  }).then(res => res.data.channels)
);

const mapHttpResponse = (httpResponse, mapApiErrorsToFields) => {
  return {
    success: httpResponse.errors === undefined,
    isCreated: httpResponse.status === StatusType.Created,
    errors: httpResponse.errors ? mapApiErrorsToFields(httpResponse.errors) : undefined,
  }
}

export const getStreamServiceApi = (streamServiceUrl) => {
  return {
    mapHttpResponse,
    getStreams: getStreamsRequest(streamServiceUrl),
    addChannel: addChannelRequest(streamServiceUrl),
    getChannels: getChannelsRequest(streamServiceUrl), 
  }
}