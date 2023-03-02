using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BLACKBOARD_WOLF : MonoBehaviour
{
    // Start is called before the first frame update

    public float maxRestingTime = 8f;
    public float maxBeastDetectionRadius = 5f;
    public float maxHidingTime = 3f;
    public float sheepDetectingRadius = 10f;
    public float maxEatingTime =  3f;
    public float eatingRadius = 2f;
    public float caughtRadius = 0.2f;
    public float digestionRadius = 3f;
    public float restingRadius = 2f;
    public float escapingRadius = 5f;
    public float boneRadius = 2f;
    public float maxTimeWithBone = 3f;
    public GameObject hidingSpot;
    public GameObject cave;
    public GameObject bone;

    public bool fear = false;
     

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (gameObject.tag == "WOLF SCARED") fear = true;

    }
}
