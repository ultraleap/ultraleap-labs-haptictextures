using System;
using Vector3 = System.Numerics.Vector3;

/* This script contains the parameters and calculations for the generation of a "Circle" haptic sensation.
The available params are set within the HapticRunner and HapticRenderer scripts. */

public class HapticCircle
{
    public Vector3 Position = Vector3.Zero;
    public float Intensity = 0;
    public float Radius = 0.02f;
    public float Frequency = 0;

    public Vector3 EvaluateAt(double seconds)
    {
        Vector3 result = new Vector3
        (
            // Calculate the x and y positions of the circle and set the height
            (float)Math.Cos(2 * Math.PI * Frequency * seconds) * Radius,
            (float)Math.Sin(2 * Math.PI * Frequency * seconds) * Radius,
            0
        );

        result.X += Position.X;
        result.Y += Position.Y;
        result.Z = Position.Z;
        return result;
    }

    internal void SetPosition(UnityEngine.Transform position1)
    {
        Position = new Vector3
            (
                position1.position.x,
                position1.position.z,
                position1.position.y
            );
    }
}
