using Godot;
using System;
using System.ComponentModel;

public partial class InteractionInitiator : RayCast3D
{
    [Signal] public delegate void HoveringInteractableEventHandler(InteractionReceiver newCollider);
    [Signal] public delegate void HoveringEndedEventHandler();


    [Export] private float handReach {get; set;} = 4.0f;
    [Export] public bool blocked {get; set;} = false;

    [Export] public bool continousCheck {get; set;}

    GodotObject collider;

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("interact") && !blocked)
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

    public override void _PhysicsProcess(double delta)
    {
        if (!continousCheck)return;

        if (collider != GetCollider())
        {
            EmitSignal(SignalName.HoveringEnded);
        }

        if (GetCollider() is InteractionReceiver currInteractable)
        {
            EmitSignal(SignalName.HoveringInteractable, currInteractable);
        }

        collider = GetCollider();
    }


}
