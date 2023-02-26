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

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        blackboardWolf = GetComponent<BLACKBOARD_WOLF>();
        arrive = GetComponent<ArrivePlusOA>();
        flee = GetComponent<FleePlusOA>();


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

        State Resting = new State("Resting",
 
            () => { },  
            () => { restingTime += Time.deltaTime; },  
            () => { restingTime = 0; }

            );

        State Hunting = new State("Hunting",

           () => {
               arrive.target = sheep;
               arrive.enabled = true; },
           () => { },
           () => { arrive.enabled = false; }

           );

        State Eating = new State("Eating",

          () => { },
          () => { eatingTime += Time.deltaTime; },
          () => { eatingTime = 0f; }

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

            () => { },
            () => { hidingTime += Time.deltaTime; },
            () => { hidingTime = 0; }

            );

        State GoingToCave = new State("GoingToCave",

         () => {
             arrive.target = cave;
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
        Transition StartHunting = new Transition("Start Hunting",
           () => {
               sheep = SensingUtils.FindInstanceWithinRadius(gameObject, "SHEEP", blackboardWolf.sheepDetectingRadius);
               return sheep != null;
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
              return hidingTime> blackboardWolf.maxHidingTime;
          }
      );


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */
        AddStates(Resting, Hunting, Eating, GoingToHidingSpot, HidingInSpot, GoingToCave);
        AddTransition(Resting, StartHunting, Hunting);
        AddTransition(Hunting, StartEating, Eating);
        AddTransition(Eating, GoToDigestion, GoingToHidingSpot);
        AddTransition(GoingToHidingSpot, StartDigestion, HidingInSpot);
        AddTransition(HidingInSpot, GoToRest, Resting);

        /* STAGE 4: set the initial state
         
        initialState = ... 

         */
        initialState = Resting;

    }
}
