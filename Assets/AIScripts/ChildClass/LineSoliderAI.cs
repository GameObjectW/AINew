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
        //技能节点初始化（skill1Action是AI中需要cd和weight控制的技能，而skill1Action是根据相应的触发条件触发的）
        skill1Action = new SYSkillAction(this, "skill1", 0.7f, 0.7f, NV, 10,Ani);
        skill2Action = new JumpAction(this, "skill2", 1.4f, 3f, NV, 3,Ani,1.65f);
        //将通过冷却和权值的技能添加到总技能列表中（所以仅将skill1Action进行CD和Weight的封装并添加，而skill2Action在update中根据条件来单独控制技能的触发）
        SkillList.Add(new SkillCDAndWeight_New(skill1Action, 10, 20));

    }


    void Update()
    {
        //第一个if就是当AI进入到近战范围内，开始从队列中取出技能来释放（释放节奏通过cd和weight控制）
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
        //这个if控制带有特定触发条件的技能的触发
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
    /// <summary>
    /// 重写回调函数来实现一些初始化，该处在技能状态结束后，将带有CD和Weight封装的技能添加到cd列表中进行冷却。
    /// 没有封装的技能也会调用这里，由于skillCdAndWeightNew为空所以不会起任何作用。
    /// 也可以正常使用需要CD冷却但是不需要添加优先级队列的技能，只需要将权重设置为999即可
    /// </summary>
    public override void OnJustExitSkillState()
    {
        AddSkillTocdList(skillCdAndWeightNew);
        skillCdAndWeightNew = null;
    }
}
