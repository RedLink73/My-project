using UnityEngine;

public class GameManagerBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (Object.FindFirstObjectByType<GameManager>() != null)
            return;

        GameObject prefab = Resources.Load<GameObject>("Managers");

        if (prefab == null)
        {
            Debug.LogError("GameManager prefab not found in Resources!");
            return;
        }

        Object.Instantiate(prefab);
    }
}