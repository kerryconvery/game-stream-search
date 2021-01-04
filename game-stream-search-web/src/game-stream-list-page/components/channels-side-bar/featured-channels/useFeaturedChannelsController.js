import { useState } from 'react';
import useAddChannelFormController from './useAddChannelFormController';
import { useStreamService } from '../../../../providers/StreamServiceProvider';
import useChannelsLoader from '../../../hooks/useChannelsLoader';

const useFeaturedChannelsController = () => {
  const [ anchorElement, setAnchorElement ] = useState(null);
  const { getChannels } = useStreamService();
  const { channels, isLoading, updateChannels } = useChannelsLoader(getChannels, () => {});

  const handleCloseForm = () => setAnchorElement(null);

  const handleChannelsUpdated = async () => {
    const channels = await getChannels();

    updateChannels(channels);

    handleCloseForm();
  }

  const formController = useAddChannelFormController(handleChannelsUpdated);

  return ({
    ...formController,
    channels,
    isLoading,
    anchorElement,
    onOpen: setAnchorElement,
    onClose: handleCloseForm,
  })
}

export default useFeaturedChannelsController;