using Godot;
using System;

public partial class VolumeSlider : HSlider
{
    [Export] public string busName;

    int busIndex;

    public override void _Ready()
    {
        busIndex = AudioServer.GetBusIndex(busName);
        ValueChanged += _OnValueChanged;

        this.Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(busIndex));

    }

    public void _OnValueChanged(double Value)
    {
        AudioServer.SetBusVolumeDb(busIndex, Mathf.LinearToDb((float)Value));
    }

}
