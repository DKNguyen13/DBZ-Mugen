using UnityEngine;

public class PlayerTransforms : MonoBehaviour
{
    [Header("Transform")]
    [SerializeField] private GameObject playerTransforms;

    public void ChangeTransform()
    {
        Instantiate(playerTransforms, transform.position,Quaternion.identity);
        return;
    }
}
