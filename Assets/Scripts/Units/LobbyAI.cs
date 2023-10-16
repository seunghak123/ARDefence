using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyAI : BaseAI
{
    private List<E_INGAME_AI_TYPE> playAnimAiLists = new List<E_INGAME_AI_TYPE>()
    {
        E_INGAME_AI_TYPE.UNIT_ATTACK,
        E_INGAME_AI_TYPE.UNIT_HIT
    };
    public void PlayAnim()
    {
        int random = Random.Range(0,playAnimAiLists.Count);
        ChangeAI(playAnimAiLists[random]);
    }
    public override void Update()
    {
        //Update문을 실행하지 않기 위한 override
    }
}
