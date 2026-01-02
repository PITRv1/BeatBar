using DialogueManagerRuntime;
using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

public partial class BaseNpc : Node3D
{
    [Export] public BeaterDataComponent beaterDataComponent {get;set;}
    [Export] public Label3D nameLabel {get; set;}
    [Export] public Node3D visualsContainer {get; set;}
    [Export] public Sprite3D beaterSprite {get; set;}
    [Export] public Marker3D eyePosition {get; set;}


    [Export] private float watchDistance {get; set;} = 5.0f;

    public override void _Ready()
    {
        nameLabel.Text = beaterDataComponent.beaterData.beaterName;
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
    }

}
