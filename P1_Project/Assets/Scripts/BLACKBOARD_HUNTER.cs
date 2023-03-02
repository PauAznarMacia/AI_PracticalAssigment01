using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BLACKBOARD_HUNTER : MonoBehaviour
{

    public float wolfDetectionRadius = 3f;
    public float wolfFarAwayRadius = 8f;
    public GameObject wolf;

    void Start()
    {
        wolf = GameObject.Find("WOLF");
    }

    void Update()
    {
        
    }
}