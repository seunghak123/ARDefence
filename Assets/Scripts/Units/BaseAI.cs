using Seunghak.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAI : MonoBehaviour
{
    [Header("UnitInfo")]
    [SerializeField] private E_INGAME_AI_TYPE unitAIType = E_INGAME_AI_TYPE.NONE;

    [Space(2)]
    [Header("UnitAnimation")]
    [SerializeField] private Animator unitAnim;
    [SerializeField] private UnitStructure unitInfo;
    private Dictionary<E_INGAME_AI_TYPE, Action> userActionDic = new Dictionary<E_INGAME_AI_TYPE, Action>();
    
    private bool isDead = false;
    private Action currentUnitEvent = null;

    public void Awake()
    {
        RegistAction();
    }
    private void RegistAction()
    {
        userActionDic[E_INGAME_AI_TYPE.UNIT_IDLE] = UnitIdle;
        userActionDic[E_INGAME_AI_TYPE.UNIT_ATTACK] = UnitAttack;
        userActionDic[E_INGAME_AI_TYPE.UNIT_MOVE] = UnitMove;
        userActionDic[E_INGAME_AI_TYPE.UNIT_EVENT] = UnitEvent;
        userActionDic[E_INGAME_AI_TYPE.UNIT_HIT] = UnitStun;

    }
    protected void ChangeAI(E_INGAME_AI_TYPE nextAIType)
    {
        if (!unitAIType.Equals(nextAIType))
        {
            unitAIType = nextAIType;

            if (unitAnim != null)
            {
                switch (unitAIType)
                {
                    case E_INGAME_AI_TYPE.UNIT_ATTACK:
                        break;
                    case E_INGAME_AI_TYPE.UNIT_HIT:
                        break;
                    case E_INGAME_AI_TYPE.UNIT_DEAD:
                        unitAnim.SetTrigger("IsDead");
                        break;
                }
            }
        }
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
        //목표 위치로 이동
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
