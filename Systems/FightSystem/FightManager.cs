using Godot;
using System;

public partial class FightManager : Control
{
    public Player playerOne;
    public BaseNpc playerTwo;

    public Variant winner;

    public void SetupFight()
    {
        
    }

    public void StartFightSquence()
    {
        
    }


    public void SetWinnerPlayer(){ winner = playerOne; EndFightSquence();}
    public void SetWinnerOpponent(){ winner = playerTwo; EndFightSquence();}



    public void EndFightSquence()
    {
        SignalBus.Instance.EmitSignal(SignalBus.SignalName.FightEnded, playerTwo, winner);
    }


}
