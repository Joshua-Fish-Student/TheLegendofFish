using UnityEngine;

public class Health : MonoBehaviour
{
    public int health = 872;
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
