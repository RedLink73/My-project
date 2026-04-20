using System;
using System.Collections.Generic;
using UnityEngine;


public class SpawnPoint : MonoBehaviour
{
    private Events _events;
    public static List<SpawnPoint> SpawnPoints = new List<SpawnPoint>();
   
    private void OnEnable()
    {
        SpawnPoints.Add(this);
    }

    private void OnDisable()
    {
        SpawnPoints.Remove(this);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.orange;
        Gizmos.DrawSphere(transform.position, 0.3f);
    }
    
}

