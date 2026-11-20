using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        StatusUI.instance.Toggle(true);
    }
}
