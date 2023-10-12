using Seunghak.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IngameManager : MonoBehaviour
{
    public static IngameManager currentManager = null;
    [SerializeField] private NavMeshSurface navMeshSurface;

    [Header("SceneObject")]
    [SerializeField] private Transform mapSpawnPos;
    [SerializeField] private Transform team1Parent;
    [SerializeField] private Transform team2Parent;
    //[SerializeField] private IngameUIManager ingameUI;

    private List<UnitController> enemyUnits = new List<UnitController>();
    private List<UnitController> teamUnits = new List<UnitController>();
    private void Awake()
    {
        currentManager = this;
    }
    public void CreateGame(int stageId)
    {

        //테스트 코드 작성
        JStageData stageData = JsonDataManager.Instance.GetStageData(0);

        Material targetMat = GameResourceManager.Instance.LoadObject(stageData.skyboxMat) as Material;
        GameObject spawnedMap = GameResourceManager.Instance.SpawnObject(stageData.mapPrefab);
        spawnedMap.transform.parent = mapSpawnPos;
        spawnedMap.transform.localPosition = Vector3.zero;

        //enemyDataJson 읽어서 적 데이터 가져올 것
        RenderSettings.skybox = targetMat;

        //MakeMapMesh();
        //ingameUI.InitGame(데이터)
        //스테이지 데이터를 받아서 게임 모듈 생성

    }


    private void MakeMapMesh()
    {
        ModifyNavMesh();
    }
    public void ModifyNavMesh()
    {
        navMeshSurface.RemoveData();
        navMeshSurface.BuildNavMesh();
    }
}
