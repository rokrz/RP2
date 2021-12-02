using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    public GameObject myMovie;
    public GameObject panel;
    public Camera tela;
    public float tempo = 10;
    // Start is called before the first frame update
    void Start()
    {
        
        myMovie.SetActive(true);
        myMovie.GetComponent<VideoPlayer>().Play();
        Destroy(myMovie, tempo);

    }

    private void OnGUI()
    {
        //GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), myMovie.texture);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
