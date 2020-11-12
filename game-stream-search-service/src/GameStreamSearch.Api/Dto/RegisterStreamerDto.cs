using System;
using System.ComponentModel.DataAnnotations;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application.Dto
{
    public class RegisterStreamerDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public StreamingPlatform Platform { get; set; }
    }
}
