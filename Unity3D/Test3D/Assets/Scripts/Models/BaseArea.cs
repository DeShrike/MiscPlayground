using UnityEngine;
using System.Collections;

public class BaseArea
{
    public int Id { get; set; }

    public int X1 { get; set; }

    public int Y1 { get; set; }

    public int X2
    {
        get
        {
            return this.X1 + this.Width - 1;
        }
    }

    public int Y2
    {
        get
        {
            return this.Y1 + this.Height - 1;
        }
    }

    public int Width { get; set; }

    public int Height { get; set; }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return string.Format("({0},{1})-({2},{3}) {4}x{5}", this.X1, this.Y1, this.X2, this.Y2, this.Width, this.Height);
    }
}
