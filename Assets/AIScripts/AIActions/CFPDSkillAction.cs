using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CFPDSkillAction : ITriggerAIAction {

    private Animator Ani;
    private AINew ai;
    private string AniTriggerString;
    private float FirstTime;
    private float SecondTime;
    private NavMeshAgent NV;

    private Coroutine co;
    
    public CFPDSkillAction(AINew ai, string aniTriggerString, float firstTime,float secondTime, NavMeshAgent nv)
    {
        this.ai = ai;
        AniTriggerString = aniTriggerString;
        FirstTime = firstTime;
        SecondTime = secondTime;
        NV = nv;
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
        if (!ai.GetComplete() && newState !=AIState.Die)
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
        NV.isStopped = true;
        ai.SetComplete(false);
        //Ani.SetTrigger(AniTriggerString);
        Debug.Log("释放技能1··············");
        yield return new WaitForSeconds(FirstTime);
        Debug.Log("释放技能2··············");
        yield return new WaitForSeconds(SecondTime);
        ai.SetComplete(true);

    }
}
