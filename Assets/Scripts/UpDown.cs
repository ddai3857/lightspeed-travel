using UnityEngine;

public class UpDown : MonoBehaviour
{
    public float speed = 2f;
    public float height = 3f;
    public float delay = 0f;

    Vector3 startPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float offset = Mathf.PingPong((Time.time + delay) * speed, height);
        transform.position = startPos + new Vector3(0, offset, 0);

    }
}
