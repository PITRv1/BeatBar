using Godot;
using System;

public partial class InteractionInitiator : RayCast3D
{
    [Export] private float handReach {get; set;} = 4.0f;

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("interact"))
        {
            BeginInteractionAttempt();
        }
    }

    private void BeginInteractionAttempt()
    {
        InteractionReceiver collider = QueryInteractionSpace();

        if (collider != null)
        {
            collider.RecieverInteraction();
        }
    }

    private InteractionReceiver QueryInteractionSpace()
    {
        this.TargetPosition = new Vector3(0,0,-handReach);
        InteractionReceiver collider = (InteractionReceiver)GetCollider();

        return collider;
    }


}
