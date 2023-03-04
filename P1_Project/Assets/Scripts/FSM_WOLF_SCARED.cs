using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_WOLF_SCARED", menuName = "Finite State Machines/FSM_WOLF_SCARED", order = 1)]
public class FSM_WOLF_SCARED : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
    private BLACKBOARD_WOLF blackboardWolf;
    private FleePlusOA flee;
    private ArrivePlusOA arrive;
    private float restingTime;
    private float hidingTime;
    private float timeWithBone;
    private GameObject dog;


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


        //FiniteStateMachine Hunting = ScriptableObject.CreateInstance<FSM_WOLF_HUNTING>();

        State InitState = new State("InitState",
           () => {
               blackboardWolf.fear = true;

           }, // write on enter logic inside {}
           () => { }, // write in state logic inside {}
           () => {   }  // write on exit logic inisde {}  
       );


        State WolfEscaping = new State("WolfEscaping",
           () => {
               flee.target = dog;
               flee.enabled = true;  }, // write on enter logic inside {}
           () => { }, // write in state logic inside {}
           () => { flee.enabled = false; }  // write on exit logic inisde {}  
       );
        State WolfHiding = new State("WolfHiding",
         () => {
             arrive.target = blackboardWolf.hidingSpot;
             arrive.enabled = true;
         }, // write on enter logic inside {}
         () => { }, // write in state logic inside {}
         () => { arrive.enabled = false; }  // write on exit logic inisde {}  
     );
        State WolfHidingFromDog = new State("WolfHidingFromDog",
         () => {
             hidingTime = 0;
              
         }, // write on enter logic inside {}
         () => { hidingTime += Time.deltaTime; }, // write in state logic inside {}
         () => {   }  // write on exit logic inisde {}  
     );
        State GoingToBone = new State("GoingToBone",
         () => {
             arrive.target = blackboardWolf.bone;
             arrive.enabled = true;
         }, // write on enter logic inside {}
         () => { }, // write in state logic inside {}
         () => { arrive.enabled = false; }  // write on exit logic inisde {}  
     );

        State PlayingWithBone = new State("PlayingWithBone",
             () => {
                 timeWithBone = 0;          
             }, // write on enter logic inside {}
             () => { timeWithBone += Time.deltaTime; }, // write in state logic inside {}
             () => {  }  // write on exit logic inisde {}  
         );

        State GoingToRest = new State("GoingToRest",
         () => {
             arrive.target = blackboardWolf.cave;
             arrive.enabled = true;
             //gameObject.tag = "WOLF RESTING";
         }, // write on enter logic inside {}
         () => { }, // write in state logic inside {}
         () => { arrive.enabled = false; }  // write on exit logic inisde {}  
     );

        State NotScaredAnymore = new State("NotScaredAnymore",
         () => {
             //gameObject.tag = "WOLF RESTING";
             blackboardWolf.fear = false;
             //gameObject.tag = "WOLF HUNTING";
         },  
         () => { /*blackboardWolf.fear = false;*/ },  
         () => {   }   
     );

        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        */
        Transition StartScaping = new Transition("StartScaping",
         () => { dog = SensingUtils.FindInstanceWithinRadius(gameObject, "BEAST", blackboardWolf.maxBeastDetectionRadius);
             return dog != null;
         }
      );

        Transition GoingToHide = new Transition("GoingToHide",
          () => { return SensingUtils.DistanceToTarget(gameObject,  dog) > blackboardWolf.escapingRadius; }   
       );

        Transition HidingSpotReached = new Transition("HidingSpotReached",
         () => { return SensingUtils.DistanceToTarget(gameObject, blackboardWolf.hidingSpot) < blackboardWolf.digestionRadius; }
      );

        Transition GoingToBones = new Transition("GoingToBones",
        () => { return hidingTime > restingTime; }
     );
        Transition BoneReached = new Transition("BoneReached",
        () => {  return SensingUtils.DistanceToTarget(gameObject, blackboardWolf.bone) < blackboardWolf.boneRadius; }
     );

        Transition GoingToCave = new Transition("GoingToCave",
        () => { return timeWithBone >= blackboardWolf.maxTimeWithBone; }
     );

        Transition CaveReached = new Transition("CaveReached",
       () => { return SensingUtils.DistanceToTarget(gameObject, blackboardWolf.cave) < blackboardWolf.restingRadius;
  
       }
    );

        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);
 
         */
        AddStates( InitState, WolfEscaping, WolfHiding, WolfHidingFromDog, GoingToBone, PlayingWithBone, GoingToRest, NotScaredAnymore);

        AddTransition(InitState, StartScaping, WolfEscaping);
        AddTransition(WolfEscaping, GoingToHide, WolfHiding);
        AddTransition(WolfHiding, HidingSpotReached, WolfHidingFromDog);
        AddTransition(WolfHidingFromDog,GoingToBones, GoingToBone);
        AddTransition(GoingToBone,BoneReached, PlayingWithBone);
        AddTransition(PlayingWithBone,GoingToCave, GoingToRest);
        AddTransition(GoingToRest,CaveReached, NotScaredAnymore);




        /* STAGE 4: set the initial state

        initialState = ... 

         */
        //initialState = Hunting
        initialState = InitState;
    }
}
