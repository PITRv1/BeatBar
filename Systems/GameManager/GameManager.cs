using Godot;
using System;

public partial class GameManager : Node
{
    [Export] public AnimationPlayer fightEffectAniamtor;
    [Export] public AnimationPlayer transitionAnimator;
    
    [Export] public Control gui;

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


    public async void ChangeGuiScene(string newScenePath)
    {
        
        
        if (currentGuiScene != null)
        {
            currentGuiScene.QueueFree();
        }

        var scene = GD.Load<PackedScene>(newScenePath);
        Control newScene = scene.Instantiate<Control>();

        gui.AddChild(newScene);
        currentGuiScene = newScene;


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
    }

    private void _OnFightEnded(Variant opponent)
    {
        fightEffectAniamtor.PlayBackwards("fightEngage");
    }


}
