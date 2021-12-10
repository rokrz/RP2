using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoController : MonoBehaviour
{
    public GameObject myMovie;
    public float tempo;
    // Start is called before the first frame update
    void Start()
    {
        myMovie.GetComponent<VideoPlayer>().Prepare();
        myMovie.GetComponent<VideoPlayer>().Play();        
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SceneManager.LoadScene("Menu");
        }
        else if (myMovie.GetComponent<VideoPlayer>().isPaused)
        {
            Destroy(myMovie, tempo);
            SceneManager.LoadScene("Menu");
        }
    }
}
