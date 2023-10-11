using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IngameManager : MonoBehaviour
{
    [SerializeField] private NavMeshSurface navMeshSurface;

    [Header("SceneObject")]
    [SerializeField] private Transform mapSpawnPos;
    [SerializeField] private Transform team1Parent;
    [SerializeField] private Transform team2Parent;
    //[SerializeField] private IngameUIManager ingameUI;

    private List<UnitController> enemyUnits = new List<UnitController>();
    private List<UnitController> teamUnits = new List<UnitController>();

    public void CreateGame()
    {
        //ingameUI.InitGame(데이터)
        //스테이지 데이터를 받아서 게임 모듈 생성

    }
    

    private void MakeMapMesh()
    {
        //맵 관련 데이터 읽어서 관련 매터리얼로 변경
        //스카이박스 매터리얼 변경
        
        //RenderSettings.skybox = newSkyboxMaterial;
        navMeshSurface.BuildNavMesh();
        ModifyNavMesh();
    }
    public void ModifyNavMesh()
    {
        navMeshSurface.RemoveData();
        navMeshSurface.BuildNavMesh();
    }
}
