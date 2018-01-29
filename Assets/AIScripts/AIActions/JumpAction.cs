using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JumpAction : ITriggerAIAction {

    private Animator Ani;
    private AINew ai;
    private string AniTriggerString;
    private float FirstTime;
    private float SecondTime;
    private NavMeshAgent NV;
    private float Height;
    private float JumpTime;

    private Coroutine co;
    private Transform EndPos;
    private Vector3 SelfPos;
    private Vector3 TargetPos;
    private Vector3 MidPos;
    private Vector3 FinalPos;
    private float InitDistance;

    public JumpAction(AINew ai, string aniTriggerString, float firstTime, float secondTime, NavMeshAgent nv, float height, Animator ani, float jumpTime)
    {
        this.ai = ai;
        AniTriggerString = aniTriggerString;
        FirstTime = firstTime;
        SecondTime = secondTime;
        NV = nv;
        Height = height;
        Ani = ani;
        JumpTime = jumpTime;
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
        
        SelfPos = ai.transform.position;
        TargetPos = ai.Target.position;
        TargetPos.y = SelfPos.y;
        FinalPos = TargetPos + (TargetPos - SelfPos).normalized * 2;
        InitDistance = ai.DistanceSelfToTarget(FinalPos);


        NV.enabled = false;
        ai.SetComplete(false);
        Ani.SetTrigger(AniTriggerString);

        float LerpTime = 0;
        yield return new WaitForSeconds(FirstTime);
        while (Vector3.Distance(ai.transform.position, FinalPos) >0.2f)
        {
            LerpTime += Time.deltaTime;
            MidPos = Vector3.Lerp(SelfPos, FinalPos, LerpTime / JumpTime);
            MidPos.y = SelfPos.y+ Height* Mathf.Sin(Mathf.Deg2Rad*(180*(1 - ai.DistanceSelfToTarget(FinalPos) /InitDistance)));
            ai.transform.position = MidPos;
            Debug.Log((1 - ai.DistanceSelfToTarget(FinalPos) / InitDistance));
            yield return null;
        }
        
        
        Debug.Log("释放技能1··············");
        yield return new WaitForSeconds(SecondTime);

        NV.enabled = true;
        ai.SetComplete(true);

    }
}
