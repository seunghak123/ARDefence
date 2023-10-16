using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCamController : MonoBehaviour
{
    private Camera connectCam;
    // Start is called before the first frame update
    void Start()
    {
        connectCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if(Input.GetMouseButtonDown(0))
        {
            Ray targetRay = connectCam.ScreenPointToRay(Input.mousePosition);

            RaycastHit rayResult;

            Physics.Raycast(targetRay, out rayResult);

            if (rayResult.collider != null && rayResult.collider.tag == "Character")
            {
                LobbyAI lobbyai = rayResult.collider.GetComponent<UnitController>().UNIT_AI as LobbyAI;
                if (lobbyai != null)
                {
                    lobbyai.PlayAnim();
                }
            }
        }
#endif 
        if (Input.touches.Length > 0)
        {
            Ray targetRay = connectCam.ScreenPointToRay(Input.touches[0].position);

            RaycastHit rayResult;

            Physics.Raycast(targetRay, out rayResult);

            if (rayResult.collider != null)
            {
                //rayResult의 태그가 생성 plane일 경우 해당 히트 포지션을 저장,
                //현재 선택된 유닛의 id를 저장한다
            }
        }
    }
}
