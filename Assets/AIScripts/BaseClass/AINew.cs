using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// AI拥有的状态
/// </summary>
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
/// <summary>
/// AI的基类
/// </summary>
public abstract class AINew : MonoBehaviour
{
    private AIState nowState;                   //AI当前状态

    public AIState LastState;                   //上一个状态，主要时如果当前状态的功能实现与上一个状态有关联时有用（目前没用）

    public Transform Target;                    //AI的目标，这里直接通过Unity拖拽添加

    public float AttackDistance;                //怪物攻击距离

    public float backThreshold;                 //怪物后退阈值，怪物距离目标太近可以进行后退操作


    protected NavMeshAgent NV;                  //寻路组件

    protected float CurrentSelfToTargetDis;     //目标与自身在xz平面内的直线距离

    private ITriggerAIAction _idleInitAction;   //各种节点接口，可丰富功能后在子类中进行挂载
    private ITriggerAIAction _attackAction;
    private ITriggerAIAction _backAction;
    private ITriggerAIAction _injuredAction;
    private ITriggerAIAction _runAction;
    private ITriggerAIAction _dieAction;
    private ITriggerAIAction _skillOneAction;


    protected Renderer[] mr;                    //AI身上的渲染组件，这里获取用来实现溶解效果（需要相应的Shader配合）
    protected Collider[] colliders;             //AI身上碰撞器，在某些时刻可能需要AI不接受碰撞信息
    protected Animator Ani;                     //动画组件
    protected List<SkillCDAndWeight_New> SkillList = new List<SkillCDAndWeight_New>();                          //存放所有技能的列表，用于AI技能的重置
    protected PropertyQueue<SkillCDAndWeight_New> PropertyQueue = new PropertyQueue<SkillCDAndWeight_New>();    //优先级队列，用于取出当前可释放的优先级最高的技能

    private Dictionary<AIState, ITriggerAIAction> _actionDic;                                                   //保存功能节点与状态的一对一映射
    
    private List<SkillCDAndWeight_New> _cdCalList = new List<SkillCDAndWeight_New>();                            //CD冷却列表，需要冷却的技能在释放时都会添加到其中

    // Use this for initialization
    public virtual void Awake()
    {
        NV = GetComponent<NavMeshAgent>();
        mr = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();
        Ani = GetComponent<Animator>();
    }

    
    /// <summary>
    /// 由于AI使用对象池存取，所以通过setActive（false）来进行初始化
    /// </summary>
    void OnEnable()
    {
        foreach (SkillCDAndWeight_New skillCdAndWeight in SkillList)    //将列表中所有的技能重新添加到CD冷却列表中重新开始冷却
        {
            _cdCalList.Add(skillCdAndWeight);
        }
        StartCoroutine(BaseUpdate());                                   //基本逻辑更新判断（未优化）
        StartCoroutine(CalCD());                                        //控制技能冷却协程
    }

    public virtual void Start ()
	{
       

    }

    private IEnumerator BaseUpdate()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(()=>DieAction!=null);
        _actionDic = new Dictionary<AIState, ITriggerAIAction>()            //保存状态与功能节点的映射（主要用来消除冗长的Switch）
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
        //简单的AI逻辑判断，这一部分后期可以通过行为树来优化
        while (true)
        {
            if (Target == null)
            {
                ChangeState(AIState.Idle);
                continue;
            }
            CurrentSelfToTargetDis = DistanceSelfToTarget(Target);

            if (_isCanChangeState)
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
    /// <summary>
    /// 计算目标与自己在xz平面内的直线距离
    /// </summary>
    /// <param name="target">目标点</param>
    /// <returns></returns>
    public float DistanceSelfToTarget(Transform target)
    {
        Vector3 SelfPos = transform.position;
        SelfPos.y = 0;
        Vector3 TargetPos = target.position;
        TargetPos.y = 0;
        return Vector3.Distance(SelfPos, TargetPos);
    }
    /// <summary>
    /// 计算目标与自己在xz平面内的直线距离
    /// </summary>
    /// <param name="target">目标点三维向量</param>
    /// <returns></returns>
    public float DistanceSelfToTarget(Vector3 target)
    {
        Vector3 SelfPos = transform.position;
        SelfPos.y = 0;
        Vector3 TargetPos = target;
        TargetPos.y = 0;
        return Vector3.Distance(SelfPos, TargetPos);
    }
    /// <summary>
    /// AI状态的切换，并根据不同的状态触发相应的功能节点
    /// </summary>
    /// <param name="state">AI需要切换到的状态</param>
    private void ChangeState(AIState state)
    {
        //当前状态死亡，但是需要切换非Idle状态是不允许的
        if ( NowState == AIState.Die&&state!=AIState.Idle)
        {
            return;
        }
        //切换状态的另一个条件，就是当前状态能够被当前状态替代
        if (!SetCurrentActionStop(nowState,state))
        {
            return;
        }
        LastState = NowState;
        NowState = state;

        _actionDic[NowState].TriggerAction();       //替换状态成功，触发功能节点
    }
    /// <summary>
    /// 提供当前状态和新状态，并返回当前状态是否允许被取消
    /// </summary>
    /// <param name="state">当前状态</param>
    /// <param name="newState">新状态</param>
    /// <returns></returns>
    public bool SetCurrentActionStop(AIState state, AIState newState)
    {
        return _actionDic[state].CancelAction(newState);
    }

    private bool _isCanChangeState = true;

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
                //状态变化后的相应回调
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
            if (value != null)
            {
                //如果动态修改技能，相应的修改相应的映射
                _actionDic[AIState.Attack] = value;
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
            if (value != null)
            {
                //如果动态修改技能，相应的修改相应的映射
                _actionDic[AIState.Back] = value;
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
            if (value != null)
            {
                //如果动态修改技能，相应的修改相应的映射
                _actionDic[AIState.Injured] = value;
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
            if (value != null)
            {
                //如果动态修改技能，相应的修改相应的映射
                _actionDic[AIState.Run] = value;
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
            if (value != null)
            {
                //如果动态修改技能，相应的修改相应的映射
                _actionDic[AIState.Die] = value;
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
            if (value != null)
            {
                //如果动态修改技能，相应的修改相应的映射
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
            if (value != null)
            {
                //如果动态修改技能，相应的修改相应的映射
                _actionDic[AIState.Idle] = value;
            }
            _idleInitAction = value;
        }
    }

    /// <summary>
    /// 设置当前AI是否允许切换状态
    /// </summary>
    /// <param name="flag"></param>
    public void SetComplete(bool flag)
    {
        _isCanChangeState = flag;
    }
    /// <summary>
    /// 获取可切换状态
    /// </summary>
    /// <returns></returns>
    public bool GetComplete()
    {
        return _isCanChangeState;
    }
    /// <summary>
    /// 测试（通过Button）手动触发被击状态
    /// </summary>
    public void TestInjured()
    {
        ChangeState(AIState.Injured);
    }
    /// <summary>
    /// 测试（通过Button）手动触发死亡状态
    /// </summary>
    public void TestDie()
    {
        ChangeState(AIState.Die);
    }
    /// <summary>
    /// 测试（通过Button）手动触发技能状态
    /// </summary>
    public void TestSkill()
    {
        ChangeState(AIState.Skill);
    }
    /// <summary>
    /// 根据两个状态来调用相应的进入和退出函数，在子类中可重写这些函数来实现一些功能
    /// </summary>
    /// <param name="NewState"></param>
    /// <param name="OldState"></param>
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
    /// <summary>
    /// CD控制协程，通过每秒一次的速度来遍历冷却列表，并将冷却完毕的技能添加到优先级队列中
    /// </summary>
    /// <returns></returns>
    IEnumerator CalCD()
    {
        List<SkillCDAndWeight_New> WillDel = new List<SkillCDAndWeight_New>();
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (_cdCalList.Count > 0)
            {
                foreach (SkillCDAndWeight_New item in _cdCalList)
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
                        _cdCalList.Remove(item);
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
    /// <summary>
    /// 手动返回一个当前可释放的技能
    /// </summary>
    /// <returns></returns>
    protected SkillCDAndWeight_New GetOneSkillInPropertyList()
    {
        SkillCDAndWeight_New skillCdAndWeightNew = PropertyQueue.Pop();
        return skillCdAndWeightNew;
    }
    /// <summary>
    /// 手动将一个技能添加到冷却列表中
    /// </summary>
    /// <param name="skill">要冷却的技能</param>
    protected void AddSkillTocdList(SkillCDAndWeight_New skill)
    {
        if (skill==null)
        {
            return;
        }
        _cdCalList.Add(skill);
    }
    /// <summary>
    /// AI被消灭（setActive(false)）后的一系列初始化
    /// </summary>
    void OnDisable()
    {
        StopCoroutine(CalCD());
        _cdCalList.Clear();
        PropertyQueue.clear();
        ChangeState(AIState.Idle);
    }
}
