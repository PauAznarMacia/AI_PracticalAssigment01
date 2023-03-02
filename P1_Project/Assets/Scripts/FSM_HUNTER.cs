using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_HUNTER", menuName = "Finite State Machines/FSM_HUNTER", order = 1)]
public class FSM_HUNTER : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
    private BLACKBOARD_HUNTER blackboardHunter;
    private WanderAroundPlusAvoid wanderAround;
    private ArrivePlusOA arrive;
    private GameObject wolf;
    private GameObject dog;
    private float chasingTime;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        blackboardHunter = GetComponent<BLACKBOARD_HUNTER>();
        wanderAround = GetComponent<WanderAroundPlusAvoid>();
        arrive = GetComponent<ArrivePlusOA>();
        wolf = blackboardHunter.wolf;
        arrive.target = wolf;
        wanderAround.attractor = GameObject.Find("Attractor");
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
        State Wandering = new State("Wandering",
            () => { wanderAround.enabled = true; },
            () => { },
            () => { wanderAround.enabled = false; }
        );
        State ReachingWolf = new State("Reaching_Wolf",
           () =>
           {
               arrive.target = wolf;
               arrive.enabled = true;

           },
           () => { },
           () => { arrive.enabled = false; }
        );
        State ScaringWolf = new State("Scaring_Wolf",

            () =>
            {
                Debug.Log(wolf.tag);
                wolf.tag = "WOLF SCARED ";
                wolf.GetComponent<FleePlusOA>().enabled = true;
                wolf.GetComponent<FleePlusOA>().target = gameObject;
                chasingTime = 0;
                arrive.target = wolf;
                arrive.enabled = true;
            },
           () => { chasingTime += Time.deltaTime; },
           () => {

               arrive.enabled = false;
              // canScareWolf = false;
           }

            );
        State WolfScared = new State("Wolf_Scared",

            () =>
            {

                wolf.tag = "WOLF RESTING";
                wolf.GetComponent<FleePlusOA>().enabled = false;
                wolf.GetComponent<FleePlusOA>().target = dog;
                wanderAround.enabled = true;

                chasingTime = 0;
               // arrive.target = null;
                arrive.enabled = false;
            },
           () => { chasingTime += Time.deltaTime; },
           () => {

               arrive.enabled = false;
               // canScareWolf = false;
           }

            );

        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        */
        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */
        Transition WolfNear = new Transition("WolfNear",
            () =>
            {
             //   wolf = SensingUtils.FindInstanceWithinRadius(gameObject, "WOLF RESTING", 200);
                dog = SensingUtils.FindInstanceWithinRadius(gameObject, "DOG", 200);

                return (SensingUtils.DistanceToTarget(gameObject, wolf) < blackboardHunter.wolfDetectionRadius);
            }
        );
        Transition WolfFarAway = new Transition("WolfFarAway",
            () =>
            {
            //    wolf = SensingUtils.FindInstanceWithinRadius(gameObject, "WOLF SCARED ", blackboardHunter.wolfFarAwayRadius);
                dog = SensingUtils.FindInstanceWithinRadius(gameObject, "DOG", 200);
                return (SensingUtils.DistanceToTarget(gameObject, wolf) > blackboardHunter.wolfFarAwayRadius);
            }
        );
        Transition WolfRan = new Transition("WolfRan",
            () =>
            {
                //    wolf = SensingUtils.FindInstanceWithinRadius(gameObject, "WOLF SCARED ", blackboardHunter.wolfFarAwayRadius);
                dog = SensingUtils.FindInstanceWithinRadius(gameObject, "DOG", 200);
                return (SensingUtils.DistanceToTarget(gameObject, wolf) > blackboardHunter.wolfFarAwayRadius);
            }
        );

        /* STAGE 4: set the initial state
         
        initialState = ... 

         */
        AddStates(Wandering, ScaringWolf, ReachingWolf, WolfScared);
        AddTransition(Wandering, WolfNear, ScaringWolf);
        AddTransition(ScaringWolf, WolfFarAway, WolfScared);
        AddTransition(WolfScared, WolfRan, Wandering);


        initialState = Wandering;

    }
}
