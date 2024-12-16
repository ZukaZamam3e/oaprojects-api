using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OAProjects.Models.CatanLogger.Models;
using OAProjects.Models.CatanLogger.Requests;
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
public class GameController(
        ILogger<GameController> logger,
        IUserStore userStore,
        IHttpClientFactory httpClientFactory,
        IGameStore _gameStore
    ) : BaseController(logger, userStore, httpClientFactory)
{
    [HttpGet("Load")]
    public async Task<IActionResult> Load(int groupId, int take = 10)
    {
        GetResponse<GameLoadResponse> response = new();

        try
        {
            int userId = await GetUserId();

            response.Model = new GameLoadResponse
            {
                Games = GetGames(groupId, userId)
            };
            response.Model.Count = response.Model.Games.Count();
            response.Model.Games = response.Model.Games.Take(take);
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpGet("Get")]
    public async Task<IActionResult> Get(int groupId, int offset = 0, string? search = null, int take = 10)
    {
        GetResponse<GameGetResponse> response = new();

        try
        {
            int userId = await GetUserId();

            response.Model = new GameGetResponse
            {
                Games = GetGames(groupId, userId, search)
            };
            response.Model.Count = response.Model.Games.Count();
            response.Model.Games = response.Model.Games.Skip(offset).Take(take);
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<GameModel> GetGames(int groupId, int userId, string? search = null)
    {
        Expression<Func<GameModel, bool>>? predicate = null;

        if (!string.IsNullOrEmpty(search))
        {
            // add search
        }
        else
        {
            //predicate = m => m.GroupId == groupId;
        }

        IEnumerable<GameModel> query = _gameStore.GetGames(groupId, predicate);

        return query;
    }

    [HttpPost("SaveGame")]
    public async Task<IActionResult> SaveGame(GameModel model,
        [FromServices] IValidator<GameModel> validator)
    {
        PostResponse<GameModel> response = new();

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
                int gameId = model.GroupId;

                response.Model = _gameStore.SaveGame(model.GroupId, userId, model);
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    

    [HttpPost("DeleteGame")]
    public async Task<IActionResult> DeleteGame(GameIdRequest request,
        [FromServices] IValidator<GameIdRequest> validator)
    {
        PostResponse<bool> response = new();

        try
        {
            int userId = await GetUserId();
            ValidationResult result = await validator.ValidateAsync(request);

            if (!result.IsValid)
            {
                response.Errors = result.Errors.Select(m => m.ErrorMessage);
            }
            else
            {
                response.Model = _gameStore.DeleteGame(request.GroupId, userId, request.GameId);
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }
}
