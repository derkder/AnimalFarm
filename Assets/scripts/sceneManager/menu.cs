using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnGUI()
    {
        Event e = Event.current; ;
        if (e.isKey)
        {
            Debug.Log("nextScece");
            PlayGame();
        }
    }
    void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
