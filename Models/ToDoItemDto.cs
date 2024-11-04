﻿using System.ComponentModel.DataAnnotations;

namespace ToDoList.Api.Models
{
    public record ToDoItemDto
    {
        public required string Id { get; init; }
        [Required]
        [StringLength(50)]
        public required string Description { get; set; }
        public bool Done { get; set; } = false;
        public bool Favorite { get; set; } = false;
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    }
}
