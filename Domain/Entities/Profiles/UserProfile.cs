﻿using System.ComponentModel.DataAnnotations;
using Domain.Entities.Images;

namespace Domain.Entities.Profiles;

public class UserProfile
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string DisplayName { get; set; }

    public string? Biography { get; set; }
    
    public string? ImageUri { get; set; }

    public List<Image> Images { get; set; } = new();
}