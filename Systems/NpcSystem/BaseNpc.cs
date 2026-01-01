using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

public partial class BaseNpc : Node3D
{

    [ExportGroup("NPC properties")]
    [Export] public string npcName { get; set; }
    
    [Export(PropertyHint.MultilineText)] public string introMessage { get; set; } 
    [Export(PropertyHint.MultilineText)] public string outroMessage { get; set; }

    [Export] public int reward {get; set;}
    
    [Export] public float damage {get; set;}
    [Export] public float damageMultiplier {get; set;}
    
    /// Time between beats in seconds
    [Export] public float beatSpeed {get; set;}

    [ExportGroup("NPC components")]
    [Export] public Label3D nameLabel {get; set;}
    [Export] public Node3D visualsContainer {get; set;}



    private void DisengagePlayer()
    {
        
    }

    private void EngagePlayer()
    {
        
    }

    private void StartFight()
    {
        
    }
}
