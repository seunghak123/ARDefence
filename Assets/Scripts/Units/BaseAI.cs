using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAI : MonoBehaviour
{
    [SerializeField] private CharacterController unitController;
    [SerializeField] private E_INGAME_AI_TYPE unitAIType = E_INGAME_AI_TYPE.NONE;
    [SerializeField] private E_INGAME_TEAM_TYPE unitTeamType;
    private bool isDead = false;
    private Action currentUnitEvent = null;

    private Dictionary<E_INGAME_AI_TYPE, Action> userActionDic = new Dictionary<E_INGAME_AI_TYPE, Action>();
    private void Awake()
    {
        RegistAction();
    }
    private void RegistAction()
    {
        userActionDic[E_INGAME_AI_TYPE.UNIT_IDLE] = UnitIdle;
        userActionDic[E_INGAME_AI_TYPE.UNIT_ATTACK] = UnitAttack;
        userActionDic[E_INGAME_AI_TYPE.UNIT_MOVE] = UnitMove;
        userActionDic[E_INGAME_AI_TYPE.UNIT_EVENT] = UnitEvent;
        userActionDic[E_INGAME_AI_TYPE.UNIT_STUN] = UnitStun;

    }
    protected void ChangeAI(E_INGAME_AI_TYPE nextAIType)
    {
        unitAIType = nextAIType;
    }
    protected virtual void UnitStun()
    {

    }
    protected virtual void UnitEvent()
    {
        if (currentUnitEvent != null)
        {
            currentUnitEvent();
        }
    }
    protected virtual void UnitMove()
    {

    }
    protected virtual void UnitAttack()
    {

    }
    protected virtual void UnitIdle()
    {
        
    }
    private void Action()
    {
        if (userActionDic.ContainsKey(unitAIType))
        {
            userActionDic[unitAIType]();
        }
        else
        {
            unitAIType = E_INGAME_AI_TYPE.UNIT_IDLE;
        }
    }

    public void Update()
    {
        if(!isDead)
        {
            Action();
        }
    }
}
