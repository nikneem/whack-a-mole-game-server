using HexMaster.RedisCache.Abstractions;
using Microsoft.Extensions.Logging;
using Wam.Core.Enums;
using Wam.Games.DataTransferObjects;
using Wam.Games.DomainModels;
using Wam.Games.Repositories;
using Wam.Users.Repositories;

namespace Wam.Games.Services;

public class GamesService(
    IGamesRepository gamesRepository, 
    IUsersRepository usersRepository,
    ICacheClientFactory cacheClientFactory, 
    ILogger<GamesService> logger) : IGamesService
{

    public Task<GameDetailsDto> Get(Guid id, CancellationToken cancellationToken)
    {
        var cacheKey = $"wam:game:id:{id}";
        var cacheClient = cacheClientFactory.CreateClient();
        return cacheClient.GetOrInitializeAsync(() => GetFromRepositoryById(id, cancellationToken), cacheKey);
    }

    public Task<GameDetailsDto> GetByCode(string code, CancellationToken cancellationToken)
    {
        var cacheKey = $"wam:game:code:{code}";
        var cacheClient = cacheClientFactory.CreateClient();
        return cacheClient.GetOrInitializeAsync(() => GetFromRepositoryByCode(code, cancellationToken), cacheKey);
    }

    public async Task<GameDetailsDto> Create(CancellationToken cancellationToken)
    {
        var game = new Game();
        if (await gamesRepository.Save(game, cancellationToken) == false)
        {
            throw new Exception("Failed to save game");
        }

        var dto = ToDto(game);
            await UpdateCache(dto);

        return dto;
    }

    public async Task<GameDetailsDto> Join(string code, Guid userId, CancellationToken cancellationToken)
    {
        var user = await usersRepository.Get(userId, cancellationToken);
        var playerModel = new Player(user.Id, user.DisplayName, user.EmailAddress, user.IsExcluded);
        var game = await gamesRepository.GetByCode(code, cancellationToken);
        game.AddPlayer(playerModel);
        if (await gamesRepository.Save(game, cancellationToken) == false)
        {
            throw new Exception("Failed to save game");
        }

        var dto = ToDto(game);
        await UpdateCache(dto);
        return dto;
    }

    public async Task<GameDetailsDto> Leave(Guid gameId, Guid playerId, CancellationToken cancellationToken)
    {
        var game = await gamesRepository.Get(gameId, cancellationToken);
        var player = game.Players.FirstOrDefault(p => p.Id == playerId);
        if (player != null)
        {
            game.RemovePlayer(player);
        }

        if (await gamesRepository.Save(game, cancellationToken) == false)
        {
            throw new Exception("Failed to save game");
        }
        var dto = ToDto(game);
        await UpdateCache(dto);
        return dto;
    }

    public async Task<GameDetailsDto> UpdateState(Guid gameId, string state, CancellationToken cancellationToken)
    {
        var newState = GameState.FromCode(state);
        var game = await gamesRepository.Get(gameId, cancellationToken);
        game.ChangeState(newState);
        var dto = ToDto(game);
        await UpdateCache(dto);
        return dto;
    }

    private async Task<GameDetailsDto> GetFromRepositoryById(Guid id, CancellationToken cancellationToken)
    {
        var game = await gamesRepository.Get(id, cancellationToken);
        var dto = ToDto(game);
        return dto;
    }

    private async Task<GameDetailsDto> GetFromRepositoryByCode(string code, CancellationToken cancellationToken)
    {
        var game = await gamesRepository.GetByCode(code, cancellationToken);
        var dto = ToDto(game);
        return dto;
    }

    private static GameDetailsDto ToDto(Game game)
    {
        var dto = new GameDetailsDto
        (
            game.Id,
            game.Code,
            game.State,
            game.Players.Select(p => new GamePlayerDto(p.Id, p.DisplayName, p.EmailAddress, p.IsBanned)).ToList()
        );
        return dto;
    }

    private async Task UpdateCache(GameDetailsDto dto)
    {
        try
        {
            var cacheKeyById = $"wam:game:id:{dto.Id}";
            var cacheKeyByCode = $"wam:game:code:{dto.Code}";
            var cacheClient = cacheClientFactory.CreateClient();
            await cacheClient.SetAsAsync(cacheKeyById, dto);
            await cacheClient.SetAsAsync(cacheKeyByCode, dto);
        }
        catch (Exception e)
        {
            logger.LogError(e, "New game created successfully, but failed to update cache");
        }
    }

}