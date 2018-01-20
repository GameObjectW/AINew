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
    private Dictionary<AIState, ITriggerAIAction> ActionDic;


    // Use this for initialization
    public virtual void Awake()
    {
        NV = GetComponent<NavMeshAgent>();
        ActionArray = new List<ITriggerAIAction>();
        

    }

    void OnEnable()
    {

        StartCoroutine(BaseUpdate());
    }

    public void Start ()
	{
        ActionDic = new Dictionary<AIState, ITriggerAIAction>()
        {
            {AIState.Idle,IdleInitAction },
            {AIState.Run,RunAction },
            {AIState.Attack,AttackAction },
            {AIState.Die,DieAction },
            {AIState.Injured,InjuredAction },
            {AIState.Back,BackAction },
            {AIState.Skill,SkillOneAction }
        };

    }

    private IEnumerator BaseUpdate()
    {
        yield return new WaitUntil(() => IdleInitAction != null);
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
                if (CurrentSelfToTargetDis < (AttackDistance - backThreshold))
                {
                    ChangeState(AIState.Back);
                }
                else if (CurrentSelfToTargetDis <= AttackDistance)
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
    float DistanceSelfToTarget(Transform target)
    {
        Vector3 SelfPos = transform.position;
        SelfPos.y = 0;
        Vector3 TargetPos = target.position;
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

        ActionDic[NowState].TriggerAction();
    }

    public bool SetCurrentActionStop(AIState state, AIState newState)
    {
        return ActionDic[state].CancelAction(newState);
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

    public virtual void OnJustEnterRunState() { }
    public virtual void OnJustExitRunState() { }
    public virtual void OnJustEnterIdleState() { }
    public virtual void OnJustExitIdleState() { }
    public virtual void OnJustEnterAttackState() { }
    public virtual void OnJustExitAttackState() { }
    public virtual void OnJustEnterInjuredState() { }
    public virtual void OnJustExitInjuredState() { }
    public virtual void OnJustEnterSkillState() { }
    public virtual void OnJustExitSkillState() { }
    public virtual void OnJustEnterDieState() { }
    public virtual void OnJustExitDieState() { }
    public virtual void OnJustEnterBackState() { }
    public virtual void OnJustExitBackState() { }

    void OnDisable()
    {
        //foreach (ITriggerAIAction triggerAiAction in ActionArray)
        //{
        //    triggerAiAction.CancelAction(AIState.Idle);
        //}
        ChangeState(AIState.Idle);
    }
}
