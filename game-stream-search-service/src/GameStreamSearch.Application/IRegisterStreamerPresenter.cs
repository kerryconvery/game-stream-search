using System;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application
{
    public interface IRegisterStreamerPresenter
    {
        void PresentStreamerRegistered(string streamerId);
        void PresentStreamerDoesNotHaveAChannel(string streamerName, StreamingPlatform platform);
    }
}
