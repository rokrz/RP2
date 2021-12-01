using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfo : MonoBehaviour
{
    public string[] worldNames = { "Igual", "Soma", "Subtracao", "Multiplica", "Divisao" };
    public Dictionary<string,int> levelsPerWorld= new Dictionary<string, int>();

    private void Start()
    {
        loadLevelsPerWorld();
    }

    public void loadLevelsPerWorld()
    {
        if (levelsPerWorld.Count == 0)
        {
            foreach (string s in worldNames)
            {
                Debug.Log("Indexing " + s + " in LevelWiki");
                levelsPerWorld.Add(s, 25);
            }
        }
        
    }

}
