using System;
using System.ComponentModel.DataAnnotations;
using GameStreamSearch.Domain.Enums;

namespace GameStreamSearch.Api.Dto
{
    public class RegisterChannelDto
    {
        [Required]
        public string ChannelName { get; set; }
        [Required]
        public StreamPlatformType StreamPlatform { get; set; }
    }
}
