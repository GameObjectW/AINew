using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISKillTrigger {
    void TriggerSkill(int num=0,Transform Target=null);
    void CancelSkill(int num = 0);
}
