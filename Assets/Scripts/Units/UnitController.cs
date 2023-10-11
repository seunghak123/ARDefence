﻿using Seunghak.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField] private BaseAI unitAI;
    //
    //[SerializeField] private BaseUnitUI unitUI;
    [SerializeField] private E_INGAME_TEAM_TYPE unitTeamType;
    //생성시 유닛 고유 식별 ID
    private long unitIntanceId;
    //AI Type받고 해당 AI 생성
    public bool SetUnitInfo(int unitId)
    {
        JUnitData unitData = JsonDataManager.Instance.GetUnitData(unitId);

        UnitStructure newUnitStructure = new UnitStructure(unitData);

        //유닛데이터 지정하고


        //AI 
        return true;
    }
    private void AttachAIComponent(int AIType)
    {
        
        //AI 타입에따라 AI 스크립트 붙여주고

        //this.gameObject.AddComponent<>
    }
}
#region UnitStruct
public class UnitStructure
{
    //기본 유닛 정보 값
    public Dictionary<E_STATVALUE_TYPE, float> baseUnitInfo = new Dictionary<E_STATVALUE_TYPE, float>();

    //계산된 유닛 정보 값(버프등)
    public Dictionary<E_STATVALUE_TYPE, float> totalUnitInfo = new Dictionary<E_STATVALUE_TYPE, float>();

    //추가된 유닛 정보 값
    public Dictionary<E_STATVALUE_TYPE, AddedReasonInfo> addedUnitInfo = new Dictionary<E_STATVALUE_TYPE, AddedReasonInfo>();
    public float GetStatValue(E_STATVALUE_TYPE getUnitInfoType)
    {
        CalcStatValue();
        if (totalUnitInfo.ContainsKey(getUnitInfoType))
        {
            return totalUnitInfo[getUnitInfoType];
        }

        return 0.0f;
    }
    public void CalcStatValue()
    {
        //계산 공식은 어찌할 것인가? 모든 addPercent합으로 할 것인가, 아니면 
        //percent는 따로따로 해서 할것인가
        foreach (var infovalue in baseUnitInfo)
        {
            switch (infovalue.Key)
            {
                //추후 Percent로 올리는 부분이 있으면 올려준다
                case E_STATVALUE_TYPE.HP_VALUE:

                    totalUnitInfo[infovalue.Key] = infovalue.Value;
                    totalUnitInfo[E_STATVALUE_TYPE.MAX_HP_VALUE] = infovalue.Value;
                    break;
                default:
                    totalUnitInfo[infovalue.Key] = infovalue.Value;
                    break;
            }
        }
        foreach (var addedValue in addedUnitInfo)
        {
            switch (addedValue.Key)
            {
                //추후 Percent로 올리는 부분이 있으면 올려준다
                //case E_STATVALUE_TYPE.ATTACK_VALUE
                default:
                    if (totalUnitInfo.ContainsKey(addedValue.Key))
                    {
                        totalUnitInfo[addedValue.Key] += addedValue.Value.reasonValue;
                    }
                    else
                    {
                        totalUnitInfo[addedValue.Key] = addedValue.Value.reasonValue;
                    }
                    break;
            }
        }
    }

    public UnitStructure(JUnitData unitData)
    {

    }
}
//시간제의 경우 IngameTimeManager에서 제거 해준다
public struct AddedReasonInfo
{
    public Texture2D reasonTexture;
    public string reasonNameString;
    public string reasonValueString;
    public float reasonValue;
}

#endregion