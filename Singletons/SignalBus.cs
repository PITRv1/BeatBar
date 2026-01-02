using Godot;
using System;

public partial class SignalBus : Node
{
    public static SignalBus Instance { get; private set; }

    [Signal] public delegate void EngagementStartedEventHandler(BaseNpc opponent);
    [Signal] public delegate void EngagementEndedEventHandler(BaseNpc opponent);

    [Signal] public delegate void FightStartedEventHandler(BaseNpc opponent);
    [Signal] public delegate void FightEndedEventHandler(BaseNpc opponent, Variant winner);



    public override void _EnterTree()
    {
        Instance = this;
    }
}
