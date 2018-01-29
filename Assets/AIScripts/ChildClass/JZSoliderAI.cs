using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JZSoliderAI : AINew {


    public override void Awake()
    {
        base.Awake();
        StartCoroutine(InitActions());
    }

    IEnumerator InitActions()
    {
        yield return new WaitForSeconds(0.01f);
        IdleInitAction = new IdleAction(this, "Idle", NV, mr, colliders, Ani);
        AttackAction = new NormalAttack(this, new string[]{ "劈砍1", "劈砍3" }, 0.44f, NV, Ani);
        BackAction = new TakeBack(this, "", NV, transform);
        InjuredAction = new InjuredAction(this, "GetHit", 1, NV, Ani);
        RunAction = new RunAction(this, "Run", NV, Ani);
        DieAction = new DieAction(this, "Die", mr, NV, transform, colliders, Ani,0.2f);
    }



}
