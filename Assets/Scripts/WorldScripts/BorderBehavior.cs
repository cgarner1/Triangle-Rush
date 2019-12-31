using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderBehavior: MonoBehaviour
{
    private float activeDistance = 10;
    [SerializeField] GameObject player;
    SpriteRenderer sprite;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color(1f, 1f, 1f,0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        float playerPos;
        float borderPos;
        if (this.gameObject.tag == "HorizWall")
        {
            playerPos = player.transform.localPosition.y;
            borderPos = transform.localPosition.y;
        } else
        {
            playerPos = player.transform.localPosition.x;
            borderPos = transform.localPosition.x;
            
        }

        float currentDistance = Mathf.Abs(playerPos - borderPos);
        
        if (currentDistance<=activeDistance)
        {
            float colorOffset = currentDistance / activeDistance;
            sprite.color = new Color(1f,1f,1f, 1f -colorOffset);
        }
        
    }
}
