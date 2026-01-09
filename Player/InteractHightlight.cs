using Godot;
using System;
using System.ComponentModel;

public partial class InteractHightlight : Control
{
    [Export] public Control itemHighlighterLeft {get;set;}
    [Export] public Control itemHighlighterRight {get;set;}
    [Export] public TextureRect prompt {get;set;}


    [Export] public InteractionInitiator interactionInitiator;



    public override void _Ready()
    {
        interactionInitiator.HoveringInteractable += _OnNewItemHovered;
        interactionInitiator.HoveringEnded += _OnHoveringEnded;
    }

    private void _OnNewItemHovered(InteractionReceiver newItem)
    {
        AabbToScreenCorners(newItem.geometryInstance.GetAabb(), newItem.geometryInstance.GlobalTransform, GetViewport().GetCamera3D());
    
        itemHighlighterLeft.Visible = true;
        itemHighlighterRight.Visible = true;
        prompt.Visible = true;
    }

    private void _OnHoveringEnded()
    {
        itemHighlighterLeft.Visible = false;
        itemHighlighterRight.Visible = false;
        prompt.Visible = false;

    }


    private void AabbToScreenCorners(Aabb aabb, Transform3D globalTransform, Camera3D camera)
    {
        Vector2 min = new(float.MaxValue, float.MaxValue);
        Vector2 max = new(float.MinValue, float.MinValue);
        Vector2 mid = Vector2.Zero;

        for (int x = 0; x <= 1; x++)
        for (int y = 0; y <= 1; y++)
        for (int z = 0; z <= 1; z++)
        {
            Vector3 corner =
                aabb.Position + aabb.Size * new Vector3(x, y, z);

            Vector3 world = globalTransform * corner;
            Vector2 screen = camera.UnprojectPosition(world);

            min = min.Min(screen);
            max = max.Max(screen);
        }

        itemHighlighterLeft.Position = min;
        itemHighlighterRight.Position = max;  
        prompt.Position = (min + max) * 0.5f;
    }

}
