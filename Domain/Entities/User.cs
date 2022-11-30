﻿using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Images;
using Domain.Entities.Junctions;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

[Table("users", Schema = "identity")]
public class User : IdentityUser
{
    public string DisplayName { get; set; }

    public string? Biography { get; set; }
    
    public List<ActivityAttendee> Activities { get; set; } = new();

    public List<Image> Images { get; set; } = new();

    public List<UserFollowing> Followings { get; set; } = new();
    
    public List<UserFollowing> Followers { get; set; } = new();
}