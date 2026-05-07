using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    Player player;
    InputSubscriptionCamera GetCameraInput;
    [SerializeField] float multiplier;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        GetCameraInput = player.GetCameraInput;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float x = GetCameraInput.MoveInput.x * multiplier;
        float y = GetCameraInput.MoveInput.y * multiplier;
        transform.position = new Vector3 (player.transform.position.x + x, player.transform.position.y + 28, player.transform.position.z - 50 + y);
    }
}
