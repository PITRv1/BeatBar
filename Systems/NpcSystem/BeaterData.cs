using Godot;
using System;

[GlobalClass]
public partial class BeaterData : Resource
{
    [Export] public string beaterName { get; set; }

    [Export] public int reward {get; set;}
    
    [Export] public float damage {get; set;}
    [Export] public float damageMultiplier {get; set;}
    
    /// Time between beats in seconds
    [Export] public float beatSpeed {get; set;}

    [Export] public Resource introDialogueResource;
    [Export] public Resource outroDialogueResource;


}
