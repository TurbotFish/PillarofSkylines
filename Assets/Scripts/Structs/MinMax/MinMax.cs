using UnityEngine;

[System.Serializable]
public class MinMax {
    public float min, max;

    /// <summary>
    /// A Vector2 containing a "minimum" value and a "maximum" value.
    /// </summary>
    /// <param name="min"> The minimum value.</param>
    /// <param name="max"> The maximum value.</param>
    public MinMax(float min, float max) {
        this.min = min;
        this.max = max;
    }

    /// <summary>
    /// A MinMax with switched min and max values.
    /// </summary>
    /// <returns> The inverted MinMax. </returns>
    public MinMax MaxMin {
        get { return new MinMax(max, min); }
    }

    /// <summary>
    /// Switches the min and max values.
    /// </summary>
    public void Inverse() {
        float tmp = min;
        min = max;
        max = tmp;
    }

    /// <summary>
    /// Clamps a value between Min and Max.
    /// </summary>
    /// <param name="value"> The value to clamp with the MinMax. </param>
    /// <returns> The clamped value. </returns>
    public float Clamp(float value) {
        return Mathf.Clamp(value, min, max);
    }

    /// <summary>
    /// Linearly interpolates between Min and Max by t.
    /// </summary>
    /// <param name="t"> The interpolation value between Min and Max. </param>
    /// <returns> The interpolated value. </returns>
    public float Lerp(float t) {
        return Mathf.Lerp(min, max, t);
    }

    /// <summary>
    /// The quotient of Min by Max.
    /// </summary>
    public float Quotient {
        get { return min / max; }
    }
}