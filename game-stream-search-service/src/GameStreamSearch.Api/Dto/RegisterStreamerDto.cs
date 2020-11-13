using System;
using System.ComponentModel.DataAnnotations;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application.Dto
{
    public class RegisterStreamerDto
    {
        [Required(ErrorMessage = "Please specify the streamers name")]
        public string Name { get; set; }
        
        public StreamingPlatform Platform { get; set; }
    }
}
