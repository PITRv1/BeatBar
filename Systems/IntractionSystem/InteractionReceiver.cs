using Godot;
using System;

public partial class InteractionReceiver : Area3D
{
    [Signal]
    public delegate void InteractionReceivedEventHandler();

    [Export] public GeometryInstance3D geometryInstance;

    public void RecieverInteraction()
    {
        EmitSignal(SignalName.InteractionReceived);
        
    }


}
