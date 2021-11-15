using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class WorldSelector : MonoBehaviour
{
    public GridMaker gridMaker;
    public GameObject worldHolder; // panel
    public GameObject worldIcon; //button prefab
    public GameObject thisCanvas;
    public int numberOfWorlds = 50; // number of levels
    public Vector2 iconSpacing;
    private Rect panelDimensions;
    private Rect iconDimensions;
    private int amountPerPage;
    private int currentLevelCount;
    private string[] worldNames;


    // Start is called before the first frame update
    void Start()
    {
        gridMaker = GameObject.FindObjectOfType<GridMaker>();
        if (gridMaker != null)
        {
            Debug.Log("Grid is not null");
        }
        else
        {
            Debug.Log("Grid is null broo");
        }
        worldNames = gridMaker.worldNames;
        numberOfWorlds = worldNames.Length;
        panelDimensions = worldHolder.GetComponent<RectTransform>().rect;
        iconDimensions = worldIcon.GetComponent<RectTransform>().rect;
        int maxInARow = Mathf.FloorToInt((panelDimensions.width + iconSpacing.x) / (iconDimensions.width + iconSpacing.x));
        int maxInACol = Mathf.FloorToInt((panelDimensions.height + iconSpacing.y) / (iconDimensions.height + iconSpacing.y));
        amountPerPage = maxInARow * maxInACol;
        Debug.Log(amountPerPage);
        int totalPages = Mathf.CeilToInt((float)numberOfWorlds / amountPerPage);
        LoadPanels(totalPages);
    }
    void LoadPanels(int numberOfPanels)
    {
        GameObject panelClone = Instantiate(worldHolder) as GameObject;
        PageSwiper swiper = worldHolder.AddComponent<PageSwiper>();
        swiper.totalPages = numberOfPanels;

        for (int i = 1; i <= numberOfPanels; i++)
        {
            GameObject panel = Instantiate(panelClone) as GameObject;
            panel.transform.SetParent(thisCanvas.transform, false);
            panel.transform.SetParent(worldHolder.transform);
            panel.name = "Page-" + i;
            panel.GetComponent<RectTransform>().localPosition = new Vector2(panelDimensions.width * (i - 1), 0);
            SetUpGrid(panel);
            int numberOfIcons = i == numberOfPanels ? numberOfWorlds - currentLevelCount : amountPerPage;
            LoadIcons(numberOfIcons, panel);
        }
        Destroy(panelClone);
    }
    void SetUpGrid(GameObject panel)
    {
        GridLayoutGroup grid = panel.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(iconDimensions.width, iconDimensions.height);
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.spacing = iconSpacing;
    }
    void LoadIcons(int numberOfIcons, GameObject parentObject)
    {
        for (int i = 0; i < numberOfIcons; i++)
        {
            currentLevelCount++;
            GameObject icon = Instantiate(worldIcon) as GameObject;
            icon.transform.SetParent(thisCanvas.transform, false);
            icon.transform.SetParent(parentObject.transform);
            icon.name = "Level " + i;
            icon.GetComponentInChildren<Text>().text = ("Mundo "+worldNames[currentLevelCount]);
        }
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    // Update is called once per frame
    void Update()
    {

    }
}