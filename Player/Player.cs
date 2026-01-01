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

    Vector2 mouseInput;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        GD.Print("HEllo world!");
    }


    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            mouseInput = mouseMotion.Relative;
        }
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
        
        Velocity = newVel;
         
        MoveAndSlide();
    }


    public override void _PhysicsProcess(double delta)
    {
        _UpdateCameraRotation();
        _UpdateMovement(delta);
    }

}
