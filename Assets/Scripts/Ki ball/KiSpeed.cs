using UnityEngine;

public class KiSpeed : MonoBehaviour
{
    public float speedKi = 6f;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (FindAnyObjectByType<PlayerMovement>()?.transform.localScale.x == -1)
        {
            rb.velocity = new Vector2(speedKi, 0);
        }
        else
        {
            rb.velocity = new Vector2(-speedKi, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("va cham enemy");
            StatusSystem enemy = collision.GetComponent<StatusSystem>();
            if (enemy!=null)
            {
                enemy.TakeDamage((int)(enemy.MaxHp * 0.4f));
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
