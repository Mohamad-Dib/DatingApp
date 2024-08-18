using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using API.Entities;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController(UserManager<AppUser> userManager,
            IUnitOfWork unitOfWork, IPhotoService photoService) : BaseApiController
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
    public async Task<ActionResult> GetPhotosForModeration()
    {
        var photos = await unitOfWork.PhotoRepository.GetUnapprovedPhotos();
        return Ok(photos);
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("approve-photo/{photoId}")]
    public async Task<ActionResult> ApprovePhoto(int photoId)
    {
        var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);
        if (photo == null) return BadRequest("Could not get photo from the database");
        photo.IsApproved = true;

        var user = await unitOfWork.UserRepository.GetUserByPhotoId(photoId);   
        if(user==null) return BadRequest("Could not get user from database");
        if(!user.Photos.Any(x=>x.IsMain)) photo.IsMain = true;  
        await unitOfWork.Complete();
        return Ok();

    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("reject-photo/{photoId}")]
    public async Task<ActionResult> RejectPhoto(int photoId)
    {
        var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);

        if (photo == null) return BadRequest("Could not get photo from the database");

        if (photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Result == "ok")
            {
                unitOfWork.PhotoRepository.RemovePhoto(photo);

            }
        }
        else
        {
            unitOfWork.PhotoRepository.RemovePhoto(photo);

        }
        await unitOfWork.Complete();    
        return Ok();




    }


}