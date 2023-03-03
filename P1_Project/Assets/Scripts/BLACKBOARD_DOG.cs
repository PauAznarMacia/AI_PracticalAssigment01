using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BLACKBOARD_DOG : MonoBehaviour
{
    // Start is called before the first frame update
    public float powerUpDetectionRadius = 30f ;
    public float wolfDetectionRadius = 15f;
    public float wolfEscapeRadius = 18f;
    public float maxChasingTime = 5f;
    public float maxEatingTime = 3f;
    public float powerUpReachedRadius = 0.5f;
    public GameObject powerUp;
    public GameObject pen;
    public float sheepDetectionRadius = 2f;
    public bool guidingSheep;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
