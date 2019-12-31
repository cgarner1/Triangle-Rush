using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    [SerializeField] private GameObject player;
    float playerX, playerY, playerZ;
    // Update is called once per frame
    void Update()
    {
        playerX = player.transform.eulerAngles.x;
        playerY = player.transform.eulerAngles.y;
        playerZ = player.transform.eulerAngles.z;

        transform.eulerAngles = new Vector3(playerX, playerY - playerY, playerZ - playerZ);

    }
}
