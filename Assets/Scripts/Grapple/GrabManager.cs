using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabManager : MonoBehaviour
{

    public List<GameObject> grabs;
    float refDist = 0;
    float closestDist = 100;
    GameObject closest;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void grabListAdd(GameObject grabable)
    {
        grabs.Add(grabable);
    }

    public void grabListRemove(GameObject grabableLeft)
    {
        grabs.Remove(grabableLeft);
    }


    public void getClosest(List<GameObject> grabs)
    {

        foreach (GameObject grab in grabs)
        {
            Vector3 dir = transform.position - grab.transform.position;

            refDist = dir.magnitude;

            if (closestDist > refDist)
            {
                closestDist = dir.magnitude;
                closest = grab;
            }

        }

        closest.transform.position = this.transform.position;
        closest.transform.rotation = this.transform.rotation;
        closest.transform.parent = this.transform;
        grabs.Remove(closest);
        closest.name = "Grabbed";
        closestDist = 100;
        refDist = 0;
        
    }

    public void removeChild()
    {
        Transform child = transform.GetChild(0);
        child.transform.parent = null;
    }


}
