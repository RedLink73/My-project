using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
   
    public static GameManager instance;
    
    [SerializeField] private GameObject playerToSpawn;
    [SerializeField] private CinemachineCamera cinemachineCamera;

    public GameObject currentPlayer;
    private Events _events;
    private NextLevel _nextLevel;
    public List<SceneReference> scenes;
    int levelIndex = 1;
    
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        _events = new Events();
        _events.A_OnLevelComplete += OnLevelLoaded;
        _nextLevel = FindObjectOfType<NextLevel>();
        _nextLevel.Init(_events);
        
        if (SpawnPoint.SpawnPoints.Count != 0)
        {
            SpawnPlayer(SpawnPoint.SpawnPoints[0].transform.position);
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public void Init()
    {
        if (instance == null)
        {
            var go = new GameObject("GameManager");
            instance = go.AddComponent<GameManager>();
            DontDestroyOnLoad(go);
        }

        instance = this;
    }

    void SpawnPlayer(Vector3 SpawnPoint)
    {
        currentPlayer = Instantiate(playerToSpawn, SpawnPoint, Quaternion.identity);
        currentPlayer.GetComponentInChildren<CinemachineCamera>().Follow = currentPlayer.transform;
        currentPlayer.GetComponent<PlayerMovement>().restristedStates.Clear();
        
        foreach (var state in _nextLevel.restrictedStates)
        {
            if (state.script != null)
            {
                currentPlayer.GetComponent<PlayerMovement>().restristedStates.Add(state.scriptName);
            }
        } 
    }

    void OnLevelLoaded()
    {
        StartCoroutine(LoadLevelAsync(scenes[levelIndex].SceneName));
        levelIndex++;
    }
    
    public IEnumerator LoadLevelAsync(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        while (!op.isDone)
        {
            yield return null;
        }
        
        _nextLevel = NextLevel.LevelCompletePoints[0];
        _nextLevel.Init(_events);
        SpawnPlayer(SpawnPoint.SpawnPoints[0].transform.position);
    }
    
    private void OnValidate()
    {
        foreach (var scene in scenes)
        {
            scene.UpdateName();
        }
    }
}