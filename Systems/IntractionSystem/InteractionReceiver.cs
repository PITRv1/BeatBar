using Godot;
using System;

public partial class InteractionReceiver : Area3D
{
    [Signal]
    public delegate void InteractionReceivedEventHandler();

    public void RecieverInteraction()
    {
        EmitSignal(SignalName.InteractionReceived);
    }

}
