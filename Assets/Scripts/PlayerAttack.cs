using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject[] attackPoints;
    [SerializeField] private float attackRadius = 0.5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int dmg = 100;

    public void OnAttack(int index)
    {
        if (attackPoints == null || index < 0 || index >= attackPoints.Length)
        {
            Debug.LogError($"Lỗi: attack point index {index} không hợp lệ!");
            return;
        }

        // Tạo một vòng tròn để kiểm tra va chạm với kẻ địch
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoints[index].transform.position, attackRadius, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            AIController controller = enemy.GetComponent<AIController>();
            if (controller != null)
            {
                if(controller.AIState != PlayerStateEnum.Defending)
                {
                    StatusSystem ss = enemy.GetComponent<StatusSystem>();
                    if (ss != null)
                    {
                        ss.TakeDamage(dmg);
                        Debug.Log($"Đánh trúng Enemy tại {attackPoints[index].transform.position}! HP còn lại: {ss.CurrentHp}");
                    }
                }
            }
        }
        //Debug.Log($"Attack {currentAttackPoint}");
    }

    public void OnDrawGizmos()
    {
        if (attackPoints == null) return;

        Gizmos.color = Color.red;
        foreach (GameObject point in attackPoints)
        {
            Gizmos.DrawSphere(point.transform.position, attackRadius);
        }
    }

}
