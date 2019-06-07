using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Numba.Tweens
{
    /// <summary>
    /// <para>Help run coroutines.</para>
    /// <para>Auto creatable, not destroyable and not visible in hierarchy.</para>
    /// </summary>
    internal sealed class CoroutineHelper : MonoBehaviour
    {
        /// <summary>
        /// <para>Return instance of this class (singleton pattern).</para>
        /// <para>Returned object will not be visualized in hierarchy and not be destroyed between scenes loading.</para>
        /// </summary>
        public static CoroutineHelper Instance { get; private set; }

        /// <summary>
        /// <para>Create and save one instance of this class (singleton pattern).</para>
        /// <para>Created object will not be visible in hierarchy and do not destroyed between scenes.</para>
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void CreateInstance()
        {
            Instance = new GameObject("RoutineHelper (Tweens)").AddComponent<CoroutineHelper>();
            Instance.gameObject.hideFlags = HideFlags.HideInHierarchy;

            DontDestroyOnLoad(Instance.gameObject);
        }
    }
}