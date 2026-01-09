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
    [Export] public AnimationPlayer fightAnimator {get; set;}



    [Export] private float watchDistance {get; set;} = 5.0f;
    [ExportGroup("Audio")]
    [Export] public AudioStreamPlayer3D beatAudioPlayer {get; set;}
    [Export] public AudioStreamPlayer3D winAudioPlayer {get; set;}
    [Export] public AudioStreamPlayer3D loseAudioPlayer {get; set;}
    [Export] public AudioStreamPlayer3D botherAudioPlayer {get; set;}
    [Export] public AudioStreamPlayer3D byeAudioPlayer {get; set;}
    [Export] public AudioStreamPlayer3D fightAudioPlayer {get; set;}






    [ExportGroup("Optional for bouncers")]
    [Export] public StaticBody3D blockingWall;
    [Export] public bool useDoorAnim = false;
    [Export] public AnimationPlayer doorAnimator;


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

        beatAudioPlayer.ProcessMode = ProcessModeEnum.Inherit;
        winAudioPlayer.ProcessMode = ProcessModeEnum.Inherit;
        loseAudioPlayer.ProcessMode = ProcessModeEnum.Inherit;
        botherAudioPlayer.ProcessMode = ProcessModeEnum.Inherit;
        byeAudioPlayer.ProcessMode = ProcessModeEnum.Inherit;
        fightAudioPlayer.ProcessMode = ProcessModeEnum.Inherit;
    }


    private void DisengagePlayer()
    {
        SignalBus.Instance.EmitSignal(SignalBus.SignalName.EngagementEnded, this);

        byeAudioPlayer.Play();
    }

    private void EngagePlayer()
    {
        SignalBus.Instance.EmitSignal(SignalBus.SignalName.EngagementStarted, this);

        botherAudioPlayer.Play();

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
        
        fightAudioPlayer.Play();

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

        if (didWin)
        {
            winAudioPlayer.Play();
        }
        else
        {
            loseAudioPlayer.Play();
        }
    }

    public void Beat(double delta)
    {
        if (beatTimer <= 0.0f)
        {
            beatTimer = beaterDataComponent.beaterData.beatSpeed + GD.Randf() * 0.25f;

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

    public void PlayLoseAnim()
    {
        fightAnimator.Play("loseAnim");
    }

    public void PlayWinAnim()
    {
        fightAnimator.Play("winAnim");
    }

    public void PlayGoonDamageSound()
    {
        beatAudioPlayer.Play();
    }


    public void DisableWall()
    {
        blockingWall.QueueFree();
    
        if (useDoorAnim)
        {
            doorAnimator.Play("OpenDoors");
        }
    }
}
