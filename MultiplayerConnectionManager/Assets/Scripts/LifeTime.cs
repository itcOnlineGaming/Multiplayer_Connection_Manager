using System.Collections;
using UnityEngine;

public class LifeTime : MonoBehaviour
{
    // Time in seconds before the object is destroyed
    public float lifetime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyAfterTime(lifetime));
    }

    private IEnumerator DestroyAfterTime(float time)
    {
        // Wait for the specified time
        yield return new WaitForSeconds(time);

        Destroy(gameObject);
    }
}
