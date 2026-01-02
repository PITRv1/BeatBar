using Godot;
using System;

public partial class Global : Node
{
    public static Global Instance { get; private set; }
    public Player player;

    public override void _EnterTree()
    {
        Instance = this;
    }
}
