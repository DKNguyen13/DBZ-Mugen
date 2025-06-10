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
            ai.AttackPlayer();
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
    private float time = 0.5f; // Thời gian giữa các đòn combo
    private float timeWait = 0;
    private AIAttack aiAttack;

    public override void EnterState(AIController ai)
    {
        aiAttack = ai.GetComponent<AIAttack>();
        if (aiAttack == null)
        {
            Debug.LogError("AIAttack component không được tìm thấy trên AI!");
            return;
        }
        countIndex = 1;
        timeWait = Time.time;
        ai.Animator.Play("atk" + countIndex);
        aiAttack.OnAttack(countIndex - 1); // Gọi tấn công với index tương ứng
    }

    public override void UpdateState(AIController ai)
    {
        if (ai.Player == null) // Kiểm tra player có tồn tại không
        {
            ai.ChangeState(new AIIdleState());
            return;
        }

        // Nếu không còn gần người chơi, di chuyển về phía người chơi
        if (!ai.IsCloseToPlayer())
        {
            // Di chuyển về phía người chơi
            ai.transform.position = Vector2.MoveTowards(ai.transform.position, ai.Player.position, ai.speed * Time.deltaTime);
            // Xoay AI theo hướng người chơi
            if (ai.Player.position.x > ai.transform.position.x)
            {
                ai.transform.localScale = new Vector3(-1, 1, 1); // Hướng phải
            }
            else
            {
                ai.transform.localScale = Vector3.one; // Hướng trái
            }
            // Chuyển về trạng thái di chuyển nếu quá xa
            ai.ChangeState(new AIMoveState());
        }
        else
        {
            // Ở gần, tiếp tục combo tấn công
            if (Time.time - timeWait > time)
            {
                countIndex++;
                if (countIndex > ai.maxComboAttack)
                {
                    countIndex = 1;
                }
                ai.Animator.Play("atk" + countIndex);
                aiAttack.OnAttack(countIndex - 1); // Gọi tấn công cho mỗi animation
                timeWait = Time.time;
            }
        }
    }

    public override void ExitState(AIController ai)
    {
        countIndex = 1; // Reset combo khi thoát trạng thái
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
