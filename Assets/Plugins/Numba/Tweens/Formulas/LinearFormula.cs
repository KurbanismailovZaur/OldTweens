using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text;

namespace Numba.Tweens.Formulas
{
    public class LinearFormula : Formula
    {
        public override float Remap(float value) => value;
        
        public new byte Calculate(byte from, byte to, float value) => (byte)(from + (to - from) * value);

        public new sbyte Calculate(sbyte from, sbyte to, float value) => (sbyte)(from + (to - from) * value);

        public new bool Calculate(bool from, bool to, float value) => value >= 1f ? to : from;

        public new char Calculate(char from, char to, float value) => (char)(from + (to - from) * value);

        public new short Calculate(short from, short to, float value) => (byte)(from + (to - from) * value);

        public new ushort Calculate(ushort from, ushort to, float value) => (ushort)(from + (to - from) * value);

        public new int Calculate(int from, int to, float value) => (int)(from + (to - from) * value);

        public new uint Calculate(uint from, uint to, float value) => (uint)(from + (to - from) * value);

        public new float Calculate(float from, float to, float value) => from + (to - from) * value;

        public new long Calculate(long from, long to, float value) => (long)(from + (to - from) * value);

        public new ulong Calculate(ulong from, ulong to, float value) => (ulong)(from + (to - from) * value);

        public new double Calculate(double from, double to, float value) => from + (to - from) * value;

        public new decimal Calculate(decimal from, decimal to, float value) => from + (to - from) * (decimal)value;

        public new Vector2 Calculate(Vector2 from, Vector2 to, float value) => from + (to - from) * value;

        public new Vector3 Calculate(Vector3 from, Vector3 to, float value) => from + (to - from) * value;

        public new Vector4 Calculate(Vector4 from, Vector4 to, float value) => from + (to - from) * value;

        public new Quaternion Calculate(Quaternion from, Quaternion to, float value) => Quaternion.Lerp(from, to, value);

        public new Rect Calculate(Rect from, Rect to, float value) => new Rect(Calculate(from.position, to.position, value), Calculate(from.size, to.size, value));

        public new Matrix4x4 Calculate(Matrix4x4 from, Matrix4x4 to, float value) => new Matrix4x4(Calculate(from.GetColumn(0), to.GetColumn(0), value), Calculate(from.GetColumn(1), to.GetColumn(1), value), Calculate(from.GetColumn(2), to.GetColumn(2), value), Calculate(from.GetColumn(3), to.GetColumn(3), value));

        public new Bounds Calculate(Bounds from, Bounds to, float value) => new Bounds(Calculate(from.center, to.center, value), Calculate(from.size, to.size, value));

        public new Color Calculate(Color from, Color to, float value) => from + (to - from) * value;

        public new Color32 Calculate(Color32 from, Color32 to, float value) => from + ((Color)to - (Color)from) * value;
    }
}