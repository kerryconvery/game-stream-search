import React from 'react';
import { render, fireEvent, waitFor, waitForElementToBeRemoved, screen } from '@testing-library/react';
import nock from 'nock';
import App from '../../app';
import { StreamServiceProvider } from '../../providers/StreamServiceProvider';
import { TelemetryTrackerProvider } from '../../providers/TelemetryTrackerProvider';
import { getStreamServiceApi } from '../../api/streamServiceApi';
import { getTelemetryTrackerApi } from '../../api/telemetryTrackerApi';
import autoMockObject from '../../test-utils/autoMockObject';
import '@testing-library/jest-dom/extend-expect';

describe('Can add featured channels', () => {
  it('should display a form when the add button is pressed', async () => {
    const addChannelForm = await openTheAddChannelForm();

    expect(addChannelForm).toBeInTheDocument();
  });

  it('should close the add channel form when the cancel button is pressed', async () => {
    const addChannelForm = await openTheAddChannelForm();

    await clickTheCancelButton();

    expect(addChannelForm).not.toBeInTheDocument();
  });

  it('should save the new channel and close the form when the save button is pressed', async () => {
    const channel = {
      channelName: 'newchannel',
      platformName: 'Twitch',
      avatarUrl: '',
      channelUrl: '',
    };

    const updateChannels = {
      channels: [channel]
    }

    nock('http://localhost:5000')
      .defaultReplyHeaders({
        'access-control-allow-origin': '*',
        'access-control-allow-credentials': 'true' ,
      })
      .options('/api/channels/Twitch/newchannel')
      .reply(200)
      .put('/api/channels/Twitch/newchannel')
      .reply(201, channel)
      .get('/api/channels')
      .reply(200, updateChannels);

    await addANewChannel('newchannel');
    await waitForElementToBeRemoved(() => screen.getByText(addChannelFormName));

    const addedChannel = await waitFor(() => screen.getByText('newchannel'));
    const notification = await waitFor(() => screen.queryByText('Channel newchannel added successfully'));

    expect(addedChannel).toBeInTheDocument();
    expect(notification).toBeInTheDocument();
  });

  it('should update an exiting channel and close the form when the save button is pressed', async () => {
    const channel = {
      channelName: 'newchannel',
      platformName: 'Twitch',
      avatarUrl: '',
      channelUrl: '',
    };

    const updateChannels = {
      channels: [channel]
    }

    nock('http://localhost:5000')
      .defaultReplyHeaders({
        'access-control-allow-origin': '*',
        'access-control-allow-credentials': 'true' ,
      })
      .options('/api/channels/Twitch/newchannel')
      .reply(200)
      .put('/api/channels/Twitch/newchannel')
      .reply(200, channel)
      .get('/api/channels')
      .reply(200, updateChannels);

    await addANewChannel('newchannel');
    await waitForElementToBeRemoved(() => screen.getByText(addChannelFormName));

    const addedChannel = await waitFor(() => screen.getByText('newchannel'));
    const notification = await waitFor(() => screen.queryByText('Channel newchannel updated successfully'));

    expect(addedChannel).toBeInTheDocument();
    expect(notification).toBeInTheDocument();
  });

  it('should display validation errors and not try to add the channel', async () => {
    await openTheAddChannelForm();

    clickTheSaveButton();

    const validationError = await waitFor(() => screen.getByText('Please enter a channel name'));

    expect(validationError).toBeInTheDocument();
  });

  it('should re-validate when a field is changed', async () => {
    await openTheAddChannelForm();

    clickTheSaveButton();

    setChannelFieldValue('newchannel');

    const validationError = await waitFor(() => screen.queryByText('Please enter a channel name'));

    expect(validationError).not.toBeInTheDocument();
  });

  it('should display channel related errors returned by the service', async () => {
    const errorResponse = {
      errors: [
        {
          errorCode: 'ChannelNotFoundOnPlatform',
          errorMessage: 'channel not found on twitch',
        }
      ]
    };

    nock('http://localhost:5000')
      .defaultReplyHeaders({
        'access-control-allow-origin': '*',
        'access-control-allow-credentials': 'true' ,
      })
      .options('/api/channels/Twitch/newchannel')
      .reply(200)
      .put('/api/channels/Twitch/newchannel')
      .reply(400, errorResponse)

    await addANewChannel('newchannel');

    const errorMessage = await waitFor(() => screen.getByText('channel not found on twitch'));

    expect(errorMessage).toBeInTheDocument();
  });

  it('should display stream platform related errors returned by the service', async () => {
    const errorResponse = {
      errors: [
        {
          errorCode: 'PlatformServiceIsNotAvailable',
          errorMessage: 'streaming service is not available',
        }
      ]
    };

    nock('http://localhost:5000')
      .defaultReplyHeaders({
        'access-control-allow-origin': '*',
        'access-control-allow-credentials': 'true' ,
      })
      .options('/api/channels/Twitch/newchannel')
      .reply(200)
      .put('/api/channels/Twitch/newchannel')
      .reply(400, errorResponse)

    await addANewChannel('newchannel');

    const errorMessage = await waitFor(() => screen.getByText('streaming service is not available'));

    expect(errorMessage).toBeInTheDocument();
  });

  beforeEach(() => {
    nock('http://localhost:5000')
    .defaultReplyHeaders({
      'access-control-allow-origin': '*',
      'access-control-allow-credentials': 'true' 
    })
    .get('/api/streams?pageSize=10')
    .reply(200, { streams: [] });

    nock('http://localhost:5000')
      .defaultReplyHeaders({
        'access-control-allow-origin': '*',
        'access-control-allow-credentials': 'true' ,
      })
      .get('/api/channels')
      .reply(200, { channels: [] })
  })
  
  const telemetryTrackerApiMock = autoMockObject(getTelemetryTrackerApi());
  const addChannelFormName = 'Add Channel';

  const renderApplication = () => {
    render(
      <StreamServiceProvider streamServiceApi={getStreamServiceApi("http://localhost:5000/api")} >
        <TelemetryTrackerProvider telemetryTrackerApi={telemetryTrackerApiMock}>
          <App />
        </TelemetryTrackerProvider>
      </StreamServiceProvider>
    )

    // We must wait for this to avoid updated state after the component is unmounted.
    return waitFor(() => screen.getByTestId('streams-not-found'));
  }

  const openTheAddChannelForm = async () => {
    await renderApplication();

    const addButton = screen.getByTitle('Add a new channel to the list');

    fireEvent.click(addButton);

    return waitFor(() => screen.getByText(addChannelFormName));
  }

  const setChannelFieldValue = (channelName) => {
    const channelField = screen.getByLabelText('Channel name');

    fireEvent.change(channelField, { target: { value: channelName } })
  }
  
  const clickTheCancelButton = () => {
    const cancelButton = screen.getByText('Cancel');
    fireEvent.click(cancelButton);

    return waitForElementToBeRemoved(() => screen.getByText(addChannelFormName));
  }

  const clickTheSaveButton = () => {
    const saveButton = screen.getByText('Save');

    fireEvent.click(saveButton);
  }

  const addANewChannel = async (channelName) => {
    await openTheAddChannelForm();

    setChannelFieldValue(channelName);

    clickTheSaveButton();
  }
});
