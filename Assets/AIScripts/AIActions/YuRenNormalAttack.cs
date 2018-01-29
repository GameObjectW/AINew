using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

public class YuRenNormalAttack : ITriggerAIAction
{
    private Animator Ani;
    private AINew ai;
    private string AniTriggerString;
    private float OncTime;
    private NavMeshAgent NV;
    private float TwiceTime;

    private Coroutine co;

    [CanBeNull] private ISKillTrigger skill;

    public YuRenNormalAttack(AINew ai, string aniTriggerString, float oncTime, NavMeshAgent nv, ISKillTrigger skill, float twiceTime, Animator ani)
    {
        this.ai = ai;
        AniTriggerString = aniTriggerString;
        OncTime = oncTime;
        NV = nv;
        this.skill = skill;
        TwiceTime = twiceTime;
        Ani = ani;
    }

    public void TriggerAction()
    {
        if (co!=null)
        {
            return;
        }
        co = UpdateManager.Ins.StartCoroutineCustom(Start());
    }

    public bool CancelAction(AIState newState)
    {
        if (!ai.GetComplete() && (newState != AIState.Die && newState != AIState.Idle))
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
            ai.SetComplete(false);
            ai.transform.LookAt(new Vector3(ai.Target.position.x, ai.transform.position.y, ai.Target.position.z));
            Ani.SetTrigger(AniTriggerString);
           // Debug.Log("攻击中··············");
            yield return new WaitForSeconds(OncTime);
            skill.TriggerSkill();
            yield return new WaitForSeconds(TwiceTime);
            ai.SetComplete(true);
            yield return new WaitForSeconds(0.1f);
            
        }
    }


}
