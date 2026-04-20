using UnityEngine;

[System.Serializable]
public class ScriptReference
{
#if UNITY_EDITOR
    public UnityEditor.MonoScript script;
#endif

    public string scriptName;

    public bool IsValid()
    {
#if UNITY_EDITOR
        if (script == null) return false;

        var type = script.GetClass();
        return type != null && typeof(State).IsAssignableFrom(type);
#else
        return true;
#endif
    }
}