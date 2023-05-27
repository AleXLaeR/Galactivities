using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.Enums;

namespace Domain.DTOs;

public class UserActivityDto
{
    public Guid Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Category { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    [Required]
    public ModerationStatus ModerationStatus { get; set; }

    [JsonIgnore]
    public string HostUsername { get; set; }
}