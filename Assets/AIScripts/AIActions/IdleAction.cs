﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IdleAction : ITriggerAIAction {
    private Animator Ani;
    private AINew ai;
    private string AniTriggerString;

    private NavMeshAgent NV;

    private Coroutine co;

    private Renderer[] mr;
    private Collider[] collider;


    private bool isFinishInit=false;

    public IdleAction(AINew ai, string aniTriggerString, NavMeshAgent nv, Renderer[] mr, Collider[] collider, Animator ani)
    {
        this.ai = ai;
        AniTriggerString = aniTriggerString;
        NV = nv;
        this.mr = mr;
        this.collider = collider;
        Ani = ani;
    }

    public void TriggerAction()
    {
        if (isFinishInit||co != null)
        {
            return;
        }
        co = UpdateManager.Ins.StartCoroutineCustom(Start());
    }

    public bool CancelAction(AIState newState)
    {
        if (!ai.GetComplete())
        {
            Debug.Log("初始化未完成。。。。。。。。。");
            return false;
        }
        UpdateManager.Ins.StopCoroutineCustom(co);
        co = null;
        ai.SetComplete(true);
        return true;
    }

    IEnumerator Start()
    {
        ai.SetComplete(false);
        NV.enabled = true;
        NV.avoidancePriority = 50;
        Ani.SetTrigger(AniTriggerString);
    //    Debug.Log("初始化中··············");
        foreach (Renderer meshRenderer in mr)
        {
            meshRenderer.material.SetFloat("_RongJie", 0);
        }
        foreach (Collider item in collider)
        {
            item.enabled = true;
        }

        ai.SetComplete(true);
        yield return null;
    }
}
