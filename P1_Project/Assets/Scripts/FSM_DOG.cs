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
    private WanderAroundPlusAvoid  wanderAround;
    private ArrivePlusOA arrive;
    private GameObject wolf;
    private GameObject powerUp;
    private float chasingTime;
    private float eatingTime;
    private bool canScareWolf;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        blackboardDog = GetComponent<BLACKBOARD_DOG>();
        wanderAround = GetComponent<WanderAroundPlusAvoid>();
        arrive = GetComponent<ArrivePlusOA>();
       // powerUp = blackboardDog.powerUp;

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
            () => { wanderAround.enabled = true; powerUp = null;},
            () => { },
            () => { wanderAround.enabled = false; }
        );

        State ReachingPowerUp = new State("Reaching_PowerUp",
           () => { arrive.target = powerUp; arrive.enabled = true;},
           () => { },
           () => { arrive.enabled = false; }
       );

        State EatingPowerUp = new State("Eating_PowerUp",
           () => { eatingTime = 0f; },
           () => { eatingTime += Time.deltaTime; },
           () => { eatingTime = 0;
                   powerUp.SetActive(false);
                   canScareWolf = true;
           }
       );

        State ScaringWolf = new State("Scaring_Wolf",

            () =>
            {
                //wolf.tag = "WOLF SCARED"; 
                gameObject.tag = "BEAST";
                chasingTime = 0;
                arrive.target = wolf;
                arrive.enabled = true;
            },
           () => { chasingTime += Time.deltaTime; },
           () => {
             
               arrive.enabled = false;
               canScareWolf = false;
               gameObject.tag = "DOG";
           }

            );

        State GoingToPen = new State("GoingToPen",
            () => { 
                Debug.Log("al corral");
                arrive.target = blackboardDog.pen;
                arrive.enabled = true;
            },
            () => { },
            () => { arrive.enabled = false; wanderAround.enabled = true; }
        );
        

        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        */
        /*Transition PowerUpNear = new Transition("PowerUpNear",
            () =>
            {
                PowerUp = SensingUtils.FindInstanceWithinRadius(gameObject, "POWER UP", blackboardDog.powerUpDetectionRadius); 
                return PowerUp != null && (SensingUtils.DistanceToTarget(gameObject,PowerUp)< blackboardDog.powerUpDetectionRadius);
            }

   );*/
        Transition powerUpDetected = new Transition("powerUpDetected",
            () =>
            {
                powerUp = SensingUtils.FindInstanceWithinRadius(gameObject, "POWER UP", blackboardDog.powerUpDetectionRadius);
                return powerUp != null;
            },
            () => { }
            );

        Transition GoToScaringWolf = new Transition("GoToScaringWolf",
            () =>
            {
                wolf = SensingUtils.FindInstanceWithinRadius(gameObject, "WOLF HUNTING", blackboardDog.wolfDetectionRadius);
                return canScareWolf  && (SensingUtils.DistanceToTarget(gameObject, wolf) < blackboardDog.wolfDetectionRadius);
            }

            );

        Transition WolfScared = new Transition("WolfScared",
           () =>
           {

               return SensingUtils.DistanceToTarget(gameObject, wolf) > blackboardDog.wolfEscapeRadius;
           }

           );

        Transition ChasingTimeOver = new Transition("ChasingTimeOver",
           () =>
           {

               return chasingTime > blackboardDog.maxChasingTime;
           }

           );

        Transition PowerUpReached = new Transition("PowerUpReached",
            () =>
            {

                return SensingUtils.DistanceToTarget(gameObject, powerUp) < blackboardDog.powerUpReachedRadius;
            }
            );


        Transition EatingTimeOver = new Transition("EatingTimeOver",
            () =>
            {

                return eatingTime > blackboardDog.maxEatingTime;
            }

            );

        Transition GuidingSheep = new Transition("GuidingSheep",
            () =>
            {
                
                if (SensingUtils.FindInstanceWithinRadius(gameObject, "SHEEP", blackboardDog.sheepDetectionRadius))
                {
                    blackboardDog.guidingSheep = true;
                }
                Debug.Log(blackboardDog.guidingSheep);
                return blackboardDog.guidingSheep;
            }

            );
        Transition BackToWander = new Transition("BackToWander",
            () =>
            {

                if (SensingUtils.FindInstanceWithinRadius(gameObject, "SHEEP", blackboardDog.sheepDetectionRadius))
                {
                    blackboardDog.guidingSheep = true;
                }
                else
                {
                    blackboardDog.guidingSheep = false;

                }
                Debug.Log(blackboardDog.guidingSheep);
                return !blackboardDog.guidingSheep;
            }

            );
        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */
        AddStates(Wandering, ReachingPowerUp, EatingPowerUp, ScaringWolf, GoingToPen);

        AddTransition(Wandering, powerUpDetected, ReachingPowerUp);
        AddTransition(ReachingPowerUp, PowerUpReached , EatingPowerUp);
        AddTransition(EatingPowerUp, EatingTimeOver, Wandering);
        AddTransition(Wandering, GoToScaringWolf, ScaringWolf);
        AddTransition(ScaringWolf, ChasingTimeOver, Wandering);
        AddTransition(ScaringWolf, WolfScared, Wandering);

        AddTransition(Wandering, GuidingSheep, GoingToPen);
        AddTransition(GoingToPen, BackToWander, Wandering);


        /* STAGE 4: set the initial state
         
        initialState = ... 

         */
        initialState = Wandering;

    }
}
