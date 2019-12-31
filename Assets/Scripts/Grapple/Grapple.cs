using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    private GrabManager grabMaster;
    private GameObject grabObject;
    [SerializeField]
    private bool isHolding = false;
    private Rigidbody2D rigid;
    private GameObject child;
    private GrappleObjectScript grappleReg;
    private ReflectorScript reflector;
    float currentTimeMulti;
    int timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        grabMaster = GameObject.FindGameObjectWithTag("GrabMaster").GetComponent<GrabManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isHolding)
        {
            if (Input.GetMouseButtonDown(1) && timer > 70)
            {
                grabMaster.getClosest(grabMaster.grabs);
                isHolding = true;

            }
            timer++;
        }

        else if(isHolding && Input.GetMouseButtonDown(1))
        {
            child = GameObject.Find("Grabbed");
            rigid = child.GetComponent<Rigidbody2D>();
            if(child.tag == "GrabAble")
            {
                grappleReg = child.GetComponent<GrappleObjectScript>();
                grappleReg.Shot(true);
                rigid.isKinematic = false;

            }
            else if (child.tag == "GrabAbleReflector")
            {
                reflector = child.GetComponent<ReflectorScript>();
                reflector.Shot(true);
                rigid.isKinematic = false;

            }

            currentTimeMulti = GameStateManager.GetTimeMulti();
            rigid.AddForce(child.transform.up * 0.3f * currentTimeMulti);
            grabMaster.removeChild();
            
            isHolding = false;
            timer = 0;
        }

        if (grabMaster.transform.childCount == 0)
        {
            isHolding = false;
        }
        //Debug.Log("ChildCount:" + grabMaster.transform.childCount);

    }

    void OnTriggerEnter2D(Collider2D coll)
    {
       
        //when they collide
        //boolean to false
        
        if(coll.CompareTag("GrabAble") || coll.CompareTag("GrabAbleReflector"))
        {
            grabObject = coll.gameObject;
            grabMaster.grabListAdd(grabObject);
        }
    }



    private void OnTriggerExit2D(Collider2D collision)
    {
        grabObject = collision.gameObject;
        grabMaster.grabListRemove(grabObject);
    }


}
