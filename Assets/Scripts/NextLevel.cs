using System;
using System.Collections.Generic;
using UnityEngine;


public class NextLevel : MonoBehaviour
{
    private Events _events;
    public List<ScriptReference> restrictedStates = new List<ScriptReference>();
    
    public static List<NextLevel> LevelCompletePoints = new List<NextLevel>();
    
    
    public void Init(Events events)
    {
        _events = events;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            // We have Collided with player.
            Debug.Log(other.name);
            _events.A_OnLevelComplete?.Invoke();
        }
    }

    private void OnEnable()
    {
        LevelCompletePoints.Add(this);
    }

    private void OnDisable()
    {
        LevelCompletePoints.Remove(this);
    }
    
    private void OnValidate()
    {
#if UNITY_EDITOR
        foreach (var s in restrictedStates)
        {
            if (s.script != null)
            {
                var type = s.script.GetClass();

                if (typeof(State).IsAssignableFrom(type))
                {
                    s.scriptName = s.script.name;
                }
                else
                {
                    Debug.LogWarning($"{s.script.name} is not a valid State!");
                    s.script = null;
                }
            }
        }
#endif
    }
    
    
}