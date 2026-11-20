using UnityEngine;

public class ContinuousSpawner : MonoBehaviour
{
    public GameObject prefab;
    public float interval = 1f;

    float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            Instantiate(prefab, transform.position, transform.rotation);
            timer = 0f;
        }
    }
}
