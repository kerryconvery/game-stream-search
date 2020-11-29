import React from 'react';
import { render, fireEvent, waitFor, waitForElementToBeRemoved, screen } from '@testing-library/react';
import nock from 'nock';
import { ConfigurationProvider } from '../../providers/ConfigurationProvider';
import App from '../../app';
import '@testing-library/jest-dom/extend-expect';

describe('Add channel form', () => {
  const renderApplication = () => {
    return render(
      <ConfigurationProvider configuration={{ "streamSearchServiceUrl": "http://localhost:5000/api" }} >
        <App />
      </ConfigurationProvider>
    )
  }

  beforeEach(() => {
    nock('http://localhost:5000')
    .defaultReplyHeaders({
      'access-control-allow-origin': '*',
      'access-control-allow-credentials': 'true' 
    })
    .get('/api/streams?pageSize=10')
    .reply(200, { items: [] });

    nock('http://localhost:5000')
      .defaultReplyHeaders({
        'access-control-allow-origin': '*',
        'access-control-allow-credentials': 'true' ,
      })
      .get('/api/channels')
      .reply(200, { items: [] });
  })

  it('should display a form when the add button is pressed', async () => {
    renderApplication();

    // We must wait for this to avoid updated state after the component is unmounted.
    await waitFor(() => screen.getByTestId('streams-not-found'));

    const addButton = screen.getByTitle('Add a new channel to the list');

    fireEvent.click(addButton);

    const addChannelForm = await waitFor(() => screen.getByText('Add Channel'));

    expect(addChannelForm).toBeInTheDocument();
  });

  it('should close the add channel form when the cancel button is pressed', async () => {
    renderApplication();

    // We must wait for this to avoid updated state after the component is unmounted.
    await waitFor(() => screen.getByTestId('streams-not-found'));

    const addButton = screen.getByTitle('Add a new channel to the list');

    fireEvent.click(addButton);

    const addChannelForm = await waitFor(() => screen.getByText('Add Channel'));

    const cancelButton = screen.getByText('Cancel');

    fireEvent.click(cancelButton);

    await waitForElementToBeRemoved(() => screen.getByText('Add Channel'));

    expect(addChannelForm).not.toBeInTheDocument();
  });

  it('should save the new channel and close the form when the save button is pressed', async () => {
    const channel = {
      channelName: 'newchannel',
      streamPlatformDisplayName: 'Twitch',
      avatarUrl: '',
      channelUrl: '',
    };

    const updateChannels = {
      items: [channel]
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

    renderApplication();

    // We must wait for this to avoid updated state after the component is unmounted.
    await waitFor(() => screen.getByTestId('streams-not-found'));

    const addButton = screen.getByTitle('Add a new channel to the list');

    fireEvent.click(addButton);

    const addChannelForm = await waitFor(() => screen.getByText('Add Channel'));

    const channelField = screen.getByLabelText('Channel name');

    fireEvent.change(channelField, { target: { value: 'newchannel' } })

    const saveButton = screen.getByText('Save');

    fireEvent.click(saveButton);

    await waitForElementToBeRemoved(() => screen.getByText('Add Channel'));

    const addedChannel = await waitFor(() => screen.getByText("newchannel"));

    expect(addChannelForm).not.toBeInTheDocument();
    expect(addedChannel).toBeInTheDocument();
  });

  it('should display validation errors and not try to add the channel', async () => {
    renderApplication();

    // We must wait for this to avoid updated state after the component is unmounted.
    await waitFor(() => screen.getByTestId('streams-not-found'));

    const addButton = screen.getByTitle('Add a new channel to the list');

    fireEvent.click(addButton);

    await waitFor(() => screen.getByText('Add Channel'));

    const saveButton = screen.getByText('Save');

    fireEvent.click(saveButton);

    const validationError = await waitFor(() => screen.getByText('Please enter a channel name'));

    expect(validationError).toBeInTheDocument();
  });

  it('should save the new channel and close the form when the save button is pressed', async () => {
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

    renderApplication();

    // We must wait for this to avoid updated state after the component is unmounted.
    await waitFor(() => screen.getByTestId('streams-not-found'));

    const addButton = screen.getByTitle('Add a new channel to the list');

    fireEvent.click(addButton);

    await waitFor(() => screen.getByText('Add Channel'));

    const channelField = screen.getByLabelText('Channel name');

    fireEvent.change(channelField, { target: { value: 'newchannel' } })

    const saveButton = screen.getByText('Save');

    fireEvent.click(saveButton);

    const errorMessage = await waitFor(() => screen.getByText('channel not found on twitch'));

    expect(errorMessage).toBeInTheDocument();
  });
});
