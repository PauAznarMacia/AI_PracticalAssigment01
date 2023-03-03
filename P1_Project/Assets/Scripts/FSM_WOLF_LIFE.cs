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
    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        blackboard = GetComponent<BLACKBOARD_WOLF>();

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */
        base.OnExit();
    }

    public override void OnConstruction()
    {
        FiniteStateMachine HUNTING = ScriptableObject.CreateInstance<FSM_WOLF_HUNTING>();
        FiniteStateMachine SCARED = ScriptableObject.CreateInstance<FSM_WOLF_SCARED>();
        HUNTING.Name = "HUNTING";
        SCARED.Name = "SCARED";

       /*Deberia haber una manera de controlar el cambio entre estados gracias a una variable que lo manje
         He pensado que el perro al asustar al lobo tigeree aqui el cambio de maquina, pero por algun motivo  
         se envia un estado null. Si se acaba este script el lobo deberia funcionar correctamente.
         */

     Transition NotScared = new Transition("NotScared",
              () => { return blackboard.fear == false;
                 // return SensingUtils.DistanceToTarget(gameObject, blackboard.dog) < blackboard.escapingRadius;
              }
          ); 

         Transition Scared = new Transition("Scared",
            () =>
            { 
                return blackboard.fear;
            }
        ); 

        AddStates(HUNTING, SCARED);

         AddTransition(HUNTING, Scared, SCARED);
         AddTransition(SCARED, NotScared, HUNTING);



        initialState = HUNTING;




    }
}
