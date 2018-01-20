using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoliderAI : AINew
{
    public bool isCanAssault=true;
    public float AssaultTriggerMaxDistance;
    public float AssaultTriggerMinDistance;
    public override void Awake()
    {
        base.Awake();
        IdleInitAction=new IdleAction(this,"",NV);
        AttackAction = new NormalAttack(this, "", 2, NV);
        BackAction = new TakeBack(this, "", 2, NV, transform, Target);
        InjuredAction = new InjuredAction(this, "", 1, NV);
        RunAction = new RunAction(this, "", Target, NV);
        DieAction = new DieAction(this, "", 5, NV, transform);
        SkillOneAction=new CFPDSkillAction(this,"",1,2,NV);
    }

    void Update()
    {
        if (isCanAssault && NowState == AIState.Run && (CurrentSelfToTargetDis < AssaultTriggerMaxDistance && CurrentSelfToTargetDis > AssaultTriggerMinDistance))
        {
            SkillOneAction=new CFPDSkillAction(this,"",3,4,NV);
            isCanAssault = false;
            TestSkill();
        }
    }

    public override void OnJustEnterAttackState()
    {
        isCanAssault = true;
    }
    
}
