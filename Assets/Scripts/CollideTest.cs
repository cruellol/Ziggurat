using UnityEngine;

public class CollideTest : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit");
    }

}
