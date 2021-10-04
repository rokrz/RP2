using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistMusic : MonoBehaviour
{
    private void Start()
    {

    }
    //play global
    private static PersistMusic instance = null;
    public static PersistMusic Instance
    {
        get { return instance; }
    }
    private void Awake()
    {
        //GameObject[] objs = GameObject.FindGameObjectsWithTag("music");
        //if (objs.Length > 1)
        //   Destroy(this.gameObject);
        //DontDestroyOnLoad(this.gameObject);
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);

    }

    private void Update()
    {

    }
}
