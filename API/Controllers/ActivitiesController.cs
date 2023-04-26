﻿using Application.Activities;
using Application.Attendance;
using Domain.Entities;
using Domain.Entities.Activities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ActivitiesController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetActivities([FromQuery] FilterParams? sortingParams)
    {
        return HandlePagedResult(await Mediator.Send(new List.Query
        {
            Params = sortingParams ?? new FilterParams()
        }));
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetActivity([FromRoute] Guid id)
    {
        return HandleResult(await Mediator.Send(new Details.Query { Id = id }));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateActivity([FromBody] Activity activity)
    {
        return HandleResult(await Mediator.Send(new Create.Command { Activity = activity }));
    }
    
    [Authorize(Policy = "IsActivityHost")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> EditActivity([FromRoute] Guid id, [FromBody] Activity activity)
    {
         activity.Id = id;
         return HandleResult(await Mediator.Send(new Edit.Command { Activity = activity }));
    }
    
    [Authorize(Policy = "IsActivityHost")]
    //[Authorize(Policy = "IsAdmin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteActivity([FromRoute] Guid id)
    {
        return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
    }

    [HttpPost("{id:guid}/attend")]
    public async Task<IActionResult> Attend([FromRoute] Guid id)
    {
        return HandleResult(await Mediator.Send(new UpdateAttendance.Command { Id = id }));
    }
    
    [Authorize(Policy = "IsAdmin")]
    [HttpPut("{id:guid}/approve")]
    public async Task<IActionResult> ApproveActivity([FromRoute] Guid id)
    {
        return HandleResult(await Mediator.Send(new ChangeStatus.Command { Id = id, Status = ModerationStatus.Resolved }));
    }
    
    [Authorize(Policy = "IsAdmin")]
    [HttpPut("{id:guid}/reject")]
    public async Task<IActionResult> RejectActivity([FromRoute] Guid id)
    {
        return HandleResult(await Mediator.Send(new ChangeStatus.Command { Id = id, Status = ModerationStatus.Rejected }));
    }
    
}