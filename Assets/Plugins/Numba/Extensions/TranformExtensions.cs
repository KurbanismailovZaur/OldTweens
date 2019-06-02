using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Extensions
{
    public static class TranformExtensions
    {
		private static void SetVector3Values(Func<Vector3> getter, Action<Vector3> setter, params (int index, float value)[] data)
        {
            var position = getter();

			for (int i = 0; i < data.Length; i++)
            	position[data[i].index] = data[i].value;
				
            setter(position);
        }

		#region Position
        public static void SetPositionX(this Transform transform, float x) => SetVector3Values(() => transform.position, (value) => transform.position = value, (0, x));

        public static void SetPositionY(this Transform transform, float y) => SetVector3Values(() => transform.position, (value) => transform.position = value, (1, y));

        public static void SetPositionZ(this Transform transform, float z) => SetVector3Values(() => transform.position, (value) => transform.position = value, (2, z));

		public static void SetPositionXY(this Transform transform, float x, float y) => SetVector3Values(() => transform.position, (value) => transform.position = value, (0, x), (1, y));

		public static void SetPositionXZ(this Transform transform, float x, float z) => SetVector3Values(() => transform.position, (value) => transform.position = value, (0, x), (2, z));

		public static void SetPositionYZ(this Transform transform, float y, float z) => SetVector3Values(() => transform.position, (value) => transform.position = value, (1, y), (2, z));

		public static void SetLocalPositionX(this Transform transform, float x) => SetVector3Values(() => transform.localPosition, (value) => transform.localPosition = value, (0, x));

		public static void SetLocalPositionY(this Transform transform, float y) => SetVector3Values(() => transform.localPosition, (value) => transform.localPosition = value, (1, y));

        public static void SetLocalPositionZ(this Transform transform, float z) => SetVector3Values(() => transform.localPosition, (value) => transform.localPosition = value, (2, z));

		public static void SetLocalPositionXY(this Transform transform, float x, float y) => SetVector3Values(() => transform.localPosition, (value) => transform.localPosition = value, (0, x), (1, y));

		public static void SetLocalPositionXZ(this Transform transform, float x, float z) => SetVector3Values(() => transform.localPosition, (value) => transform.localPosition = value, (0, x), (2, z));

		public static void SetLocalPositionYZ(this Transform transform, float y, float z) => SetVector3Values(() => transform.localPosition, (value) => transform.localPosition = value, (1, y), (2, z));
		#endregion

		#region Rotation
		public static void SetEulerAnglesX(this Transform transform, float x) => SetVector3Values(() => transform.eulerAngles, (value) => transform.eulerAngles = value, (0, x));

		public static void SetEulerAnglesY(this Transform transform, float y) => SetVector3Values(() => transform.eulerAngles, (value) => transform.eulerAngles = value, (1, y));

		public static void SetEulerAnglesZ(this Transform transform, float z) => SetVector3Values(() => transform.eulerAngles, (value) => transform.eulerAngles = value, (2, z));

		public static void SetEulerAnglesXY(this Transform transform, float x, float y) => SetVector3Values(() => transform.eulerAngles, (value) => transform.eulerAngles = value, (0, x), (1, y));

		public static void SetEulerAnglesXZ(this Transform transform, float x, float z) => SetVector3Values(() => transform.eulerAngles, (value) => transform.eulerAngles = value, (0, x), (2, z));

		public static void SetEulerAnglesYZ(this Transform transform, float y, float z) => SetVector3Values(() => transform.eulerAngles, (value) => transform.eulerAngles = value, (1, y), (2, z));

		public static void SetLocalEulerAnglesX(this Transform transform, float x) => SetVector3Values(() => transform.localEulerAngles, (value) => transform.localEulerAngles = value, (0, x));

		public static void SetLocalEulerAnglesY(this Transform transform, float y) => SetVector3Values(() => transform.localEulerAngles, (value) => transform.localEulerAngles = value, (1, y));

		public static void SetLocalEulerAnglesZ(this Transform transform, float z) => SetVector3Values(() => transform.localEulerAngles, (value) => transform.localEulerAngles = value, (2, z));

		public static void SetLocalEulerAnglesXY(this Transform transform, float x, float y) => SetVector3Values(() => transform.localEulerAngles, (value) => transform.localEulerAngles = value, (0, x), (1, y));

		public static void SetLocalEulerAnglesXZ(this Transform transform, float x, float z) => SetVector3Values(() => transform.localEulerAngles, (value) => transform.localEulerAngles = value, (0, x), (2, z));

		public static void SetLocalEulerAnglesYZ(this Transform transform, float y, float z) => SetVector3Values(() => transform.localEulerAngles, (value) => transform.localEulerAngles = value, (1, y), (2, z));
		#endregion

		#region Scale
        public static void SetLocalScaleX(this Transform transform, float x) => SetVector3Values(() => transform.localScale, (value) => transform.localScale = value, (0, x));

        public static void SetLocalScaleY(this Transform transform, float y) => SetVector3Values(() => transform.localScale, (value) => transform.localScale = value, (1, y));

        public static void SetLocalScaleZ(this Transform transform, float z) => SetVector3Values(() => transform.localScale, (value) => transform.localScale = value, (2, z));

		public static void SetLocalScaleXY(this Transform transform, float x, float y) => SetVector3Values(() => transform.localScale, (value) => transform.localScale = value, (0, x), (1, y));

		public static void SetLocalScaleXZ(this Transform transform, float x, float z) => SetVector3Values(() => transform.localScale, (value) => transform.localScale = value, (0, x), (2, z));

		public static void SetLocalScaleYZ(this Transform transform, float y, float z) => SetVector3Values(() => transform.localScale, (value) => transform.localScale = value, (1, y), (2, z));
		#endregion
    }
}