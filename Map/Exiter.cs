using Godot;
using System;

public partial class Exiter : Area3D
{
    public override void _Ready()
    {
        BodyEntered += GetOut;
    }


    public void GetOut(Node3D _)
    {
        if (_ is Player)
        {
            GetTree().Quit();        
        }
    }
}
