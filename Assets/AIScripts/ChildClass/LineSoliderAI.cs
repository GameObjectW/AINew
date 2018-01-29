using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSoliderAI : AINew
{

    ITriggerAIAction skill1Action;
    ITriggerAIAction skill2Action;

    public float SYAttackDistance;

    private bool isCanAssault = true;
    public float AssaultTriggerMaxDistance;
    public float AssaultTriggerMinDistance;

    private SkillCDAndWeight_New skillCdAndWeightNew;
    public override void Awake()
    {
        base.Awake();
        IdleInitAction = new IdleAction(this, "Idle", NV, mr, colliders, Ani);
        AttackAction = new NormalAttack(this, new string[] { "attack1", "attack2" }, 0.44f, NV, Ani);
        BackAction = new TakeBack(this, "", NV, transform);
        InjuredAction = new InjuredAction(this, "GetHit", 1, NV, Ani);
        RunAction = new RunAction(this, "Run", NV, Ani);
        DieAction = new DieAction(this, "Die", mr, NV, transform, colliders, Ani, 0.2f);
        //技能节点初始化
        skill1Action = new SYSkillAction(this, "skill1", 0.7f, 0.7f, NV, 10,Ani);
        skill2Action = new JumpAction(this, "skill2", 1.4f, 3f, NV, 3,Ani,1.65f);
        //将技能添加到总技能列表中，激活时向CD计算器添加技能
        SkillList.Add(new SkillCDAndWeight_New(skill1Action, 10, 20));

    }


    void Update()
    {
        if (PropertyQueue.Count()!=0&& GetComplete() && CurrentSelfToTargetDis < SYAttackDistance)
        {
            skillCdAndWeightNew = GetOneSkillInPropertyList();
            if (skillCdAndWeightNew != null)
            {
                SkillOneAction = skillCdAndWeightNew.Skill;
                Debug.Log("........................................");
                TestSkill();
            }
        }
        if (isCanAssault && NowState == AIState.Run && (CurrentSelfToTargetDis < AssaultTriggerMaxDistance && CurrentSelfToTargetDis > AssaultTriggerMinDistance))
        {
            SkillOneAction = skill2Action;
            isCanAssault = false;
            TestSkill();
        }

    }
    public override void OnJustEnterAttackState()
    {
        isCanAssault = true;
    }

    public override void OnJustExitSkillState()
    {
        AddSkillTocdList(skillCdAndWeightNew);
        skillCdAndWeightNew = null;
    }
}
