using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Idle,
    Run,
    Attack,
    Die,
    Injured,
    Back,
    Skill
}
public abstract class AINew : MonoBehaviour
{
    private AIState nowState;

    public AIState LastState;

    public Transform Target;

    public float AttackDistance;

    public float backThreshold;


    [Range(0,100)]
    public int CurrentHP;

    protected NavMeshAgent NV;

    protected float CurrentSelfToTargetDis;

    private ITriggerAIAction _idleInitAction;
    private ITriggerAIAction _attackAction;
    private ITriggerAIAction _backAction;
    private ITriggerAIAction _injuredAction;
    private ITriggerAIAction _runAction;
    private ITriggerAIAction _dieAction;
    private ITriggerAIAction _skillOneAction;

    private List<ITriggerAIAction> ActionArray;
    protected Renderer[] mr;
    protected Collider[] colliders;
    protected Animator Ani;
    protected List<SkillCDAndWeight_New> SkillList = new List<SkillCDAndWeight_New>();
    protected PropertyQueue<SkillCDAndWeight_New> PropertyQueue = new PropertyQueue<SkillCDAndWeight_New>();

    private Dictionary<AIState, ITriggerAIAction> _actionDic;
    
    private List<SkillCDAndWeight_New> CdCalList = new List<SkillCDAndWeight_New>();

    // Use this for initialization
    public virtual void Awake()
    {
        NV = GetComponent<NavMeshAgent>();
        ActionArray=new List<ITriggerAIAction>();
        mr = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();
        Ani = GetComponent<Animator>();
    }

    

    void OnEnable()
    {
        foreach (SkillCDAndWeight_New skillCdAndWeight in SkillList)
        {
            CdCalList.Add(skillCdAndWeight);
        }
        StartCoroutine(BaseUpdate());
        StartCoroutine(CalCD());
    }

    public virtual void Start ()
	{
       

    }

    private IEnumerator BaseUpdate()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(()=>DieAction!=null);
        _actionDic = new Dictionary<AIState, ITriggerAIAction>()
        {
            {AIState.Idle, IdleInitAction },
            {AIState.Run,RunAction },
            {AIState.Attack,AttackAction },
            {AIState.Die,DieAction },
            {AIState.Injured,InjuredAction },
            {AIState.Back,BackAction },
            {AIState.Skill,SkillOneAction }
        };

        ChangeState(AIState.Idle);
        while (true)
        {
            if (Target == null)
            {
                ChangeState(AIState.Idle);
                continue;
            }
            CurrentSelfToTargetDis = DistanceSelfToTarget(Target);

            if (isCanChangeState)
            {
                //if (CurrentSelfToTargetDis < (AttackDistance - backThreshold))
                //{
                //    ChangeState(AIState.Back);
                //}
                //else 
                if (CurrentSelfToTargetDis <= AttackDistance)
                {
                    ChangeState(AIState.Attack);
                }
                else if (CurrentSelfToTargetDis > AttackDistance)
                {
                    ChangeState(AIState.Run);
                }
            }
            yield return null;
        }
    }
    public float DistanceSelfToTarget(Transform target)
    {
        Vector3 SelfPos = transform.position;
        SelfPos.y = 0;
        Vector3 TargetPos = target.position;
        TargetPos.y = 0;
        return Vector3.Distance(SelfPos, TargetPos);
    }
    public float DistanceSelfToTarget(Vector3 target)
    {
        Vector3 SelfPos = transform.position;
        SelfPos.y = 0;
        Vector3 TargetPos = target;
        TargetPos.y = 0;
        return Vector3.Distance(SelfPos, TargetPos);
    }
    private void ChangeState(AIState state)
    {
        if ( NowState == AIState.Die&&state!=AIState.Idle)
        {
            return;
        }
        if (!SetCurrentActionStop(nowState,state))
        {
            return;
        }
        LastState = NowState;
        NowState = state;

        _actionDic[NowState].TriggerAction();
    }

    public bool SetCurrentActionStop(AIState state, AIState newState)
    {
        return _actionDic[state].CancelAction(newState);
    }

    private bool isCanChangeState = true;

    public AIState NowState
    {
        get
        {
            return nowState;
        }

        set
        {
            if (value!=nowState)
            {
                StateChangeCallBackDeal(value,nowState);
            }
            nowState = value;
        }
    }

    protected ITriggerAIAction AttackAction
    {
        get
        {
            return _attackAction;
        }

        set
        {
            if (_attackAction!=null)
            {
                ActionArray.Remove(_attackAction);
            }
            if (value!=null)
            {
                ActionArray.Add(value);
            }
            _attackAction = value;
        }
    }

    protected ITriggerAIAction BackAction
    {
        get
        {
            return _backAction;
        }

        set
        {
            if (_backAction != null)
            {
                ActionArray.Remove(_backAction);
            }
            if (value != null)
            {
                ActionArray.Add(value);
            }
            _backAction = value;
        }
    }

    protected ITriggerAIAction InjuredAction
    {
        get
        {
            return _injuredAction;
        }

        set
        {
            if (_injuredAction != null)
            {
                ActionArray.Remove(_injuredAction);
            }
            if (value != null)
            {
                ActionArray.Add(value);
            }
            _injuredAction = value;
        }
    }

    protected ITriggerAIAction RunAction
    {
        get
        {
            return _runAction;
        }

        set
        {
            if (_runAction != null)
            {
                ActionArray.Remove(_runAction);
            }
            if (value != null)
            {
                ActionArray.Add(value);
            }
            _runAction = value;
        }
    }

    protected ITriggerAIAction DieAction
    {
        get
        {
            return _dieAction;
        }

        set
        {
            if (_dieAction != null)
            {
                ActionArray.Remove(_dieAction);
            }
            if (value != null)
            {
                ActionArray.Add(value);
            }
            _dieAction = value;
        }
    }

    protected ITriggerAIAction SkillOneAction
    {
        get
        {
            return _skillOneAction;
        }

        set
        {
            if (_skillOneAction != null)
            {
                ActionArray.Remove(_skillOneAction);
            }
            if (value != null)
            {
                ActionArray.Add(value);
                _actionDic[AIState.Skill] = value;
            }
            _skillOneAction = value;
        }
    }

    protected ITriggerAIAction IdleInitAction
    {
        get
        {
            return _idleInitAction;
        }

        set
        {
            if (_idleInitAction != null)
            {
                ActionArray.Remove(_idleInitAction);
            }
            if (value != null)
            {
                ActionArray.Add(value);
            }
            _idleInitAction = value;
        }
    }

    public void SetComplete(bool flag)
    {
        isCanChangeState = flag;
    }
    public bool GetComplete()
    {
        return isCanChangeState;
    }
    public void TestInjured()
    {
        ChangeState(AIState.Injured);
    }
    public void TestDie()
    {
        ChangeState(AIState.Die);
    }
    public void TestSkill()
    {
        ChangeState(AIState.Skill);
    }

    private void StateChangeCallBackDeal(AIState NewState,AIState OldState)
    {
        switch (OldState)
        {
            case AIState.Idle:
                OnJustExitIdleState();
                break;
            case AIState.Run:
                OnJustExitRunState();
                break;
            case AIState.Attack:
                OnJustExitAttackState();
                break;
            case AIState.Die:
                OnJustExitDieState();
                break;
            case AIState.Injured:
                OnJustExitInjuredState();
                break;
            case AIState.Back:
                OnJustExitBackState();
                break;
            case AIState.Skill:
                OnJustExitSkillState();
                break;
            default:
                throw new ArgumentOutOfRangeException("OldState", OldState, null);
        }
        switch (NewState)
        {
            case AIState.Idle:
                OnJustEnterIdleState();
                break;
            case AIState.Run:
                OnJustEnterRunState();
                break;
            case AIState.Attack:
                OnJustEnterAttackState();
                break;
            case AIState.Die:
                OnJustEnterDieState();
                break;
            case AIState.Injured:
                OnJustEnterInjuredState();
                break;
            case AIState.Back:
                OnJustEnterBackState();
                break;
            case AIState.Skill:
                OnJustEnterSkillState();
                break;
            default:
                throw new ArgumentOutOfRangeException("NewState", NewState, null);
        }
    }

    public virtual void OnJustEnterRunState()
    {
        NV.avoidancePriority = 50;
    }
    public virtual void OnJustExitRunState() { Ani.ResetTrigger("Run");}
    public virtual void OnJustEnterIdleState() { }
    public virtual void OnJustExitIdleState() { }
    public virtual void OnJustEnterAttackState() { NV.avoidancePriority = 2;transform.LookAt(new Vector3(Target.position.x,transform.position.y,Target.position.z)); }
    public virtual void OnJustExitAttackState() { }
    public virtual void OnJustEnterInjuredState() { }
    public virtual void OnJustExitInjuredState() { }
    public virtual void OnJustEnterSkillState() { transform.LookAt(new Vector3(Target.position.x, transform.position.y, Target.position.z)); }
    public virtual void OnJustExitSkillState() { }
    public virtual void OnJustEnterDieState() { }
    public virtual void OnJustExitDieState() { }
    public virtual void OnJustEnterBackState() { }
    public virtual void OnJustExitBackState() { }

    IEnumerator CalCD()
    {
        List<SkillCDAndWeight_New> WillDel = new List<SkillCDAndWeight_New>();
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (CdCalList.Count > 0)
            {
                foreach (SkillCDAndWeight_New item in CdCalList)
                {
                    if (item.SkillIsReady())
                    {
                        WillDel.Add(item);
                    }
                }
                if (WillDel.Count > 0)
                {
                    foreach (SkillCDAndWeight_New item in WillDel)
                    {
                        CdCalList.Remove(item);
                        if (item.CD != 999)
                        {
                            PropertyQueue.Push(item);
                        }
                    }
                    WillDel.Clear();
                }

            }
        }
    }

    protected SkillCDAndWeight_New GetOneSkillInPropertyList()
    {
        SkillCDAndWeight_New skillCdAndWeightNew = PropertyQueue.Pop();
        return skillCdAndWeightNew;
    }

    protected void AddSkillTocdList(SkillCDAndWeight_New skill)
    {
        if (skill==null)
        {
            return;
        }
        CdCalList.Add(skill);
    }

    void OnDisable()
    {
        StopCoroutine(CalCD());
        CdCalList.Clear();
        PropertyQueue.clear();
        ChangeState(AIState.Idle);
    }
}
