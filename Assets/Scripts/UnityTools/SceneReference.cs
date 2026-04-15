using UnityEngine;

[System.Serializable]
public class SceneReference
{
    [SerializeField] private string sceneName;

#if UNITY_EDITOR
    [SerializeField] public UnityEditor.SceneAsset sceneAsset;
#endif

    public string SceneName => sceneName;

    public void UpdateName()
    {
#if UNITY_EDITOR
        if (sceneAsset != null)
        {
            sceneName = sceneAsset.name;
        }
#endif
    }
}