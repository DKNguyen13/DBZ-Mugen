using UnityEngine;

public abstract class PlayerState
{
    public abstract void EnterState(PlayerMovement player);
    public abstract void UpdateState(PlayerMovement player);
    public abstract void ExitState(PlayerMovement player);
}

//Idle
public class IdleState : PlayerState
{
    public override void EnterState(PlayerMovement player)
    {
        player.Animator.Play("Idle");
    }

    public override void UpdateState(PlayerMovement player)
    {
        if (player.HorizontalInput != 0)
        {
            player.ChangeState(new WalkState(), PlayerStateEnum.Walking);
        }
        else if (player.isGround && Input.GetKeyDown(KeyCode.UpArrow))
        {
            player.ChangeState(new JumpState(), PlayerStateEnum.Jumping);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            player.ChangeState(new ComboState(), PlayerStateEnum.Attacking);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            player.ChangeState(new Combo1State(), PlayerStateEnum.Attacking);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            player.ChangeState(new DefenseState(), PlayerStateEnum.Defending);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            player.ChangeState(new KiAuraState(), PlayerStateEnum.Aura);
        }
        else if(Input.GetKey(KeyCode.Q))
        {
            player.ChangeState(new KiBallState(),PlayerStateEnum.UsingKiBall);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            player.ChangeState(new KiSmallBallState(), PlayerStateEnum.UsingSmallKiBall);
        }
        else if(Input.GetKey(KeyCode.DownArrow))
        {
            player.ChangeState(new SitDownState(), PlayerStateEnum.Sitting);
        }
        else if(player.CheckConditionTransform(false) && Input.GetKeyDown(KeyCode.Z))
        {
            player.Animator.Play("back_transform");
            return;
        }
    }
    public override void ExitState(PlayerMovement player)
    {
    }

}

//Walk
public class WalkState : PlayerState
{
    public override void EnterState(PlayerMovement player)
    {
        player.Animator.Play("walk");
    }
    public override void UpdateState(PlayerMovement player)
    {
        if (player.HorizontalInput == 0)
        {
            player.ChangeState(new IdleState(), PlayerStateEnum.Idle);
        }
        else if (player.isGround && Input.GetKeyDown(KeyCode.UpArrow))
        {
            player.ChangeState(new JumpState(), PlayerStateEnum.Jumping);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            player.Rb.velocity = Vector2.zero;
            player.ChangeState(new ComboState(), PlayerStateEnum.Attacking);
            return;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            player.Rb.velocity = Vector2.zero;
            player.ChangeState(new Combo1State(), PlayerStateEnum.Attacking);
            return;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            player.ChangeState(new DefenseState(), PlayerStateEnum.Defending);
            player.Rb.velocity = Vector2.zero;
            return;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            player.ChangeState(new KiAuraState(), PlayerStateEnum.Aura);
            player.Rb.velocity = Vector2.zero;
            return;
        }
        else if(Input.GetKey(KeyCode.LeftShift))
        {
            player.ChangeState(new DashState(),PlayerStateEnum.Dashing);
        }
        else if(Input.GetKey(KeyCode.Q))
        {
            player.Rb.velocity = Vector2.zero;
            player.ChangeState(new KiBallState(), PlayerStateEnum.UsingKiBall);
            return;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            player.Rb.velocity = Vector2.zero;
            player.ChangeState(new KiSmallBallState(), PlayerStateEnum.UsingSmallKiBall);
            return;
        }
        player.Walk();
    }
    public override void ExitState(PlayerMovement player)
    {
    }
}

//Jump
public class JumpState : PlayerState
{
    public override void EnterState(PlayerMovement player)
    {
        player.Animator.Play("jump");
        player.Jump();
    }
    public override void UpdateState(PlayerMovement player)
    {
        if(player.isGround && !player.isJumping)
        {
            player.ChangeState(new IdleState(), PlayerStateEnum.Idle);
        }
        if(!player.isGround && Input.GetKeyDown(KeyCode.A))
        {
            player.ChangeState(new AttackJumpState(), PlayerStateEnum.Attacking);
        }
        if (player.Rb.velocity.y < 0)
        {
            player.isJumping = false;
        }
    }
    public override void ExitState(PlayerMovement player)
    {
    }
}

//Combo
public class ComboState : PlayerState
{
    private int comboIndex = 0;
    private float lastComboAttackTime = 0;
    private float comboResetTime = 0.65f;
    public override void EnterState(PlayerMovement player)
    {
        comboIndex = 1;
        lastComboAttackTime = Time.time;
        PlayerComboAnimation(player);
    }
    public override void UpdateState(PlayerMovement player)
    {
        if(Time.time - lastComboAttackTime > comboResetTime || comboIndex > player.MaxCombo1)
        {
            player.ChangeState(new IdleState(), PlayerStateEnum.Idle);
            return;
        }
        if(Input.GetKeyDown(KeyCode.A) && player.canAttack)
        {
            comboIndex++;
            lastComboAttackTime = Time.time;
            if (comboIndex > player.MaxCombo1)
            {
                comboIndex = 1;
            }
            PlayerComboAnimation(player);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            player.ChangeState(new Combo1State(), PlayerStateEnum.Attacking);
        }
    }

    public override void ExitState(PlayerMovement player)
    {
        comboIndex = 0;
        player.canAttack = false;
    }

    public void PlayerComboAnimation(PlayerMovement player)
    {
        player.canAttack = false;
        player.Animator.CrossFade("atk" + comboIndex, 0.1f);
    }
}


//Combo 1
public class Combo1State : PlayerState
{
    private int startIndex;
    private int comboIndex = 0;
    private float lastComboAttackTime = 0;
    private float comboResetTime = 0.8f;

    public override void EnterState(PlayerMovement player)
    {
        startIndex = player.MaxCombo1;
        comboIndex = startIndex + 1;
        lastComboAttackTime = Time.time;
        PlayerComboAnimation(player);
    }

    public override void UpdateState(PlayerMovement player)
    {
        if(Time.time - lastComboAttackTime > comboResetTime || comboIndex > player.MaxCombo2)
        {
            player.ChangeState(new IdleState(), PlayerStateEnum.Idle);
            return;
        }
        if (Input.GetKeyDown(KeyCode.S) && player.canAttack)
        {
            comboIndex++;
            lastComboAttackTime = Time.time;
            if (comboIndex > player.MaxCombo2)
            {
                comboIndex = startIndex + 1;
            }
            PlayerComboAnimation(player);
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            player.ChangeState(new ComboState(), PlayerStateEnum.Attacking);
        }
    }

    public override void ExitState(PlayerMovement player)
    {
        comboIndex = startIndex;
        player.canAttack = false;
    }

    public void PlayerComboAnimation(PlayerMovement player)
    {
        player.canAttack = false;
        int index = comboIndex;
        player.Animator.CrossFade("atk" + index,0.1f);
    }
}

//Defense
public class DefenseState : PlayerState
{
    private bool isDownDefense = false;
    public override void EnterState(PlayerMovement player)
    {
        isDownDefense = Input.GetKey(KeyCode.DownArrow);
        player.Animator.Play(isDownDefense ? "down_defense" : "defense");
    }

    public override void UpdateState(PlayerMovement player)
    {
        if(!Input.GetKey(KeyCode.D))
        {
            player.ChangeState(new IdleState(), PlayerStateEnum.Idle);
            return;
        }
        // Nếu đang không ở DownDefense mà giữ DownArrow thì đổi animation
        if (!isDownDefense && Input.GetKey(KeyCode.DownArrow))
        {
            isDownDefense = true;
            player.Animator.Play("down_defense");
        }
        // Nếu đang ở DownDefense mà thả DownArrow thì đổi lại
        else if (isDownDefense && !Input.GetKey(KeyCode.DownArrow))
        {
            isDownDefense = false;
            player.Animator.Play("defense");
        }
    }
    public override void ExitState(PlayerMovement player)
    {
    }
}

//Aura
public class KiAuraState : PlayerState
{
    private bool isAuraKi = false;
    private float time, waitTime = 0.3f;
    private float elapsedTime = 0f, transformTime = 5f;
    public override void EnterState(PlayerMovement player)
    {
        /*
        player.AuraKi.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        player.AuraKi.transform.position = player.transform.position;
        player.AuraKi.Play();
        */
        player.InstantiateAura(true);
        player.Animator.Play("aura");
        time = Time.time;
    }
    public override void UpdateState(PlayerMovement player)
    {
        isAuraKi = Input.GetKey(KeyCode.E);
        
        if(!isAuraKi)
        {
            player.ChangeState(new IdleState(), PlayerStateEnum.Idle);
            player.StopSound();
            return;
        }
        if (Input.GetKey(KeyCode.A) && player.CheckKi(60))
        {
            player.ChangeState(new FinalKiBall(), PlayerStateEnum.UsingKiBall);
        }
        if (Time.time - time > waitTime)
        {
            player.PlayerAuraKi();
            time = Time.time;
        }
        if(player.CheckConditionTransform(true))
        {
            elapsedTime += Time.deltaTime;
            if(elapsedTime > 2f)
            {
                //player.SoundEffect()
                player.ChangeAnimationAura();
            }
            if (elapsedTime >= transformTime)
            {
                player.ChangeState(new ChangeTransformState(), PlayerStateEnum.Transform);
                return;
            }
        }
    }
    public override void ExitState(PlayerMovement player)
    {
        player.InstantiateAura(false);
        //player.AuraKi.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}

//Dash
public class DashState : PlayerState
{
    private float dashSpeed = 11f;
    private bool isWalking = false;

    public override void EnterState(PlayerMovement player)
    {
        player.Animator.Play("dash");
        player.Rb.velocity = new Vector2(dashSpeed * player.HorizontalInput, player.Rb.velocity.y);
    }
    public override void UpdateState(PlayerMovement player)
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            player.Rb.velocity = Vector2.zero;
            player.ChangeState(new ComboState(), PlayerStateEnum.Attacking);
            return;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            player.Rb.velocity = Vector2.zero;
            player.ChangeState(new Combo1State(), PlayerStateEnum.Attacking);
            return;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            player.ChangeState(new DefenseState(), PlayerStateEnum.Defending);
            player.Rb.velocity = Vector2.zero;
            return;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            player.ChangeState(new KiAuraState(), PlayerStateEnum.Aura);
            player.Rb.velocity = Vector2.zero;
            return;
        }
        isWalking = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);
        if (isWalking)  
        {
            // Cập nhật vận tốc khi giữ LeftShift
            if (Input.GetKey(KeyCode.LeftShift))
            {
                player.Rb.velocity = new Vector2(dashSpeed * player.HorizontalInput, player.Rb.velocity.y);

                // Đổi hướng của nhân vật
                if (player.HorizontalInput == 1)
                {
                    player.transform.localScale = new Vector3(-1, 1, 1);  // Hướng phải
                }
                else if (player.HorizontalInput == -1)
                {
                    player.transform.localScale = Vector3.one;  // Hướng trái
                }
                return;
            }
            else
            {
                // Nếu không còn nhấn LeftShift, chuyển về trạng thái Walk
                player.ChangeState(new WalkState(), PlayerStateEnum.Walking);
            }
        }
        else
        {
            player.Rb.velocity = Vector2.zero;
            player.ChangeState(new IdleState(), PlayerStateEnum.Idle);
            return;
        }
    }
    public override void ExitState(PlayerMovement player)
    {

    }
}

//Sit down
public class SitDownState : PlayerState
{
    private bool isAttacking = false;
    public override void EnterState(PlayerMovement player)
    {
        player.Animator.Play("sitdown");
    }
    public override void UpdateState(PlayerMovement player)
    {
        if (isAttacking)
        {
            if (player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                isAttacking = false;
                player.Animator.Play("sitdown");
            }
            return;
        }
        if (player.HorizontalInput != 0)
        {
            player.ChangeState(new WalkState(),PlayerStateEnum.Walking);
        }
        else if (!Input.GetKey(KeyCode.DownArrow))
        {
            player.ChangeState(new IdleState(), PlayerStateEnum.Idle);
            return ;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            player.ChangeState(new KiAuraState(), PlayerStateEnum.Aura);
        }
        else if(Input.GetKey(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.A))
        {
            player.ChangeState(new SitAttackState(), PlayerStateEnum.Attacking);
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            player.Animator.Play("atk6");
            isAttacking = true;
        }
    }
    public override void ExitState(PlayerMovement player)
    {
    }

}

//Medium ki ball
public class KiBallState : PlayerState
{
    private float timeToChangeState = 0.6f;
    private float timeCount = 0;

    public override void EnterState(PlayerMovement player)
    {
        if (player.CheckKi(20))
        {
            player.Animator.Play("ki_ball");
            timeCount = 0;
        }
    }

    public override void UpdateState(PlayerMovement player)
    {
        timeCount += Time.deltaTime;
        if(timeCount >= timeToChangeState)
        {
            if (player.HorizontalInput != 0)
            {
                player.ChangeState(new WalkState(), PlayerStateEnum.Walking);
            }
            else
            {
                player.ChangeState(new IdleState(), PlayerStateEnum.Idle);
            }
        }
    }

    public override void ExitState(PlayerMovement player)
    {
    }
}

//Small ki ball
public class KiSmallBallState : PlayerState
{
    private float timeToChangeState = 1f;
    private float timeCount = 0;
    public override void EnterState(PlayerMovement player)
    {
        if (player.CheckKi(10))
        {
            player.Animator.Play("ki_smallBall");
            timeCount = 0;
        }
    }
    public override void UpdateState(PlayerMovement player)
    {
        timeCount += Time.deltaTime;
        if (timeCount >= timeToChangeState)
        {
            if (player.HorizontalInput != 0)
            {
                player.ChangeState(new WalkState(), PlayerStateEnum.Walking);
            }
            else
            {
                player.ChangeState(new IdleState(), PlayerStateEnum.Idle);
            }
        }
    }
    public override void ExitState(PlayerMovement player)
    {
    }
}

//Attack jump
public class AttackJumpState : PlayerState
{
    public override void EnterState(PlayerMovement player)
    {
        player.Animator.Play("atk8");
        player.Rb.velocity = new Vector2(player.Rb.velocity.x, -6f);
    }

    public override void UpdateState(PlayerMovement player)
    {
        if (player.isGround)
        {
            player.ChangeState(new IdleState(), PlayerStateEnum.Idle);
            return;
        }
    }
    public override void ExitState(PlayerMovement player)
    {
        player.Rb.velocity = new Vector2(player.HorizontalInput * player.Speed, player.Rb.velocity.y);
    }
}

//Sit attack
public class SitAttackState : PlayerState
{
    public override void EnterState(PlayerMovement player)
    {
        player.Animator.Play("atk9");
    }
    public override void UpdateState(PlayerMovement player)
    {
        if(player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
           if(Input.GetKeyDown(KeyCode.A))
           {
                player.ChangeState(new ComboState(), PlayerStateEnum.Attacking);
           }
           else if (Input.GetKeyDown(KeyCode.S))
           {
                player.ChangeState(new ComboState(), PlayerStateEnum.Attacking);
           }
           if(Input.GetKey(KeyCode.DownArrow))
           {
                player.ChangeState(new SitDownState(), PlayerStateEnum.Sitting);
           }
           else
           {
                player.ChangeState(new IdleState(), PlayerStateEnum.Idle);
                return;
           }
        }
        else if(player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >=0.6f && player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                player.Animator.Play("atk6");
            }
        }
    }
    public override void ExitState(PlayerMovement player)
    {
    }
}

public class ChangeTransformState : PlayerState
{
    public override void EnterState(PlayerMovement player)
    {
        player.Animator.Play("transform");
        player.InstantiateAura(true);
        player.ChangeAnimationAura();
        player.Animator.Play("aura");
    }

    public override void UpdateState(PlayerMovement player)
    {
        if(player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            player.InstantiateAura(false);
            player.ChangeTransform();
            return;
        }
    }
    public override void ExitState(PlayerMovement player)
    {
    }

}

//Final ki ball
public class FinalKiBall : PlayerState
{
    private float timeToChangeState = 1f, timeCount = 0;

    public override void EnterState(PlayerMovement player)
    {
        if (player.CheckKi(50))
        {
            player.Animator.Play("finalKiBall");
            timeCount = 0;
        }
        else return;
    }
    public override void UpdateState(PlayerMovement player)
    {
        timeCount += Time.deltaTime;
        if(timeCount >= timeToChangeState)
        {
            if (player.HorizontalInput != 0)
            {
                player.ChangeState(new WalkState(), PlayerStateEnum.Walking);
            }
            else
            {
                player.ChangeState(new IdleState(), PlayerStateEnum.Idle);
            }
        }
    }
    public override void ExitState(PlayerMovement player)
    {
    }


}

