using Godot;
using GodotPlugins.Game;
using System;

public partial class GameManager : Node
{
    [Export] public AnimationPlayer fightEffectAniamtor;
    [Export] public AnimationPlayer transitionAnimator;
    
    [Export] public Control gui;
    [Export] public Control mainMenu;
    [Export] public Node mapHolder;

    private static readonly PackedScene fightScene = GD.Load<PackedScene>("res://Systems/FightSystem/FightScene.tscn");
    private static readonly PackedScene mapScene = GD.Load<PackedScene>("res://Map/map.tscn");


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
        
        transitionAnimator.Play("fadeInGame");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("menu"))
        {
            if (mainMenu.Visible == false)
            {
                _ShowMainMenu();
            } else
            {
                _HideMainMenu();
            }
        }
    }



    public async void LoadFightScene(BaseNpc opponent)
    {
        transitionAnimator.Play("fade");

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

        transitionAnimator.PlayBackwards("fade");

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

    public void _ShowMainMenu()
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;
        transitionAnimator.Play("fadeMainMenu");
        // Engine.TimeScale = 0.0f;


    }

    public void _HideMainMenu()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        transitionAnimator.PlayBackwards("fadeMainMenu");
        
    }

    public void ResetMainScene()
    {
        mapHolder.GetChild(0).QueueFree();
        Node3D newMap = mapScene.Instantiate<Node3D>();
        mapHolder.AddChild(newMap);
    }

    public void _ExitGame()
    {
        GetTree().Quit();
    }
}
