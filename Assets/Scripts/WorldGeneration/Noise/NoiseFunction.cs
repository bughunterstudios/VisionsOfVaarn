using UnityEngine;

public abstract class NoiseFunction : MonoBehaviour
{
    private void Awake()
    {
        Initialize();
    }

    public abstract void Initialize();

    public abstract float GetValue(float currentvalue, float smoothingvalue, float x, float y);

    public abstract Color Coloring(Color currentcolor, float smoothingvalue, float x, float y);
}
