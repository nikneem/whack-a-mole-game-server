using System.Diagnostics;

namespace Wam.Core.Enums;

public abstract class GameState
{

    public static readonly GameState New;
    public static readonly GameState Current;
    public static readonly GameState Started;
    public static readonly GameState Finished;
    public static readonly GameState Cancelled;
    public static readonly GameState[] All;

    public static GameState FromCode(string code)
    {
        var state = All.FirstOrDefault(s => s.Code == code);
        if (state == null)
        {
            throw new InvalidOperationException($"Invalid game state code {code}");
        }
        return state;
    }

    public abstract string Code { get; }
    public virtual string TranslationKey => $"Game.States.{Code}";

    public virtual bool CanChangeTo(GameState state) => false;

    static GameState()
    {
        All = new[]
        {
            New = new GameStateNew() , 
            Current = new GameStateCurrent(),
            Started = new GameStateStarted(),
            Finished = new GameStateFinished(),
            Cancelled = new GameStateCancelled()
        };
    }
}

public class GameStateNew : GameState
{
    public override string Code => "New";
    public override bool CanChangeTo(GameState state)
    {
        return state == Current || state == Cancelled;
    }
}

public class GameStateCurrent : GameState
{
    public override string Code => "Current";

    public override bool CanChangeTo(GameState state)
    {
        return state == Started || state == Cancelled;
    }
}

public class GameStateStarted : GameState
{
    public override string Code => "Started";

    public override bool CanChangeTo(GameState state)
    {
        return state == Finished || state == Cancelled;
    }
}

public class GameStateFinished : GameState
{
    public override string Code => "Finished";
}

public class GameStateCancelled : GameState
{
    public override string Code => "Cancelled";
}