using Godot;
using System;

public partial class GameManager : Node
{
    [Export] public AnimationPlayer fightEffectAniamtor;
    [Export] public AnimationPlayer transitionAnimator;
    
    [Export] public Control gui;

    private static readonly PackedScene fightScene = GD.Load<PackedScene>("res://Systems/FightSystem/FightScene.tscn");

    public Control currentGuiScene = null;


    public override void _Ready()
    {
        Global.Instance.gameManager = this;

        SignalBus.Instance.FightStarted += _OnFightStarted;
        SignalBus.Instance.FightEnded += _OnFightEnded;

        if (gui.GetChildCount() > 0)
        {
    		currentGuiScene = gui.GetChild<Control>(0);
        }
        
    }


    public async void LoadFightScene(BaseNpc opponent)
    {
                
        if (currentGuiScene != null)
        {
            currentGuiScene.QueueFree();
        }

        FightManager newFightScene = fightScene.Instantiate<FightManager>();
        
        newFightScene.playerOne = Global.Instance.player;
        newFightScene.playerTwo = opponent;

        newFightScene.SetupFight();

        gui.AddChild(newFightScene);
        currentGuiScene = newFightScene;

    }

    public async void ClearCurrentGuiScene()
    {
        if (currentGuiScene != null)
        {
            currentGuiScene.QueueFree();
            currentGuiScene = null;
        }
    }


    private void _OnFightStarted(BaseNpc opponent)
    {
        fightEffectAniamtor.Play("fightEngage");
        LoadFightScene(opponent);
    }

    private void _OnFightEnded(BaseNpc opponent, Variant winner)
    {
        fightEffectAniamtor.PlayBackwards("fightEngage");
        ClearCurrentGuiScene();

        opponent.FinishFight( winner.AsGodotObject() == opponent ); // if the winner and the opponent are the same the did win contion of the opponent is gonna be true.
    }


}
