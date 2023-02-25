using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_SHEEP", menuName = "Finite State Machines/FSM_SHEEP", order = 1)]
public class FSM_SHEEP : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
    private BLACKBOARD_SHEEP blackboardSheep;
    private FlockingAround flockingAround;
    private Flee flee;
    private GameObject wolf;
    private GameObject dog;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */

        blackboardSheep = GetComponent<BLACKBOARD_SHEEP>();
        flockingAround = GetComponent<FlockingAround>();
        flee = GetComponent<Flee>();

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */

        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------
         
        State varName = new State("StateName",
            () => { }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );

        */

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

        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        */

        Transition wolfNearby = new Transition("Wolf Nearby",
            () => {
                wolf = SensingUtils.FindInstanceWithinRadius(gameObject, "WOLF", blackboardSheep.wolfDetectionRadius);
                return wolf != null;
            }
        );

        Transition wolfFarAway = new Transition("Wolf Far Away",
            () => { return SensingUtils.DistanceToTarget(gameObject, wolf) > blackboardSheep.wolfFarAwayRadius; }
        );

        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

        */

        AddStates(walkAround, runAway);
        AddTransition(walkAround, wolfNearby, runAway);
        AddTransition(runAway, wolfFarAway, walkAround);

        /* STAGE 4: set the initial state
         
        initialState = ... 

        */

        initialState = walkAround;
    }
}