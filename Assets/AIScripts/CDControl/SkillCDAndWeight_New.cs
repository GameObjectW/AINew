using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SkillCDAndWeight_New : IComparable
{
    private ITriggerAIAction skill;
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
        
        Skill.TriggerAction();
        
    }
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
