using Godot;
using System;

public partial class Piano : AudioStreamPlayer3D
{
    public void _OnTrackFinished()
    {
        Play();
    }
}
