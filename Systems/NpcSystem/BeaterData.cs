using Godot;
using System;

[GlobalClass]
public partial class BeaterData : Resource
{
    [Export] public string beaterName { get; set; }

    [Export] public int reward {get; set;} = 10;
    
    [Export] public float damage {get; set;} = 1f;
    [Export] public float damageMultiplier {get; set;} = 0f;
    [Export] public float health {get; set;} = 100f;

    
    /// Time between beats in seconds
    [Export] public float beatSpeed {get; set;} = .2f;

    [Export] public Resource introDialogueResource;
    [Export] public Resource outroDialogueResource;


}
