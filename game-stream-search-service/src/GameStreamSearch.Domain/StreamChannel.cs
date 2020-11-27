using System;
using System.Collections.Generic;
using GameStreamSearch.Domain.Dto;
using GameStreamSearch.Domain.Enums;

namespace GameStreamSearch.Domain
{
    public class StreamChannel
    {
        public StreamChannel()
        {
            Status = ChannelStatusType.Pending;
        }

        public string ChannelName { get; set; }
        public DateTime DateRegistered { get; set; }
        public ChannelStatusType Status { get; set; }
        public StreamPlatformType StreamPlatform { get; set; }
        public string AvatarUrl { get; set; }
        public string ChannelUrl { get; set; }

        public IEnumerable<ValidationMessage> Validate()
        {
            var errors = new List<ValidationMessage>();

            if (string.IsNullOrEmpty(ChannelName))
            {
                errors.Add(new ValidationMessage("ChannelName", "Stream channel name cannot be blank"));
            }

            return errors;
        }
    }
}
