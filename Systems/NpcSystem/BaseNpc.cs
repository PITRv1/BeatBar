using DialogueManagerRuntime;
using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

public partial class BaseNpc : Node3D
{
    [Signal] public delegate void BeatInputedEventHandler();
    

    [Export] public Marker3D fightPlayerPosition {get;set;} // this is where the player will be moved when fighting the specific goon

    [Export] public BeaterDataComponent beaterDataComponent {get;set;}
    [Export] public Label3D nameLabel {get; set;}
    [Export] public Node3D visualsContainer {get; set;}
    [Export] public Sprite3D beaterSprite {get; set;}
    [Export] public Marker3D eyePosition {get; set;}

    [Export] private float watchDistance {get; set;} = 5.0f;

    Tween tween;
    public Timer fightCountdownTimer;

    private float beatTimer;
    private bool canBeat = false;

    public override void _Ready()
    {
        nameLabel.Text = beaterDataComponent.beaterData.beaterName;
        
        fightCountdownTimer = new Timer();
        AddChild(fightCountdownTimer);

        fightCountdownTimer.WaitTime = 3.0f;
        fightCountdownTimer.OneShot = true;

        fightCountdownTimer.Timeout += () => {canBeat = true;};
    }


    private void DisengagePlayer()
    {
        SignalBus.Instance.EmitSignal(SignalBus.SignalName.EngagementEnded, this);
    }

    private void EngagePlayer()
    {
        SignalBus.Instance.EmitSignal(SignalBus.SignalName.EngagementStarted, this);

        DialogueManager.ShowDialogueBalloon(
            beaterDataComponent.beaterData.introDialogueResource,
            "start",
            [this, new Godot.Collections.Dictionary { 
                {"data", beaterDataComponent.beaterData }
                }
            ]
        );
    }


    private void StartFight()
    {
        SignalBus.Instance.EmitSignal(SignalBus.SignalName.FightStarted, this);
        
        ResetTween();
        tween.TweenProperty(
            Global.Instance.player,
            "global_position",
            fightPlayerPosition.GlobalPosition,
            1.0
        )
        .SetTrans(Tween.TransitionType.Cubic)
        .SetEase(Tween.EaseType.InOut);

        fightCountdownTimer.Start();
    }

    public void FinishFight(bool didWin)
    {
        canBeat = false;

        DialogueManager.ShowDialogueBalloon(
            beaterDataComponent.beaterData.outroDialogueResource,
            "start",
            [this, new Godot.Collections.Dictionary { 
                {"data", beaterDataComponent.beaterData },
                {"endCondition", didWin}

                }
            ]
        );
    }

    public void Beat(double delta)
    {
        if (beatTimer <= 0.0f)
        {
            beatTimer = beaterDataComponent.beaterData.beatSpeed;
            EmitSignal(SignalName.BeatInputed);
        }
        beatTimer -= (float)delta;
    }

    private void UpdateSpriteBillboard()
    {
                if (GlobalPosition.DistanceTo(Global.Instance.player.GlobalPosition) <  watchDistance)
        {
            beaterSprite.Billboard = BaseMaterial3D.BillboardModeEnum.FixedY;
        }
        else
        {
            beaterSprite.Billboard = BaseMaterial3D.BillboardModeEnum.Disabled;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        UpdateSpriteBillboard();

        if (canBeat)
        {
            
            Beat(delta);
        }
    }


    private void ResetTween()
    {
        if (tween != null) tween.Kill();   
        tween = CreateTween();
    }

    public void GivePlayerMoney()
    {
        Global.Instance.player.GiveMoney(beaterDataComponent.beaterData.reward);
    }
}
