using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;


public enum UIState
{
    None,
    Craft,
    System,
    Score
}

public class UIManager : MonoBehaviour, IDeathListener, IUIStateListener
{
    [SerializeField] private GameObject systemEnvironment;
    [SerializeField] private GameObject gameOverBoard;

    [SerializeField] private TextMeshProUGUI curPlayerText;
    [SerializeField] private TextMeshProUGUI totalPlayerText;

    Coroutine ActivateInput;

    public delegate void OnUIStateChangeEvent(UIState state);
    public event OnUIStateChangeEvent OnUIStateChanged;


    public void OnStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Lobby :
                ActivateInput = StartCoroutine(ActivateInGameInput());
                break;
            case GameState.InGame :
                break;
            case GameState.GameOver:
                StopCoroutine(ActivateInput);
                break;
        }
    }
    public void OnDeath()
    {
        ActivateScoreBoard();
    }

    void Awake()
    {
        systemEnvironment.SetActive(false);
        gameOverBoard.SetActive(false);
    }

    void Init()
    {

    }

    IEnumerator ActivateInGameInput()
    {
        while(true)
        {
            ControlSystemEnvironment();
            yield return null;
        }
    }
    void ControlSystemEnvironment()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CursorX.Free(!systemEnvironment.activeInHierarchy);
            systemEnvironment.SetActive(!systemEnvironment.activeInHierarchy);
        }
    }

    void ActivateScoreBoard()
    {
        gameOverBoard.SetActive(true);
        CursorX.Free(true);
    }




}
