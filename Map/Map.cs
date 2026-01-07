using Godot;
using System;

public partial class Map : Node3D
{
    [Export] public AnimationPlayer mapAnimator {get;set;}

    public void OpenBarDoors()
    {
        mapAnimator.Play("OpenDoors");
    }
}
