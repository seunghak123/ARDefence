using Seunghak.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAI : MonoBehaviour
{
    [Header("UnitInfo")]
    [SerializeField] private E_INGAME_AI_TYPE unitAIType = E_INGAME_AI_TYPE.NONE;
    [SerializeField] private E_INGAME_TEAM_TYPE unitTeamType;

    [Space(2)]
    [Header("UnitAnimation")]
    [SerializeField] private Animator unitAnim;
    [SerializeField] private UnitStructure unitInfo;

    [Space(2)]
    [Header("UnitFuction")]
    [SerializeField] private CharacterController characterController;
    private Dictionary<E_INGAME_AI_TYPE, Action> userActionDic = new Dictionary<E_INGAME_AI_TYPE, Action>();
    
    private bool isDead = false;
    private JUnitData aiUnitData = null;
    private Action currentUnitEvent = null;

    private BaseAI targetAI = null;
    public void Awake()
    {
        characterController = GetComponent<CharacterController>();
        RegistAction();
    }
    private void RegistAction()
    {
        userActionDic[E_INGAME_AI_TYPE.UNIT_IDLE] = UnitIdle;
        userActionDic[E_INGAME_AI_TYPE.UNIT_ATTACK] = UnitAttack;
        userActionDic[E_INGAME_AI_TYPE.UNIT_MOVE] = UnitMove;
        userActionDic[E_INGAME_AI_TYPE.UNIT_EVENT] = UnitEvent;
        userActionDic[E_INGAME_AI_TYPE.UNIT_HIT] = UnitHit;
        userActionDic[E_INGAME_AI_TYPE.UNIT_DEAD] = UnitDead;

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
                        unitAnim.SetTrigger("Attack");
                        break;
                    case E_INGAME_AI_TYPE.UNIT_HIT:
                        unitAnim.SetTrigger("Hit");
                        break;
                    case E_INGAME_AI_TYPE.UNIT_DEAD:
                        unitAnim.SetTrigger("IsDead");
                        break;
                }
            }
        }
    }
    #region UnitFunction
    private BaseAI FindTargetObject()
    {
        targetAI = null;
        //팀타입이 아군일떄
        List<BaseAI> targetLists = IngameManager.currentManager.GetEnemyUnits();

        if (targetLists.Count <= 0)
        {
            return null;
        }
        BaseAI closeAI = targetLists[0];

        for(int i=1;i< targetLists.Count; i++)
        {
            //거리가 가깝고, 죽지 않았다면 해당 타겟으로 가까운 AI지정
            if(Vector3.Distance( closeAI.transform.position,this.transform.position)>
                Vector3.Distance(targetLists[i].transform.position,this.transform.position)
                && !targetLists[i].isDead)
            {
                closeAI = targetLists[i];
            }
        }
        //IngameManager
        // 인근 UnitController 를 찾아서 TeamType이 자신의 Team이 아닌 것을 찾아야한다.

        if (closeAI.isDead)
        {
            return null;
        }
        targetAI = closeAI;
        return closeAI;
    }
    #endregion UnitFunction

    #region UnitState
    protected virtual void UnitHit()
    {
        //데미지 입었을떄 해당, 만약 현재 HP가 0 이하라면 사망처리
    }
    protected virtual void UnitDead()
    {
        //사망했을 시 쓰러져 있고, 애니메이션 종료시 이벤트 읽어서 사망처리
        //물체는 살려둔다
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
        //타겟의 거리가 공격 거리까지 들어올떄까지 해당 방향으로 이동
    }
    protected virtual void UnitAttack()
    {
        //일반적인 공격력 계산 공식으로 세팅
        //BaseAI의 경우는 투사체등이 아닌 직접 공격에 해당(무조껀 히트)
    }
    protected virtual void UnitIdle()
    {
        if(isDead)
        {
            //죽어있는 상태면 아무것도 하지 않는다
            return;
        }
        if (FindTargetObject() != null)
        {
            ChangeAI(E_INGAME_AI_TYPE.UNIT_MOVE);
        }
        //타겟이 있으면 움직임
        //타겟 없으면 타겟서치

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
    #endregion UnitState
    public virtual void Update()
    {
        if(!isDead)
        {
            Action();
        }
    }
}
