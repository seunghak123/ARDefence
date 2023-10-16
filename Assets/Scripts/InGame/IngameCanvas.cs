using Seunghak.SceneManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameCanvas : MonoBehaviour
{
    [Header("BaseCanvasOption")]
    [SerializeField] private Button optionButton;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private Transform uiSpawnPos;
    [Space(2)]
    [Header("OptionPanel")]
    [SerializeField] private Button exitButton;

    private void OnEnable()
    {
        optionButton.onClick.AddListener(OnOffOptionPanel);
    }
    private void OnDisable()
    {
        optionButton.onClick.RemoveListener(OnOffOptionPanel);
    }
    private void OnOffOptionPanel()
    {
        optionPanel.SetActive(!optionPanel.activeSelf);
    }
    private void ExitInGame()
    {
        SceneManager.Instance.ChangeScene(E_SCENE_TYPE.LOBBY);
    }
}
