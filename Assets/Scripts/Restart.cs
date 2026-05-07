
using UnityEngine;
using UnityEngine.SceneManagement;


public class Restart : MonoBehaviour
{
    public GameObject spawnPoint;



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartScene();

            Debug.Log("Hello after press ");
        }
    }

    public void RestartScene()
    {
        GameObject Player = GameObject.FindWithTag("Player");
       // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        Player.transform.position = spawnPoint.transform.position;
    }
}
