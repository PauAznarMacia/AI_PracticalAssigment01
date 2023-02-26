using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_DOG", menuName = "Finite State Machines/FSM_DOG", order = 1)]
public class FSM_DOG : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
    private BLACKBOARD_DOG blackboardDog;
    private WanderAround wanderAround;
    private Arrive arrive;
    private GameObject wolf;
    private GameObject PowerUp;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */ 
        blackboardDog = GetComponent<BLACKBOARD_DOG>();
        wanderAround = GetComponent<WanderAround>();
        arrive = GetComponent<Arrive>();
       
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

        State ReachingPowerUp = new State("Reaching_PowerUp",
           () => {
               arrive.target = PowerUp;
               arrive.enabled = true; },
           () => { },
           () => { arrive.enabled = false; }
       );

        State ScaringWolf = new State("Scaring_Wolf",

            () => {
                arrive.target = wolf;
                arrive.enabled = true;
            },
           () => { },
           () => { arrive.enabled = false; }

            );




        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        */
        Transition PowerUpNear = new Transition("PowerUpNear",
            () => {
                PowerUp = SensingUtils.FindInstanceWithinRadius(gameObject, "PowerUp", blackboardDog.powerUpDetectionRadius);
                return PowerUp != null;
            }
   );

        Transition WolfNear = new Transition("WolfNear",
            () =>
            {

                return SensingUtils.DistanceToTarget(gameObject, wolf)> blackboardDog.wolfEscapeRadius;
            }

            );
        Transition WolfScared = new Transition("WolfScared",
            () =>
            {
                wolf = SensingUtils.FindInstanceWithinRadius(gameObject, "wolf", blackboardDog.wolfEscapeRadius);
                return wolf != null;
            }

            );

        Transition ChasingTimeOver = new Transition("ChasingTimeOver",
           () =>
           {
               wolf = SensingUtils.FindInstanceWithinRadius(gameObject, "wolf", blackboardDog.wolfEscapeRadius);
               return wolf != null;
           }

           );


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */
        AddStates(Wandering, ReachingPowerUp, ScaringWolf);
        AddTransition(Wandering, PowerUpNear, ReachingPowerUp);
        AddTransition(ReachingPowerUp, WolfNear, ScaringWolf);
        AddTransition(ScaringWolf, ,Wandering,);


        /* STAGE 4: set the initial state
         
        initialState = ... 

         */

    }
}
