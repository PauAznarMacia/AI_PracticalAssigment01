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

    public override void OnEnter()
    {
        blackboardSheep = GetComponent<BLACKBOARD_SHEEP>();
        flockingAround = GetComponent<FlockingAroundPlusAvoidance>();
        flee = GetComponent<FleePlusOA>();

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
                Debug.Log("holi2");
              
                flockingAround.attractor = dog;

                Debug.Log(dog != null);

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

        // STAGE 3: add states and transitions to the FSM 

        AddStates(LookingForDog,walkAround, runAway);
        AddTransition(LookingForDog, dogNearby, walkAround);
        AddTransition(walkAround, wolfNearby, runAway);
        AddTransition(runAway, wolfFarAway, walkAround);

        // STAGE 4: set the initial state

        initialState = LookingForDog;
    }
}