using System;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController(UserManager<AppUser> userManager) : BaseApiController
{
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await userManager.Users
                 .OrderBy(x => x.UserName)
                 .Select(x => new
                 {
                     x.Id,
                     Username = x.UserName,
                     Roles = x.UserRoles.Select(r => r.Role.Name).ToList()
                 }).ToListAsync();

        return Ok(users);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");

        var selectedRoles = roles.Split(',').ToArray();

        var user = await userManager.FindByNameAsync(username);
        if (user == null) return BadRequest("User not found");
        var UserRoles = await userManager.GetRolesAsync(user);

        var results = await userManager.AddToRolesAsync(user, selectedRoles.Except(UserRoles));

        if (!results.Succeeded) return BadRequest("Failed to add roles");


        results = await userManager.RemoveFromRolesAsync(user, UserRoles.Except(selectedRoles));

        if (!results.Succeeded) return BadRequest("Failed to remove from roles");
        return Ok(await userManager.GetRolesAsync(user));
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public ActionResult GetPhotosForModeration()
    {
        return Ok("Only admins or moderators can see this");
    }

}
