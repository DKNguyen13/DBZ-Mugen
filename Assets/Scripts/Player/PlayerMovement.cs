using System.Collections;
using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float speed = 4f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float horizontalInput = 0;
    [SerializeField] private LayerMask groundLayer; // Layer ground
    [SerializeField] private float raycastDistance = 0.05f;// Độ dài tia Raycast
    [SerializeField] private Transform kiPoint,auraTransform; 
    [SerializeField] private GameObject[] kiSkills;
    [SerializeField] private GameObject auraKi;
    [SerializeField] private int maxCombo1, maxCombo2;
    [SerializeField] private int maxTransform, currentTransform;
    [SerializeField] private AudioClip[] soundEffects;
    [SerializeField] private PlayerStateEnum playerState = PlayerStateEnum.Idle;
    private GameObject aura = null;
    public bool isGround = true, isJumping = false, canAttack = true;
    private Animator animator;
    private Rigidbody2D rb;
    private PlayerState currentState;
    private AudioSource au;
    private StatusSystem status;
    private bool isPlayer = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        au = GetComponent<AudioSource>();
        status = GetComponent<StatusSystem>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeState(new IdleState(), PlayerStateEnum.Idle);
        maxCombo2 = maxCombo2 + maxCombo1;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        isGround = CheckGround();
        if(currentState!= null)
        {
            currentState.UpdateState(this);
        }
    }

    //Walk
    public void Walk()
    {
        if(!isGround) return;
        if (horizontalInput == 1)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (horizontalInput == -1)
        {
            transform.localScale = Vector3.one;
        }
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
    }

    //Jump
    public void Jump()
    {
        animator.Play("jump");
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        isJumping = true;
    }

    //Aura
    public void InstantiateAura(bool x)
    {
        if (x)
        {
            aura = Instantiate(auraKi, auraTransform.position, Quaternion.identity);
        }
        else
        {
            Destroy(aura);
        }
    }

    //Sound
    public void PlaySoundByIndex(int index)
    {
        if (index >= 0 && index < soundEffects.Length)
        {
            au.PlayOneShot(soundEffects[index]);
        }
    }
    
    public void SoundEffectAura(int index)
    {
        if(index >=0 && index < soundEffects.Length)
        {
            au.clip = soundEffects[index];
            au.Play();
        }
    }

    public void StopSound()
    {
        au.Stop();
    }
    //Check ground
    private bool CheckGround()
    {
        // Bắn 1 tia từ vị trí nhân vật xuống dưới
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, groundLayer);
        // Nếu Raycast chạm vào Layer mặt đất
        return hit.collider != null;
    }
    // Vẽ Raycast trong Scene để dễ debug
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * raycastDistance);
    }

    //Change State
    public void ChangeState(PlayerState changeState, PlayerStateEnum newState)
    {
        if (currentState != null) currentState.ExitState(this);
        currentState = changeState;
        currentState.EnterState(this);
        playerState = newState;
    }

    public void ChangeTransform()
    {
        StartCoroutine(Change());
    }

    IEnumerator Change()
    {
        yield return new WaitForSeconds(0.3f);
        GetComponent<PlayerTransforms>()?.ChangeTransform();
        Destroy(gameObject);
    }
    public bool CheckConditionTransform(bool next)
    {
        if (next)
        {
            if (status.CurrentKi == status.MaxKi && currentTransform < maxTransform) return true;
            return false;
        }
        else
        {
            if (currentTransform > 0) return true;
            return false;
        }
    }

    public void ChangeAnimationAura()
    {
        Animator auraAnimator = aura.GetComponent<Animator>();
        if (auraAnimator != null)
        {
            auraAnimator.Play("change_transform");
        }
        else
        {
            Debug.LogError("Animator không tồn tại trên auraKi!");
        }
    }

    //Ki ball
    public void KiBall(int x)
    {
        if (kiSkills.Length == 0 || kiPoint == null) return;
        float ki = x switch
        {
            1 => 0.1f,
            2 => 0.2f,
            3 => 0.5f,
            _ => 0
        };
        int useK = (int)(status.MaxKi * ki);
        if (CheckKi(useK))
        {
            status.UseKi(useK);
            if(x == 0)
            {
                GameObject kiBallGameobject = Instantiate(kiSkills[3], kiPoint.position, Quaternion.identity);
                ParticleSystem kiBall = kiBallGameobject.GetComponent<ParticleSystem>();
                if (kiBall == null)
                {
                    return;
                }
                var main = kiBall.velocityOverLifetime;
                if (transform.localScale.x == -1)
                {
                    main.x = 20;
                }
                else
                {
                    main.x = -20;
                }
                kiBall.Play();
            }
            else if (x == 1)
            {
                GameObject kiBallObject = Instantiate(kiSkills[0], kiPoint.position, Quaternion.identity);
                ParticleSystem kiBall = kiBallObject.GetComponent<ParticleSystem>();
                if (kiBall != null)
                {
                    var main = kiBall.velocityOverLifetime;
                    if (transform.localScale.x == -1)
                    {
                        main.x = 20;
                    }
                    else
                    {
                        main.x = -20;
                    }
                    kiBall.Play();
                }
            }
            else if (x == 2)
            {
                GameObject kiBallObject = Instantiate(kiSkills[1], kiPoint.position, Quaternion.identity);

                ParticleSystem kiBall = kiBallObject.GetComponent<ParticleSystem>();
                if (kiBall != null)
                {
                    var main = kiBall.velocityOverLifetime;
                    if (transform.localScale.x == -1)
                    {
                        main.x = 20;
                    }
                    else
                    {
                        main.x = -20;
                    }
                    kiBall.Play();
                }
            }
            else if(x == 3)
            {
                GameObject finalKi = Instantiate(kiSkills[2],kiPoint.position, Quaternion.identity);
            }
        }
        //Destroy(kiBallObject, kiBall.main.duration);
    }

    //Check ki 
    public bool CheckKi(int amount)
    {
        return status.CurrentKi >= amount;
    }

    public void PlayerAuraKi()
    {
        status.AuraKi((int)(status.MaxKi * 0.05f));
    }

    //End attack
    public void OnComboAttackEnd()
    {
        if (isPlayer)
        {
            canAttack = true;
        }
    }


    //Getter setter
    public float HorizontalInput => horizontalInput;
    public float Speed => speed;
    public float JumpForce => jumpForce;
    public Rigidbody2D Rb => rb;
    public Animator Animator => animator;
    //public ParticleSystem AuraKi => auraKi;
    public int MaxCombo1 { get =>  maxCombo1; }
    public int MaxCombo2 { get =>  maxCombo2; }
    public PlayerStateEnum PlayerState { get => playerState; set => playerState = value; }
}
