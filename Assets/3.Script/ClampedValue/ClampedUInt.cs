using UnityEngine;

public class ClampedUInt : ClampedValue<uint>
{
    public ClampedUInt(uint min, uint max, uint initial)
        : base(min, max, initial)
    {
        LerpFunc = (minV, maxV, t) => (uint)Mathf.Lerp(minV, maxV, t);
    }

    public override float Ratio => (float)(Current - Min) / (float)(Max - Min);
    
    protected override uint Add(uint a, uint b) => a + b;
    protected override uint Subtract(uint a, uint b)
    {
        if (a < b) return Min; // 언더플로 방지용
        return a - b;
    }
    protected override uint Clamp(uint value, uint min, uint max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}