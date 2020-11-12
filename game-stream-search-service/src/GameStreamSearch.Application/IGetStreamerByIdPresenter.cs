using System;
using GameStreamSearch.Application.Dto;

namespace GameStreamSearch.Application
{
    public interface IGetStreamerByIdPresenter
    {
        void PresentStreamer(StreamerDto streamer);
        void PresentStreamerNotFound();
    }
}
