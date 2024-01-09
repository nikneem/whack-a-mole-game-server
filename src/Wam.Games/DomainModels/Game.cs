using HexMaster.DomainDrivenDesign;
using HexMaster.DomainDrivenDesign.ChangeTracking;
using Wam.Core.Enums;
using Wam.Core.ExtensionMethods;

namespace Wam.Games.DomainModels;

public class Game : DomainModel<Guid>
{
    private readonly List<Player> _players;
    public string Code { get; }
    public GameState State { get; private set; }
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
    public void ChangeState(GameState value)
    {
        if (State.CanChangeTo(value))
        {
            State = value;
            SetState(TrackingState.Modified);
        }
        else
        {
            throw new InvalidOperationException($"Cannot change state from {State.Code} to {value.Code}");
        }
    }

    public Game(Guid id,
        string code,
        string state,
        DateTimeOffset createdOn,
        DateTimeOffset? startedOn = null,
        DateTimeOffset? finishedOn = null,
        List<Player>? players = null) : base(id)
    {
        Code = code;
        State = GameState.FromCode(state);
        CreatedOn = createdOn;
        StartedOn = startedOn;
        FinishedOn = finishedOn;
        _players = players ?? [];
    }

    public Game() : base(Guid.NewGuid(), TrackingState.New)
    {
        Code = StringExtensions.GenerateGameCode();
        CreatedOn = DateTimeOffset.UtcNow;
        State = GameState.New;
        _players = [];
    }
}