using UnityEngine;

public class AIController : MonoBehaviour
{
    private Animator animator;
    private Transform player;
    private AIState currentState;

    public float speed = 5f, distance = 4f;
    public float maxComboAttack = 3;
    public PlayerStateEnum aiState = PlayerStateEnum.Idle;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        ChangeState(new AIIdleState());
        player = FindAnyObjectByType<PlayerMovement>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }

    public void ChangeState(AIState newState)
    {
        if(currentState != null)
        {
            currentState.ExitState(this);
        }
        currentState = newState;
        currentState.EnterState(this);
    }
    public bool IsCloseToPlayer()
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.position) < distance; // Khoảng cách nhỏ hơn để phù hợp 1v1
    }

    public void MoveTowardsPlayer()
    {
        if (player == null) return;
        if (!IsCloseToPlayer()) // Chỉ di chuyển khi chưa đến tầm đánh
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
        else
        {
            //ChangeState(new AIAttackState()); // Chuyển sang trạng thái tấn công
        }
    }

    public void OnComboAttackEnd()
    {

    }

    public void AttackPlayer()
    {
        Debug.Log("AI Attack!");
    }

    //Getter setter
    public Animator Animator { get => animator;  }
    public PlayerStateEnum AIState { get => aiState; set => aiState = value; }
}
