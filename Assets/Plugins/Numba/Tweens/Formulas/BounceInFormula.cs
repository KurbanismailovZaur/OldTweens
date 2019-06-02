using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text;

namespace Numba.Tweens.Formulas
{
    public class BounceInFormula : Formula
    {
        public override float RemapFormula(float value) => 1f - BounceOut.RemapFormula(1f - value);

        public override byte Calculate(byte from, byte to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public override sbyte Calculate(sbyte from, sbyte to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public override bool Calculate(bool from, bool to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public override char Calculate(char from, char to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public override short Calculate(short from, short to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public override ushort Calculate(ushort from, ushort to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public override int Calculate(int from, int to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public override uint Calculate(uint from, uint to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public override float Calculate(float from, float to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public override long Calculate(long from, long to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public override ulong Calculate(ulong from, ulong to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public override double Calculate(double from, double to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public override decimal Calculate(decimal from, decimal to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public override Vector2 Calculate(Vector2 from, Vector2 to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public override Vector3 Calculate(Vector3 from, Vector3 to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public override Vector4 Calculate(Vector4 from, Vector4 to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public override Quaternion Calculate(Quaternion from, Quaternion to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public override Rect Calculate(Rect from, Rect to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public override Matrix4x4 Calculate(Matrix4x4 from, Matrix4x4 to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public override Bounds Calculate(Bounds from, Bounds to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public override Color Calculate(Color from, Color to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public override Color32 Calculate(Color32 from, Color32 to, float value) => Linear.Calculate(from, to, RemapFormula(value));
    }
}