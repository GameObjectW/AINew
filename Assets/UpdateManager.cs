using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{

    public static UpdateManager Ins;
    void Awake()
    {
        Ins = this;
    }

    public Coroutine StartCoroutineCustom(IEnumerator obj)
    {
        return StartCoroutine(obj);
    }

    public void StopCoroutineCustom(Coroutine co)
    {
        if (co==null)
        {
            return;
        }
        StopCoroutine(co);
    }
}
