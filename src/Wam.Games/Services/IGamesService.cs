using Wam.Games.DataTransferObjects;

namespace Wam.Games.Services;

public interface IGamesService
{
    Task<GameDetailsDto> Get(Guid id, CancellationToken cancellationToken);
    Task<GameDetailsDto> GetByCode(string code, CancellationToken cancellationToken);
    Task<GameDetailsDto> Create(CancellationToken cancellationToken);
    
    Task<GameDetailsDto> Join(string code, Guid userId, CancellationToken cancellationToken);
    Task<GameDetailsDto> Leave(Guid gameId, Guid userId, CancellationToken cancellationToken);

    Task<GameDetailsDto> UpdateState(Guid gameId, string state, CancellationToken cancellationToken);

}