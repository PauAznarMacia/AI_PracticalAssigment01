using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_SHEEP", menuName = "Finite State Machines/FSM_SHEEP", order = 1)]
public class FSM_SHEEP : FiniteStateMachine
{
    private BLACKBOARD_SHEEP blackboardSheep;
    private FlockingAroundPlusAvoidance flockingAround;
    private FleePlusOA flee;
    private GameObject wolf;
    private GameObject dog;
    private GameObject pen;
    // private SteeringContext steeringContext;
    // private float normalSpeed;

    public override void OnEnter()
    {
        blackboardSheep = GetComponent<BLACKBOARD_SHEEP>();
        flockingAround = GetComponent<FlockingAroundPlusAvoidance>();
        flee = GetComponent<FleePlusOA>();
        pen = GameObject.Find("PEN");
       // steeringContext = GetComponent<SteeringContext>();
       // normalSpeed = steeringContext.maxSpeed;

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        // STAGE 1: create the states with their logic(s)

        State LookingForDog = new State("LookingForDog",
            () => { },
            () => { },
            () => { }
        );

        State walkAround = new State("WalkAround",
            () => {
              
                flockingAround.attractor = dog;

                flockingAround.enabled = true;
            },
            () => { },
            () => { flockingAround.enabled = false; }
        );

        State runAway = new State("RunAway",
            () => {
                flee.target = wolf;
                flee.enabled = true;
            },
            () => { },
            () => { flee.enabled = false; }
        );

        State ArrivedToPen = new State("ArrivedToPen",
            () => {
                Debug.Log("tic a casa");
                //gameObject.SetActive(false);
                Destroy(gameObject);
            },
            () => { },
            () => {  }
        );
        
        /*State caught = new State("Caught",
            () => { flockingAround.enabled=false;
            flee.enabled = false;},
            () => { ;},
            () => { ;}
        ); 
*/
        // STAGE 2: create the transitions with their logic(s)


        Transition dogNearby = new Transition("Dog Nearby",
            () => {
                dog = SensingUtils.FindInstanceWithinRadius(gameObject, "DOG", blackboardSheep.dogDetectionRadius);
                return dog != null;
            }
        );


        Transition wolfNearby = new Transition("Wolf Nearby",
            () => {
                wolf = SensingUtils.FindInstanceWithinRadius(gameObject, "WOLF HUNTING", blackboardSheep.wolfDetectionRadius);
                return wolf != null;
            }
        );


        Transition wolfFarAway = new Transition("Wolf Far Away",
            () => {                                       
                 
                return SensingUtils.DistanceToTarget(gameObject, wolf) > blackboardSheep.wolfFarAwayRadius;
            }
        );

        Transition ArrivingToPen = new Transition("ArrivingToPen",
            () =>
            {
                return SensingUtils.DistanceToTarget(gameObject, pen) < blackboardSheep.penDistanceToSafety;
            }

            );

        /* Transition sheepCaught = new Transition("Sheep Caught",
              () => {                                       

                  return SensingUtils.DistanceToTarget(gameObject, wolf) > blackboardWolf.caughtRadius;
              }
          ); 
          */

        // STAGE 3: add states and transitions to the FSM 

        AddStates(LookingForDog,walkAround, runAway, ArrivedToPen);

        AddTransition(LookingForDog, wolfNearby, runAway);
        AddTransition(LookingForDog, dogNearby, walkAround);
        AddTransition(walkAround, wolfNearby, runAway);
        AddTransition(runAway, wolfFarAway, walkAround);

        AddTransition(walkAround, ArrivingToPen, ArrivedToPen);

        //AddTransition(runAway, sheepCaught, caught);

        // STAGE 4: set the initial state

        initialState = LookingForDog;
    }
}