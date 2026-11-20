using UnityEngine;

public class MoveForever : MonoBehaviour
{
    public Vector3 direction = Vector3.forward;  
    public float speed = 5f;
    public float lifetime = 10f;                 

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += direction.normalized * speed * Time.deltaTime;
    }
}
