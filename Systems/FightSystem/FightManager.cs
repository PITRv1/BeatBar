using Godot;
using System;

public partial class FightManager : Node
{
    public Variant winner;


    public void StartFightSquence()
    {
        
    }



    public void EndFightSquence()
    {
        SignalBus.Instance.EmitSignal(SignalBus.SignalName.FightEnded, winner);
        GD.Print("fightmanager   ,fight ended");
    }
}
