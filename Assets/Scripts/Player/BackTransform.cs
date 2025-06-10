using UnityEngine;

public class BackTransform : MonoBehaviour
{
    public GameObject backTransform;

    public void BackChangeTransform()
    {
        Instantiate(backTransform,transform.position, Quaternion.identity);
        Destroy(gameObject);
        return;
    }
}
