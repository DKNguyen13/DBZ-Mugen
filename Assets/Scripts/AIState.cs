using System.Collections;
using UnityEngine;
public abstract class AIState
{
    public abstract void EnterState(AIController ai);
    public abstract void UpdateState(AIController ai);
    public abstract void ExitState(AIController ai);
}

//AI idle
public class AIIdleState : AIState
{
    public override void EnterState(AIController ai)
    {
        ai.Animator.Play("Idle");
    }



    public override void UpdateState(AIController ai)
    {
        if (!ai.IsCloseToPlayer())
        {
            ai.ChangeState(new AIMoveState());
        }
        else
        {
            //ai.AttackPlayer();
        }
    }
    public override void ExitState(AIController ai)
    {
    }
}

//AI Attack
public class AIAttackState : AIState
{
    private int countIndex = 1;
    public float time = 0.5f;
    public float timeWait = 0;
    public override void EnterState(AIController ai)
    {
        timeWait = Time.time;
        ai.Animator.Play("atk" + countIndex);
    }

    public override void UpdateState(AIController ai)
    {

        if (!ai.IsCloseToPlayer())
        {
            ai.ChangeState(new AIMoveState());
        }
        else
        {
            if(Time.time - timeWait >time)
            {
                countIndex++;
                if (countIndex > 6)
                {
                    countIndex = 1;
                }
                ai.Animator.Play("atk" + countIndex);
                timeWait = Time.time;
            }
            
        }
    }

    public override void ExitState(AIController ai)
    {
    }
}


//AI Move
public class AIMoveState : AIState
{
    public override void EnterState(AIController ai)
    {
        ai.Animator.Play("walk");
    }

    public override void UpdateState(AIController ai)
    {
        ai.MoveTowardsPlayer();
    }

    public override void ExitState(AIController ai)
    {
    }
}

