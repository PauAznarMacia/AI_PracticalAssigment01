using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_WOLF_HUNTING", menuName = "Finite State Machines/FSM_WOLF_HUNTING", order = 1)]
public class FSM_WOLF_HUNTING : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/

    private BLACKBOARD_WOLF blackboardWolf;
    private ArrivePlusOA arrive; 
    private FleePlusOA flee;
    private float restingTime;
    private float hidingTime;
    private float eatingTime;
    private bool full;
    private GameObject sheep;
    private GameObject cave;
    private GameObject hidingSpot;
    private GameObject peril;
    private Color normalColor;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        blackboardWolf = GetComponent<BLACKBOARD_WOLF>();
        arrive = GetComponent<ArrivePlusOA>(); 
        flee = GetComponent<FleePlusOA>();
        cave = blackboardWolf.cave;
        hidingSpot = blackboardWolf.hidingSpot;
        normalColor = GetComponent<SpriteRenderer>().color;

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */
        GetComponent<SpriteRenderer>().color = normalColor;
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

        State Resting = new State("Resting",
 
            () => { full = false; sheep = null; },  
            () => { restingTime += Time.deltaTime; },  
            () => { restingTime = 0;  }

            );

        State Hunting = new State("Hunting",

           () => {
               arrive.target = sheep;
               arrive.enabled = true; 
               gameObject.tag = "WOLF HUNTING";},
           () => { },
           () => {sheep.transform.parent = transform; arrive.enabled = false; }

           );

        State Eating = new State("Eating",

          () => {  eatingTime = 0f; },
          () => {
              
              eatingTime += Time.deltaTime;
              if (eatingTime >= blackboardWolf.maxEatingTime) full = true;
              
          },
          () => { /*Destroy(sheep)*/ sheep.transform.parent = gameObject.transform;

          }  

          );

        State GoingToHidingSpot = new State("GoingToHidingSpot",

          () => {
              
              arrive.target = hidingSpot;
              arrive.enabled = true;
          },
          () => { },
          () => { arrive.enabled = false; }

          );


        State HidingInSpot = new State("HidingInSpot",

            () => { hidingTime = 0; },
            () => { hidingTime += Time.deltaTime; },
            () => { Destroy(sheep); }

            );

        State GoingToCave = new State("GoingToCave",

         () => {
             arrive.target = cave;
             arrive.enabled = true;
         },
         () => { },
         () => { arrive.enabled = false; }

         );

        State Scaping = new State("Scaping",
         () => { flee.target = peril; 
         flee.enabled = true;
         GetComponent<SpriteRenderer>().color = new Color(3f/256, 120f/256, 7f/256);
         },
         () => { ; },
         () => {flee.enabled = false; 
         GetComponent<SpriteRenderer>().color = normalColor;}

        );



        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        */
        Transition StartHunting = new Transition("Start Hunting",
           () => {
               sheep = SensingUtils.FindInstanceWithinRadius(gameObject, "SHEEP", blackboardWolf.sheepDetectingRadius);
               return sheep != null && full == false;
           }
       );

        Transition StartEating = new Transition("Start Eating",
           () => {
             return SensingUtils.DistanceToTarget(gameObject, sheep) < blackboardWolf.eatingRadius;
           }
       );

        Transition GoToDigestion = new Transition("GoToDigestion",
           () => {
             
               return full && (eatingTime > blackboardWolf.maxEatingTime);
           }
       );

        Transition StartDigestion = new Transition("StartDigestion",
           () => {
               return SensingUtils.DistanceToTarget(gameObject, hidingSpot) < blackboardWolf.digestionRadius;
           }
       );

        Transition GoToRest = new Transition("GoToRest",
          () => {
              return hidingTime > blackboardWolf.maxHidingTime;
          }
      );

        Transition StartResting = new Transition("StartResting",
         () => {
             return SensingUtils.DistanceToTarget(gameObject, cave) < blackboardWolf.restingRadius;
         }
     );

     Transition ScapingDog = new Transition("ScapingDog",
         () => {
             peril = SensingUtils.FindInstanceWithinRadius(gameObject, "BEAST", blackboardWolf.escapingRadius);
             return SensingUtils.DistanceToTarget(gameObject, peril) < blackboardWolf.escapingRadius;
         }
     );

        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */
        AddStates(Resting, Hunting, Eating, GoingToHidingSpot, HidingInSpot, GoingToCave, Scaping);
        AddTransition(Resting, StartHunting, Hunting);
        AddTransition(Hunting, StartEating, Eating);
        AddTransition(Eating, GoToDigestion, GoingToHidingSpot);
        AddTransition(GoingToHidingSpot, StartDigestion, HidingInSpot);
        AddTransition(HidingInSpot, GoToRest, GoingToCave);
        AddTransition(GoingToCave, StartResting , Resting);
        AddTransition(Hunting, ScapingDog, Scaping);
        AddTransition(Scaping, ScapingDog, GoingToCave);

        /* STAGE 4: set the initial state
         
        initialState = ... 

         */
        initialState = Resting;

    }
}
