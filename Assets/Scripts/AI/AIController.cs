using UnityEngine;

public class AIController : MonoBehaviour
{
    private Animator animator;
    private Transform player;
    private AIState currentState;

    public float speed = 5f; // Tốc độ di chuyển của AI
    public float distance = 1.5f; // Khoảng cách để bắt đầu tấn công
    public int maxComboAttack = 3; // Số đòn combo tối đa
    public PlayerStateEnum aiState = PlayerStateEnum.Idle;
    [SerializeField] private AudioClip[] soundEffects;
    private AudioSource au;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        au = GetComponent<AudioSource>();
        if (au == null)
        {
            Debug.LogError("AudioSource component không được tìm thấy trên AI!");
        }
        ChangeState(new AIIdleState());
        player = FindAnyObjectByType<PlayerMovement>().transform;
        if (player == null)
        {
            Debug.LogError("Không tìm thấy Player trong scene!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return; // Tránh lỗi nếu player không tồn tại
        currentState.UpdateState(this);
    }

    public void ChangeState(AIState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState(this);
        }
        currentState = newState;
        currentState.EnterState(this);
        aiState = PlayerStateEnum.Idle; // Cập nhật trạng thái, có thể điều chỉnh tùy logic
    }

    public bool IsCloseToPlayer()
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.position) < distance; // Kiểm tra khoảng cách
    }

    public void MoveTowardsPlayer()
    {
        if (player == null) return;
        if (!IsCloseToPlayer()) // Chỉ di chuyển khi chưa đến tầm đánh
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            // Xoay AI theo hướng người chơi
            if (player.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1); // Hướng phải
            }
            else
            {
                transform.localScale = Vector3.one; // Hướng trái
            }
        }
        else
        {
            ChangeState(new AIAttackState()); // Chuyển sang trạng thái tấn công khi đủ gần
        }
    }

    public void AttackPlayer()
    {
        ChangeState(new AIAttackState()); // Chuyển sang trạng thái tấn công
    }

    // Hàm phát âm thanh cho Animation Event
    public void PlaySoundByIndex(int index)
    {
    }

    // End attack
    public void OnComboAttackEnd()
    {
        // Có thể thêm logic khi combo kết thúc, ví dụ: chuyển về Idle
        ChangeState(new AIIdleState());
    }

    // Getter setter
    public Animator Animator { get => animator; }
    public Transform Player { get => player; } // Thêm getter cho player
    public PlayerStateEnum AIState { get => aiState; set => aiState = value; }
}