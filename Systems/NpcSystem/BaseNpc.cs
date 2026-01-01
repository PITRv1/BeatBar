using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

public partial class BaseNpc : Node3D
{

    [ExportGroup("NPC properties")]
    [Export] public string npcName { get; set; }
    
    [Export] public Godot.Collections.Array<string> introMessageOptions { get; set; }
    [Export] public Godot.Collections.Array<string> outroMessageOptions { get; set; }

    [Export] public int reward {get; set;}
    
    [Export] public float damage {get; set;}
    [Export] public float damageMultiplier {get; set;}
    
    /// Time between beats in seconds
    [Export] public float beatSpeed {get; set;}

    [ExportGroup("NPC components")]
    [Export] public Label3D nameLabel {get; set;}
    [Export] public Node3D visualsContainer {get; set;}

    public override void _Ready()
    {
        nameLabel.Text = npcName;
    }

    private void DisengagePlayer()
    {
        
    }

    private void EngagePlayer()
    {
        GD.Print("you gonna get it buster");
    }

    private void StartFight()
    {
        
    }
}
