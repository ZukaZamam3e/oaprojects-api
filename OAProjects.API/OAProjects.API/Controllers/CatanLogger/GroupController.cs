using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OAProjects.Models.CatanLogger.Models;
using OAProjects.Models.CatanLogger.Responses;
using OAProjects.Models.Common.Responses;
using OAProjects.Store.CatanLogger.Stores.Interfaces;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using System.Linq.Expressions;

namespace OAProjects.API.Controllers.CatanLogger;

[ApiController]
[Route("api/catan-logger/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize("User.ReadWrite")]
public class GroupController(
        ILogger<GroupController> logger,
        IUserStore userStore,
        IHttpClientFactory httpClientFactory,
        IGroupStore _groupStore
    ) : BaseController(logger, userStore, httpClientFactory)
{
    [HttpGet("Load")]
    public async Task<IActionResult> Load(int take = 10)
    {
        GetResponse<GroupLoadResponse> response = new();

        try
        {
            int userId = await GetUserId();

            response.Model = new GroupLoadResponse
            {
                Groups = GetGroups(userId)
            };
            response.Model.Count = response.Model.Groups.Count();
            response.Model.Groups = response.Model.Groups.Take(take);
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpGet("Get")]
    public async Task<IActionResult> Get(int offset = 0, string? search = null, int take = 10)
    {
        GetResponse<GroupGetResponse> response = new();

        try
        {
            int userId = await GetUserId();

            response.Model = new GroupGetResponse
            {
                Groups = GetGroups(userId, search)
            };
            response.Model.Count = response.Model.Groups.Count();
            response.Model.Groups = response.Model.Groups.Skip(offset).Take(take);
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<GroupModel> GetGroups(int userId, string? search = null)
    {
        Expression<Func<GroupModel, bool>>? predicate = null;

        if (!string.IsNullOrEmpty(search))
        {
            predicate = m => m.GroupName.ToLower().Contains(search.ToLower())
                    && m.Users.Any(m => m.UserId == userId);
        }
        else
        {
            predicate = m => m.Users.Any(m => m.UserId == userId);
        }

        IEnumerable<GroupModel> query = _groupStore.GetGroups(predicate);

        return query;
    }

    [HttpPost("SaveGroup")]
    public async Task<IActionResult> SaveGroup(GroupModel model,
        [FromServices] IValidator<GroupModel> validator)
    {
        PostResponse<GroupModel> response = new();

        try
        {
            int userId = await GetUserId();
            ValidationResult result = await validator.ValidateAsync(model);

            if (!result.IsValid)
            {
                response.Errors = result.Errors.Select(m => m.ErrorMessage);
            }
            else
            {
                int groupId = model.GroupId;

                if (groupId <= 0)
                {
                    groupId = _groupStore.CreateGroup(userId, model);
                }
                else
                {
                    _groupStore.UpdateGroup(userId, model);
                }

                response.Model = _groupStore.GetGroups(m => m.GroupId == groupId && m.Users.Any(n => n.UserId == userId)).First();
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    // Add User
    // Remove User
    // Change User Role
    //  - Admin can only change roles
    //  - Admin cannot remove admin


}
