using UnityEngine;

public class AIAttack : MonoBehaviour
{
    [SerializeField] private GameObject[] attackPoints; // Các điểm tấn công của AI
    [SerializeField] private float attackRadius = 0.5f; // Bán kính tấn công
    [SerializeField] private LayerMask playerLayer; // Layer của người chơi
    [SerializeField] private int dmg = 50; // Sát thương của AI

    public void OnAttack(int index)
    {
        if (attackPoints == null || index < 0 || index >= attackPoints.Length)
        {
            Debug.LogError($"Lỗi: attack point index {index} không hợp lệ!");
            return;
        }

        // Kiểm tra va chạm với người chơi trong bán kính tấn công
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoints[index].transform.position, attackRadius, playerLayer);

        foreach (Collider2D player in hitPlayers)
        {
            StatusSystem ss = player.GetComponent<StatusSystem>();
            if (ss != null)
            {
                // Kiểm tra trạng thái của người chơi, không gây sát thương nếu đang phòng thủ
                PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
                if (playerMovement != null && playerMovement.PlayerState != PlayerStateEnum.Defending)
                {
                    ss.TakeDamage(dmg);
                    Debug.Log($"AI đánh trúng Player tại {attackPoints[index].transform.position}! HP còn lại: {ss.CurrentHp}");
                }
            }
        }
    }

    // Vẽ Gizmos để debug phạm vi tấn công
    public void OnDrawGizmos()
    {
        if (attackPoints == null) return;

        Gizmos.color = Color.blue; // Màu khác để phân biệt với Player
        foreach (GameObject point in attackPoints)
        {
            Gizmos.DrawSphere(point.transform.position, attackRadius);
        }
    }
}