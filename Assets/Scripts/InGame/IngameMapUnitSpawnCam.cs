using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameMapUnitSpawnCam : MonoBehaviour
{
    [SerializeField] private Camera ingameCam;
    private bool isSpawnedTime = false;
    private List<UnitController> spawnedUnitLists;
    protected void Update()
    {
        if (isSpawnedTime && Input.touches.Length>0)
        {
            Ray targetRay = ingameCam.ScreenPointToRay(Input.touches[0].position);

            RaycastHit rayResult; 
            
            Physics.Raycast(targetRay,out rayResult);

            if (rayResult.collider != null)
            {
                //rayResult의 태그가 생성 plane일 경우 해당 히트 포지션을 저장,
                //현재 선택된 유닛의 id를 저장한다
            }
        }
    }
    //스폰 시간일때, 스폰 Cam에서 팀에 따라 Hit하면 유닛 생성 위치 지정

}
