using FSMs;
using UnityEngine;
using Steerings;


[CreateAssetMenu(fileName = "FSM_WOLF_LIFE", menuName = "Finite State Machines/FSM_WOLF_LIFE", order = 1)]
public class FSM_WOLF_LIFE : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/

    private BLACKBOARD_WOLF blackboard;
    private GameObject beast;
    public override void OnEnter()
    { 
        blackboard = GetComponent<BLACKBOARD_WOLF>();

        base.OnEnter();  
    }

    public override void OnExit()
    { 
        base.OnExit();
    }

    public override void OnConstruction()
    {
        FiniteStateMachine HUNTING = ScriptableObject.CreateInstance<FSM_WOLF_HUNTING>();
        FiniteStateMachine SCARED = ScriptableObject.CreateInstance<FSM_WOLF_SCARED>();
        HUNTING.Name = "HUNTING";
        SCARED.Name = "SCARED";

     

      Transition NotScared = new Transition("NotScared",
              () => { return blackboard.fear == false;
                

              }
          );  

         Transition Scared = new Transition("Scared",
            () =>
            {
               beast = SensingUtils.FindInstanceWithinRadius(gameObject, "BEAST", blackboard.maxBeastDetectionRadius);
               return beast != null;
            }
        ); 

        AddStates(HUNTING, SCARED);

         AddTransition(HUNTING, Scared, SCARED);
         AddTransition(SCARED, NotScared, HUNTING);



        initialState = HUNTING;




    }
}
