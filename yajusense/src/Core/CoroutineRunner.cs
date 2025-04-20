using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using UnityEngine;

namespace yajusense.Core;

public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;

    public static void Initialize(CoroutineRunner instance)
    {
        _instance = instance;
    }

    public static Coroutine StartManagedCoroutine(IEnumerator routine)
    {
        return _instance?.StartCoroutine(routine.WrapToIl2Cpp());
    }

    public static void StopManagedCoroutine(Coroutine routine)
    {
        _instance?.StopCoroutine(routine);
    }
}