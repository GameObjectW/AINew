﻿using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

public class NormalAttack : ITriggerAIAction
{

    private Animator Ani;
    private AINew ai;
    private string[] AniTriggerStringArray;
    private float OncTime;
    private NavMeshAgent NV;


    private Coroutine co;


    public NormalAttack(AINew ai, string[] aniTriggerStringArray, float oncTime, NavMeshAgent nv, Animator ani)
    {
        this.ai = ai;
        AniTriggerStringArray = aniTriggerStringArray;
        OncTime = oncTime;
        NV = nv;
        Ani = ani;
    }

    public void TriggerAction()
    {
        if (co != null)
        {
            return;
        }
        co = UpdateManager.Ins.StartCoroutineCustom(Start());
    }

    public bool CancelAction(AIState newState)
    {
        if (!ai.GetComplete() && (newState != AIState.Die && newState != AIState.Idle && newState != AIState.Injured))
        {
            return false;
        }
        UpdateManager.Ins.StopCoroutineCustom(co);
        ai.SetComplete(true);
        co = null;
        return true;
    }

    IEnumerator Start()
    {
        NV.avoidancePriority = 2;
        NV.isStopped = true;
        while (true)
        {
            string animatorString = AniTriggerStringArray[Random.Range(0, AniTriggerStringArray.Length)];
            ai.SetComplete(false);
            ai.transform.LookAt(new Vector3(ai.Target.position.x, ai.transform.position.y, ai.Target.position.z));
            Ani.SetTrigger(animatorString);
         //   Debug.Log("攻击中··············");
            yield return new WaitUntil(() => Ani.GetCurrentAnimatorStateInfo(0).IsName(animatorString));
            yield return new WaitForSeconds(Ani.GetCurrentAnimatorStateInfo(0).length-0.1f);
            ai.SetComplete(true);
            yield return new WaitForSeconds(1f);

        }
    }
}
