﻿using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Entities.Activities;
using Domain.Entities.Comments;
using Domain.Entities.Junctions;
using Domain.Entities.Users;

namespace Application.Core;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        var currentUsername = String.Empty;
        
        CreateMap<Activity, Activity>();

        CreateMap<Activity, ActivityDto>()
            .ForMember(
                d => d.HostUsername, 
                o => o.MapFrom(s =>
                    s.Attendees.FirstOrDefault(a => a.IsHost)!.User.UserName)
            );
        
        CreateMap<ActivityAttendee, AttendeeDto>()
            .ForMember(d => d.DisplayName, 
                o => o.MapFrom(s =>
                    s.User.DisplayName)
            )
            .ForMember(d => d.Username, 
                o => o.MapFrom(s =>
                    s.User.UserName)
            )
            .ForMember(d => d.Biography, 
                o => o.MapFrom(s =>
                    s.User.Biography)
            )
            .ForMember(d => d.ImageUri, 
                o => o.MapFrom(s =>
                    s.User.Images.FirstOrDefault(i => i.IsMain)!.Uri)
            )
            .ForMember(d => d.FollowersCount, 
                o => o.MapFrom(s =>
                    s.User.Followers.Count)
            )
            .ForMember(d => d.FollowingCount, 
                o => o.MapFrom(s =>
                    s.User.Followings.Count)
            )
            .ForMember(d => d.IsFollowing, 
                o => o.MapFrom(s =>
                    s.User.Followers.Any(f => f.Observer.UserName == currentUsername))
            );

        CreateMap<User, UserProfile>()
            .ForMember(d => d.ImageUri, 
                o => o.MapFrom(s =>
                s.Images.FirstOrDefault(i => i.IsMain)!.Uri)
            )
            .ForMember(d => d.FollowersCount, 
                o => o.MapFrom(s =>
                    s.Followers.Count)
            )
            .ForMember(d => d.FollowingCount, 
                o => o.MapFrom(s =>
                    s.Followings.Count)
            )
            .ForMember(d => d.IsFollowing, 
                o => o.MapFrom(s =>
                    s.Followers.Any(f => f.Observer.UserName == currentUsername))
            );

        CreateMap<ActivityAttendee, UserActivityDto>()
            .ForMember(d => d.Id, 
                o => o.MapFrom(s =>
                    s.Activity.Id)
            )
            .ForMember(d => d.Title, 
                o => o.MapFrom(s =>
                    s.Activity.Title)
            )
            .ForMember(d => d.Date, 
                o => o.MapFrom(s =>
                    s.Activity.Date)
            )
            .ForMember(d => d.Category, 
                o => o.MapFrom(s =>
                    s.Activity.Category)
            )
            .ForMember(d => d.ModerationStatus,
                o => o.MapFrom( s => 
                    s.Activity.ModerationStatus)
            )
            .ForMember(d => d.HostUsername, 
                o => o.MapFrom(s =>
                    s.Activity.Attendees.FirstOrDefault(aa => aa.IsHost)!.User.UserName)
            );

        CreateMap<Comment, CommentDto>()
            .ForMember(d => d.DisplayName,
                o => o.MapFrom(s =>
                    s.Author.DisplayName)
            )
            .ForMember(d => d.Username,
                o => o.MapFrom(s =>
                    s.Author.UserName)
            )
            .ForMember(d => d.ImageUri,
                o => o.MapFrom(s =>
                    s.Author.Images.FirstOrDefault(i => i.IsMain)!.Uri)
            );
    }
}