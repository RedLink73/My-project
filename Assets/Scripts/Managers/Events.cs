using System;

public class Events
{
    public Action A_OnLevelComplete;
    public Action A_OnPlayerDeath;
    
    public void OnPlayerDeath()
    {
        A_OnPlayerDeath?.Invoke();
        // Handle player death event
    }

    public void OnLevelComplete()
    {
        // Handle level complete event
        A_OnLevelComplete?.Invoke();
    }
}