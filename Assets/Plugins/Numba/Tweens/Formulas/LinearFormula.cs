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
        public override float RemapFormula(float value) => value;
        
        public override byte Calculate(byte from, byte to, float value) => (byte)(from + (to - from) * value);

        public override sbyte Calculate(sbyte from, sbyte to, float value) => (sbyte)(from + (to - from) * value);

        public override bool Calculate(bool from, bool to, float value) => value >= 1f ? to : from;

        public override char Calculate(char from, char to, float value) => (char)(from + (to - from) * value);

        public override short Calculate(short from, short to, float value) => (byte)(from + (to - from) * value);

        public override ushort Calculate(ushort from, ushort to, float value) => (ushort)(from + (to - from) * value);

        public override int Calculate(int from, int to, float value) => (int)(from + (to - from) * value);

        public override uint Calculate(uint from, uint to, float value) => (uint)(from + (to - from) * value);

        public override float Calculate(float from, float to, float value) => from + (to - from) * value;

        public override long Calculate(long from, long to, float value) => (long)(from + (to - from) * value);

        public override ulong Calculate(ulong from, ulong to, float value) => (ulong)(from + (to - from) * value);

        public override double Calculate(double from, double to, float value) => from + (to - from) * value;

        public override decimal Calculate(decimal from, decimal to, float value) => from + (to - from) * (decimal)value;

        public override Vector2 Calculate(Vector2 from, Vector2 to, float value) => from + (to - from) * value;

        public override Vector3 Calculate(Vector3 from, Vector3 to, float value) => from + (to - from) * value;

        public override Vector4 Calculate(Vector4 from, Vector4 to, float value) => from + (to - from) * value;

        public override Quaternion Calculate(Quaternion from, Quaternion to, float value) => Quaternion.Lerp(from, to, value);

        public override Rect Calculate(Rect from, Rect to, float value) => new Rect(Calculate(from.position, to.position, value), Calculate(from.size, to.size, value));

        public override Matrix4x4 Calculate(Matrix4x4 from, Matrix4x4 to, float value) => new Matrix4x4(Calculate(from.GetColumn(0), to.GetColumn(0), value), Calculate(from.GetColumn(1), to.GetColumn(1), value), Calculate(from.GetColumn(2), to.GetColumn(2), value), Calculate(from.GetColumn(3), to.GetColumn(3), value));

        public override Bounds Calculate(Bounds from, Bounds to, float value) => new Bounds(Calculate(from.center, to.center, value), Calculate(from.size, to.size, value));

        public override Color Calculate(Color from, Color to, float value) => from + (to - from) * value;

        public override Color32 Calculate(Color32 from, Color32 to, float value) => from + ((Color)to - (Color)from) * value;
    }
}