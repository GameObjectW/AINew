using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITriggerAIAction
{
    void TriggerAction();
    bool CancelAction(AIState state);
}
