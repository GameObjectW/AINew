using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CFSkillAction : ITriggerAIAction {

    private Animator Ani;
    private AINew ai;
    private string[] AniTriggerString;
    private float FirstTime;
    private float SecondTime;
    private float ThirdTime;
    private NavMeshAgent NV;
    private int Damage;
    private float CFSpeed;

    private Coroutine co;
    private float InitSpeed;
    private Vector3 EndPos;

    public CFSkillAction(AINew ai, string[] aniTriggerString, float firstTime, float secondTime, float thirdTime, NavMeshAgent nv, int damage, Animator ani, float cfSpeed)
    {
        this.ai = ai;
        AniTriggerString = aniTriggerString;
        FirstTime = firstTime;
        SecondTime = secondTime;
        NV = nv;
        Damage = damage;
        Ani = ani;
        CFSpeed = cfSpeed;
        ThirdTime = thirdTime;
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
        if (!ai.GetComplete() && newState != AIState.Die)
        {
            return false;
        }
        UpdateManager.Ins.StopCoroutineCustom(co);
        co = null;
        ai.SetComplete(true);
        return true;
    }

    IEnumerator Start()
    {
        NV.avoidancePriority = 1;
        NV.isStopped = true;
        EndPos = ai.Target.transform.position;
        ai.SetComplete(false);
        Ani.SetTrigger(AniTriggerString[0]);
        Debug.Log("释放技能1··············@@@@@@@@@@@@@@@@@@@@@@@@@@");
        yield return new WaitForSeconds(FirstTime);
        NV.isStopped = false;
        InitSpeed = NV.speed;
        NV.speed = CFSpeed;
        NV.avoidancePriority = 1;
        NV.SetDestination(EndPos);
        yield return new WaitUntil(()=>ai.DistanceSelfToTarget(EndPos)<ai.AttackDistance);
        NV.isStopped = true;
        NV.velocity = Vector3.zero;
        Ani.SetTrigger(AniTriggerString[1]);
        yield return new WaitForSeconds(SecondTime);

        //GameDataManager.Ins.Player.GetComponentInChildren<HitByBulletBase>().BulletDeal(Damage, ai.TheType);
        yield return new WaitForSeconds(ThirdTime);
        NV.speed = InitSpeed;
        ai.SetComplete(true);

    }
}
