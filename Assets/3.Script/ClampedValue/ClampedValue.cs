using System;

public abstract class ClampedValue<T> where T : struct, IComparable<T>
{
    public class Callback
    {
        /// <summary>
        /// (T prev, T current)
        /// </summary>
        public Action<T,T> OnValueChanged;
        
        /// <summary>
        /// (T prev, T amount)
        /// </summary>
        public Action<T,T> OnIncreased;
        
        /// <summary>
        /// (T prev, T amount)
        /// </summary>
        public Action<T,T> OnDecreased;
        
        /// <summary>
        /// (T prev, T current)
        /// </summary>
        public Action<T,T> OnMinReached;
        
        /// <summary>
        /// (T prev, T current)
        /// </summary>
        public Action<T,T> OnMaxReached;
    }
    
    public Callback Events { get; protected set; } = new Callback();

    public T Min { get; protected set; }
    public T Max { get; protected set; }
    public T Current { get; protected set; }
    
    public abstract float Ratio { get; }
    
    public void ResetToMin() => Set(Min);
    public void ResetToMax() => Set(Max);

    public bool IsMin => Current.CompareTo(Min) == 0;
    public bool IsMax => Current.CompareTo(Max) == 0;
    
    public void Increase(T amount)
    {
        T prev = Current;
        Set(Add(prev, amount));
        Events.OnIncreased?.Invoke(prev, amount);
    }

    public void Decrease(T amount)
    {
        T prev = Current;
        Set(Subtract(prev, amount));
        Events.OnDecreased?.Invoke(prev, amount);
    }

    protected Func<T, T, float, T> LerpFunc;

    public void Lerp(float t)
    {
        if (LerpFunc == null)
            throw new InvalidOperationException("LerpFunc is not set for this ClampedValue type");

        Set(LerpFunc(Min, Max, t));
    }
    
    public void SetMinMax(T newMin, T newMax)
    {
        if (newMin.CompareTo(newMax) > 0)
        {
            throw new ArgumentException ($"SetMinMax: Min({newMin}) >>>> Max({newMax})");
        }
        
        Min = newMin;
        Max = newMax;
        Set(Current); //범위 확인 용도
    }

    protected ClampedValue(T min, T max, T initial)
    {
        Min = min;
        Max = max;
        Current = Clamp(initial, min, max);
    }
    
    protected void Set(T value)
    {
        value = Clamp(value, Min, Max);
        if (Current.CompareTo(value) == 0) return;

        T prev = Current;
        Current = value;

        Events.OnValueChanged?.Invoke(prev, Current);
        if (Current.CompareTo(Min) == 0) Events.OnMinReached?.Invoke(prev, Current);
        if (Current.CompareTo(Max) == 0) Events.OnMaxReached?.Invoke(prev, Current);
    }
    protected abstract T Add(T a, T b);
    protected abstract T Subtract(T a, T b);
    protected abstract T Clamp(T value, T min, T max);
}