using HexMaster.DomainDrivenDesign;
using HexMaster.DomainDrivenDesign.ChangeTracking;

namespace Wam.Games.DomainModels;

public class Game : DomainModel<Guid>
{

    private List<Player> _players;

    public DateTimeOffset CreatedOn { get; private set; }
    public DateTimeOffset? StartedOn { get; private set; }
    public DateTimeOffset? FinishedOn { get; private set; }
    public IReadOnlyList<Player> Players => _players.AsReadOnly();

    public void AddPlayer(Player player)
    {
        if (_players.Any(p => p.Id == player.Id))
        {
            throw new InvalidOperationException("Player already added");
        }

        _players.Add(player);
        SetState(TrackingState.Modified);
    }

    public void RemovePlayer(Player player)
    {
        if (_players.Contains(player))
        {
            throw new InvalidOperationException("Player not found");
        }

        _players.Remove(player);
        SetState(TrackingState.Modified);
    }

    public Game(Guid id,
        DateTimeOffset createdOn,
        DateTimeOffset? startedOn = null,
        DateTimeOffset? finishedOn = null,
        List<Player> players = null) : base(id)
    {
        _players = new List<Player>();
    }
}