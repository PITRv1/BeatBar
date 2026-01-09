using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Player : CharacterBody3D
{
    [Signal] public delegate void BeatInputedEventHandler();

    [Export] private float walkSpeed = 300.0f;
    [Export] private float movementSmoothingFactor = 8.0f;

    [Export] private float mouseSens = 0.003f;

    [ExportGroup("PlayerParts")]
    [Export] public Camera3D playerCam;
    [Export] public InteractionInitiator interactionInitiator;
    [Export] public BeaterDataComponent beaterDataComponent;
    [Export] public Label moneyDisplay;
    [Export] public AudioStreamPlayer walkAudioPlayer; 

    private BaseNpc currentOpponent;

    private Vector2 mouseInput;
    private States currentState = States.FREE;

    public int money = 0;

    private float headbobTime = 0f;
    private const float headbobFrequency =  1.5f;
    private const float headbobMoveAmount = 0.08f;



    public enum States
    {
        FREE,
        ENGAGING
    }


    public override void _Ready()
    {
        Global.Instance.player = this;

        SignalBus.Instance.EngagementStarted += _OnEngagementStarted;
        SignalBus.Instance.EngagementEnded += _OnEngagementEnded;

        moneyDisplay.Text = "$  " + Convert.ToString(money);
    }

    public override void _ExitTree()
    {
        SignalBus.Instance.EngagementStarted -= _OnEngagementStarted;
        SignalBus.Instance.EngagementEnded -= _OnEngagementEnded;

    }


    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            mouseInput = mouseMotion.Relative;
        }

        if (@event.IsActionPressed("interact")){
            EmitSignal(SignalName.BeatInputed);
        }
    }


    public void GiveMoney(int amount)
    {
        money += amount;
        moneyDisplay.Text = "$  " + Convert.ToString(money);

    }

    public void TakeMoney(int amount)
    {
        money -= amount;
        moneyDisplay.Text = "$  " + Convert.ToString(money);

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

    private void _UpdateCameraBob(double delta)
    {
        headbobTime += (float)delta * Velocity.Length();
    
        Vector3 targetPosition = new Vector3(
            Mathf.Cos(headbobTime * (headbobFrequency) * 0.5f) * headbobMoveAmount,
            Mathf.Sin(headbobTime * (headbobFrequency)) * headbobMoveAmount,
            0
        );
        
        playerCam.Transform = playerCam.Transform with 
        { 
            Origin = playerCam.Transform.Origin.Lerp(targetPosition, (float)delta * 10.0f) 
        };



        // if (playerCam.Position.Y < -0.06 && walkAudioPlayer.Playing == false )
        // {
            
        //     walkAudioPlayer.Play();
        // }
    }

    private void UpdateStateMachine(double delta)
    {
        switch (currentState)
        {
            case States.FREE:
                if (Input.MouseMode == Input.MouseModeEnum.Visible) return;

                _UpdateCameraRotation();
                _UpdateMovement(delta);
                _UpdateCameraBob(delta);

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
