using UnityEngine;

public class FlattenNoise : NoiseFunction
{
    public AnimationCurve FlatCurve;
    private static AnimationCurve FlatCurve_private;

    public override void Initialize()
    {
        FlatCurve_private = FlatCurve;
    }

    public override float GetValue(float currentvalue, float smoothingvalue, float x, float y)
    {
        return Mathf.Lerp(currentvalue, currentvalue * 0.2f, FlatCurve_private.Evaluate(smoothingvalue));
    }

    public override Color Coloring(Color currentcolor, float smoothingvalue, float x, float y)
    {
        int chosen_val = SecondRegion.Regions_value_next_level(x, y, 100, 100);

        Color toreturn = currentcolor;

        if (chosen_val < 20)
            toreturn.a = NoiseControl.FloatLerp(currentcolor.a, 0.75f, smoothingvalue);

        return toreturn;
    }
}
