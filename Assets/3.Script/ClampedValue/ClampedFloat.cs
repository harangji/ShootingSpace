using UnityEngine;

public class ClampedFloat : ClampedValue<float>
{
    public ClampedFloat(float min, float max, float initial)
        : base(min, max, initial)
    {
        LerpFunc = Mathf.Lerp;
    }
    
    public override float Ratio => (Current - Min) / (Max - Min);
    protected override float Clamp(float value, float min, float max) => Mathf.Clamp(value, min, max);
    protected override float Add(float a, float b) => a + b;
    protected override float Subtract(float a, float b) => a - b;
}