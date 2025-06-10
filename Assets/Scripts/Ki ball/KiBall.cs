using System.Collections;
using UnityEngine;

public class KiBall : MonoBehaviour
{
    public int x = 1;
    public float dmgBonous = 0;
    private ParticleSystem particleSystem;
    private int emittedParticles = 0;
    private int maxParticles = 3;
    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        if(x == 1)
        {
            StartCoroutine(EmitParticles());
        }
    }
    private IEnumerator EmitParticles()
    {
        for (int i = 0; i < maxParticles; i++)
        {
            particleSystem.Emit(1);
            yield return new WaitForSeconds(0.2f); // Thời gian chờ giữa các lần bắn
        }
    }
    private void OnParticleCollision(GameObject other)
    {
        if (x == 1)
        {
            if (other.CompareTag("Ground"))
            {
                // Logic xử lý va chạm
                Debug.Log("Ki Ball đã va chạm với: " + other.name);
                emittedParticles++;
                // Kiểm tra nếu va chạm với ground
                if (emittedParticles == maxParticles)
                {
                    Destroy(gameObject);
                    Debug.Log("Đã bắn hết 3 hạt, hủy đối tượng!");
                }
            }
            else if (other.CompareTag("Enemy"))
            {
                Debug.Log("Ki Ball đã va chạm với: " + other.name);
                emittedParticles++;
                Rigidbody2D enemyRb = other.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    // Đẩy enemy ra một chút theo hướng của Ki Ball (giả sử Ki Ball đang di chuyển theo phương ngang)
                    Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
                    enemyRb.AddForce(knockbackDirection * 2f, ForceMode2D.Impulse);
                    StatusSystem statusSystem = other.GetComponent<StatusSystem>();
                    statusSystem.TakeDamage((int)(statusSystem.MaxHp * (0.03f + dmgBonous)));
                }
                if (emittedParticles == maxParticles)
                {
                    Destroy(gameObject);
                    Debug.Log("Đã bắn hết 3 hạt, hủy đối tượng!");
                }
            }
        }
        else if (x == 2)
        {
            // Logic xử lý va chạm
            Debug.Log("Ki Ball đã va chạm với: " + other.name);

            // Kiểm tra nếu va chạm với ground
            if (other.CompareTag("Ground"))
            {
                // Thực hiện logic khi va chạm với ground
                Debug.Log("Ki Ball đã va chạm với Ground!");
                Destroy(gameObject);
            }
            else if (other.CompareTag("Enemy"))
            {
                Debug.Log("Ki Ball đã va chạm với: " + other.name);
                Rigidbody2D enemyRb = other.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    // Đẩy enemy ra một chút theo hướng của Ki Ball (giả sử Ki Ball đang di chuyển theo phương ngang)
                    Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
                    enemyRb.AddForce(knockbackDirection * 3f, ForceMode2D.Impulse); // Điều chỉnh lực đẩy nếu cần
                    StatusSystem statusSystem = other.GetComponent<StatusSystem>();
                    statusSystem.TakeDamage((int)(statusSystem.MaxHp * (0.13f + dmgBonous)));
                    Destroy(gameObject);
                }
            }
        }
    }
}
