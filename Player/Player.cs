using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Player : CharacterBody3D
{
    [Export] private float walkSpeed = 300.0f;
    [Export] private float movementSmoothingFactor = 8.0f;

    [Export] private float mouseSens = 0.003f;

    [ExportGroup("PlayerParts")]
    [Export] private Camera3D playerCam;
    [Export] private InteractionInitiator interactionInitiator;
    [Export] public BeaterDataComponent beaterDataComponent;


    private BaseNpc currentOpponent;

    private Vector2 mouseInput;
    private States currentState = States.FREE;

    private int money = 0;

    public enum States
    {
        FREE,
        ENGAGING
    }


    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        Global.Instance.player = this;

        SignalBus.Instance.EngagementStarted += _OnEngagementStarted;
        SignalBus.Instance.EngagementEnded += _OnEngagementEnded;

    }


    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            mouseInput = mouseMotion.Relative;
        }
    }


    public void GiveMoney(int amount)
    {
        money += amount;
    }

    public void TakeMoney(int amount)
    {
        money -= amount;
    }


    private void _UpdateCameraRotation()
    {
        this.RotateY(-mouseInput.X * mouseSens);
        playerCam.RotateX(-mouseInput.Y * mouseSens);
        float clampedRotationX = Mathf.Clamp(playerCam.Rotation.X, Mathf.DegToRad(-90), Mathf.DegToRad(90));

        playerCam.Rotation = new Vector3(clampedRotationX, playerCam.Rotation.Y, playerCam.Rotation.Z);
        mouseInput = Vector2.Zero;
    }


    private void _UpdateMovement(double delta)
    {
        Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward").Normalized();
        Vector3 wishDir = this.GlobalTransform.Basis * new Vector3(inputDir.X, 0.0f, inputDir.Y);

        Vector3 newVel = Vector3.Zero;
        
        newVel.X = Mathf.Lerp(Velocity.X, wishDir.X * walkSpeed * (float)delta, (float)delta * movementSmoothingFactor);
        newVel.Z = Mathf.Lerp(Velocity.Z, wishDir.Z * walkSpeed * (float)delta, (float)delta * movementSmoothingFactor);

        if (!IsOnFloor())
        {
            newVel.Y = Velocity.Y - 16.0f * (float)delta;
            newVel.Y = Mathf.Max(newVel.Y, -60.0f);
        }
        
        Velocity = newVel;
        
        MoveAndSlide();
    }

    private void _LockCameraToPoint(Vector3 targetPos, double delta)
    {
        Vector3 direction = (targetPos - GlobalPosition);
        direction.Y = 0f;
        direction = direction.Normalized();

        if (direction.LengthSquared() > 0.001f)
        {
            Transform3D lookTransform = Transform.LookingAt(GlobalPosition + direction, Vector3.Up);
            Transform = Transform.InterpolateWith(lookTransform, (float)delta * 8.0f);
        }
        

        Vector3 camDirection = (targetPos - playerCam.GlobalPosition).Normalized();
        float horizontalDist = new Vector2(camDirection.X, camDirection.Z).Length();
        float targetXRotation = -Mathf.Atan2(camDirection.Y, horizontalDist);
        targetXRotation = Mathf.Clamp(targetXRotation, Mathf.DegToRad(-90), Mathf.DegToRad(90));

        float currentX = playerCam.Rotation.X;
        float newX = Mathf.Lerp(currentX, targetXRotation, (float)delta * 8.0f);
        playerCam.Rotation = new Vector3(newX, playerCam.Rotation.Y, playerCam.Rotation.Z);


        mouseInput = Vector2.Zero;
    }

    private void UpdateStateMachine(double delta)
    {
        switch (currentState)
        {
            case States.FREE:
                _UpdateCameraRotation();
                _UpdateMovement(delta);

            break;
            
            case States.ENGAGING:
                _LockCameraToPoint(currentOpponent.eyePosition.GlobalPosition, delta);
            break;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        UpdateStateMachine(delta);
    }

    private void _OnEngagementStarted(BaseNpc opponent)
    {
        if (opponent == null) return;

        currentOpponent = opponent;
        currentState = States.ENGAGING;
        Input.MouseMode = Input.MouseModeEnum.Visible;

        Velocity = Vector3.Zero;
        interactionInitiator.blocked = true;
    }

    
    private void _OnEngagementEnded(BaseNpc opponent)
    {
        currentOpponent = null;
        currentState = States.FREE;
        Input.MouseMode = Input.MouseModeEnum.Captured;

        interactionInitiator.blocked = false;

    }

}
