using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// 该类继承IComparable接口，用于在优先级队列中进行排序，实现技能和其对应的CD与Weight的封装
/// </summary>
public class SkillCDAndWeight_New : IComparable
{
    private ITriggerAIAction skill;             //技能接口
    public int CD;
    public int CurrentCD=0;
    public int Weight;

    public ITriggerAIAction Skill
    {
        get
        {
            CurrentCD = 0;
            return skill;
        }

        set
        {
            skill = value;
        }
    }

    public SkillCDAndWeight_New(ITriggerAIAction skill, int cD, int weight)
    {
        Skill = skill;
        CD = cD;
        Weight = weight;
    }
    public void TriggerSkill() {
        
        Skill.TriggerAction();                      //触发技能
        
    }
    /// <summary>
    /// 判断技能是否冷却完毕并返回当前状态，每调用一次CD根据time进行增加
    /// </summary>
    /// <param name="time">当前CD增加的时长</param>
    /// <returns></returns>
    public bool SkillIsReady(int time=1) {
       // Debug.Log(CurrentCD);
        CurrentCD += time;
        return CurrentCD >= CD;
    }
    public int CompareTo(object obj)
    {
        SkillCDAndWeight_New bts = (SkillCDAndWeight_New)obj;
        if (this.Weight < bts.Weight)
        {
            return -1;
        }
        if (this.Weight >= bts.Weight)
        {
            return 1;
        }
        return 0;
    }
}
