using UnityEngine;

public class TieredNoise : NoiseFunction
{
    public float MultiplyVal;
    private static float MultiplyVal_private;

    public AnimationCurve TieredCurve;
    private static AnimationCurve TieredCurve_private;

    public AnimationCurve TieredCurve2;
    private static AnimationCurve TieredCurve2_private;

    public override void Initialize()
    {
        TieredCurve_private = TieredCurve;
        TieredCurve2_private = TieredCurve2;
        MultiplyVal_private = MultiplyVal;
    }

    public override float GetValue(float currentvalue, float smoothingvalue, float x, float y)
    {
        float val = Mathf.RoundToInt(currentvalue * MultiplyVal_private) / MultiplyVal_private;
        float diff = Mathf.Abs(val - currentvalue) * MultiplyVal_private * 2;
        return Mathf.Lerp(currentvalue, Mathf.Lerp(currentvalue, val, TieredCurve2_private.Evaluate(diff)), TieredCurve_private.Evaluate(smoothingvalue));
    }

    public override Color Coloring(Color currentcolor, float smoothingvalue, float x, float y)
    {
        int chosen_val = SecondRegion.Regions_value_next_level(x, y, 100, 100);
        float alpha = 0.75f;
        if (chosen_val < 50)
            alpha = 0.50f;
        if (chosen_val < 10)
            alpha = 0.25f;
        if (chosen_val < 3)
            alpha = 0f;

        Color toreturn = Color.Lerp(currentcolor, BandedColor.GetColor(0, x, y, false), smoothingvalue);
        toreturn.a = NoiseControl.FloatLerp(currentcolor.a, alpha, smoothingvalue);

        return toreturn;
    }
}
