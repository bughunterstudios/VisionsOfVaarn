using UnityEngine;

public class NoNoise : NoiseFunction
{
    public override void Initialize()
    {
    }

    public override float GetValue(float currentvalue, float smoothingvalue, float x, float y)
    {
        return currentvalue;
    }

    public override Color Coloring(Color currentcolor, float smoothingvalue, float x, float y)
    {
        return currentcolor;
    }
}
