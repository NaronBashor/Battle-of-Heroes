using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState
    {
        None,
        MainMenu,
        Shop,
        Heroes,
        Arena,
        Game,
        Map
    }

    public GameState state;
    private OptionsManager optionsManager;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        SetGameState(GameState.MainMenu);
    }

    private void Start()
    {
        optionsManager = FindAnyObjectByType<OptionsManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (optionsManager.IsOptionsPanelOpen()) {
                optionsManager.CloseOptionsPanel();
            } else {
                optionsManager.OpenOptionsPanel();
            }
        }
    }

    public GameState GetCurrentGameState()
    {
        return state;
    }

    public void SetGameState(GameState newState)
    {
        if (newState == GetCurrentGameState()) {
            // Do nothing
        } else {
            state = newState;
        }
    }
}
