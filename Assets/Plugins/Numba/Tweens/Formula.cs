using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using Numba.Tweens.Formulas;

namespace Numba.Tweens
{
    public abstract class Formula
    {
        public static LinearFormula Linear { get; private set; } = new LinearFormula();

        public static QuadraticInFormula QuadIn { get; private set; } = new QuadraticInFormula();

        public static QuadraticOutFormula QuadOut { get; private set; } = new QuadraticOutFormula();

        public static QuadraticInOutFormula QuadInOut { get; private set; } = new QuadraticInOutFormula();

        public static CubicInFormula CubicIn { get; private set; } = new CubicInFormula();

        public static CubicOutFormula CubicOut { get; private set; } = new CubicOutFormula();

        public static CubicInOutFormula CubicInOut { get; private set; } = new CubicInOutFormula();

        public static QuarticInFormula QuartIn { get; private set; } = new QuarticInFormula();

        public static QuarticOutFormula QuartOut { get; private set; } = new QuarticOutFormula();

        public static QuarticInOutFormula QuartInOut { get; private set; } = new QuarticInOutFormula();

        public static QuinticInFormula QuintIn { get; private set; } = new QuinticInFormula();

        public static QuinticOutFormula QuintOut { get; private set; } = new QuinticOutFormula();

        public static QuinticInOutFormula QuintInOut { get; private set; } = new QuinticInOutFormula();

        public static SineInFormula SineIn { get; private set; } = new SineInFormula();

        public static SineOutFormula SineOut { get; private set; } = new SineOutFormula();

        public static SineInOutFormula SineInOut { get; private set; } = new SineInOutFormula();

        public static CircularInFormula CircIn { get; private set; } = new CircularInFormula();

        public static CircularOutFormula CircOut { get; private set; } = new CircularOutFormula();

        public static CircularInOutFormula CircInOut { get; private set; } = new CircularInOutFormula();

        public static ExponentialInFormula ExpoIn { get; private set; } = new ExponentialInFormula();

        public static ExponentialOutFormula ExpoOut { get; private set; } = new ExponentialOutFormula();

        public static ExponentialInOutFormula ExpoInOut { get; private set; } = new ExponentialInOutFormula();

        public static ElasticInFormula ElasticIn { get; private set; } = new ElasticInFormula();

        public static ElasticOutFormula ElasticOut { get; private set; } = new ElasticOutFormula();

        public static ElasticInOutFormula ElasticInOut { get; private set; } = new ElasticInOutFormula();

        public static BackInFormula BackIn { get; private set; } = new BackInFormula();

        public static BackOutFormula BackOut { get; private set; } = new BackOutFormula();

        public static BackInOutFormula BackInOut { get; private set; } = new BackInOutFormula();

        public static BounceInFormula BounceIn { get; private set; } = new BounceInFormula();

        public static BounceOutFormula BounceOut { get; private set; } = new BounceOutFormula();

        public static BounceInOutFormula BounceInOut { get; private set; } = new BounceInOutFormula();

        public abstract float RemapFormula(float value);

        public byte Calculate(byte from, byte to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public sbyte Calculate(sbyte from, sbyte to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public bool Calculate(bool from, bool to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public char Calculate(char from, char to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public short Calculate(short from, short to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public ushort Calculate(ushort from, ushort to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public int Calculate(int from, int to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public uint Calculate(uint from, uint to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public float Calculate(float from, float to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public long Calculate(long from, long to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public ulong Calculate(ulong from, ulong to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public double Calculate(double from, double to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public decimal Calculate(decimal from, decimal to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public Vector2 Calculate(Vector2 from, Vector2 to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public Vector3 Calculate(Vector3 from, Vector3 to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public Vector4 Calculate(Vector4 from, Vector4 to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public Quaternion Calculate(Quaternion from, Quaternion to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public Rect Calculate(Rect from, Rect to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public Matrix4x4 Calculate(Matrix4x4 from, Matrix4x4 to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public Bounds Calculate(Bounds from, Bounds to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public Color Calculate(Color from, Color to, float value) => Linear.Calculate(from, to, RemapFormula(value));

        public Color32 Calculate(Color32 from, Color32 to, float value) => Linear.Calculate(from, to, RemapFormula(value));
    }
}