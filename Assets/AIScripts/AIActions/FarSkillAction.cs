using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

public class FarSkillAction : ITriggerAIAction {

    private Animator Ani;
    private AINew ai;
    private string AniTriggerString;
    private float OncTime;
    private NavMeshAgent NV;
    private float TwiceTime;

    private Coroutine co;

    [CanBeNull] private ISKillTrigger skill;

    public FarSkillAction(AINew ai, string aniTriggerString, float oncTime, NavMeshAgent nv, ISKillTrigger skill, float twiceTime, Animator ani)
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
        if (co != null)
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
        NV.avoidancePriority = 50;
        NV.isStopped = true;

        ai.SetComplete(false);
        Ani.SetTrigger(AniTriggerString);
        Debug.Log("攻击中··············");
        yield return new WaitForSeconds(OncTime);
        skill.TriggerSkill();
        yield return new WaitForSeconds(TwiceTime);
        
        yield return new WaitForSeconds(0.1f);

        ai.SetComplete(true);

    }

}
