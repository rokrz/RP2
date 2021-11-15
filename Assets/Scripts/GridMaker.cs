﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GridMaker : MonoBehaviour
{
    public Dictionary<ElementTypes, char> elementValues = new Dictionary<ElementTypes, char>();
    int rows, cols;
    public String[] worldNames = {"TESTE", "Igual", "IgualI", "IgualH", "Soma", "Subtracao", "Multiplica", "Divisao" };
    public Dictionary<String, int> levelsPerWorld = new Dictionary<string, int>();
    public GameObject cellHolder;
    public Dictionary<String,List<LevelCreator>> worldHolder = new Dictionary<String, List<LevelCreator>>();
    public List<LevelCreator> levelHolder;
    public List<GameObject> cells = new List<GameObject>();
    public List<GameObject> background = new List<GameObject>();
    public List<SpriteLibrary> spriteLibrary = new List<SpriteLibrary>();
    public static GridMaker instance = null;
    public GameObject boundary;
    int currentWorld = 0;
    int currentLevel = 0;
    int totalIguais = 0;
    public int currentIguais = 0;
    private UIManager ui;
    public Dictionary<string, Dialogue> dialogues;
    public DialogueTrigger dialogueTrigger;
    public Animator levelCompleteBox;
    public bool isEndLevel { get; private set; }

    public int Rows
    {
        get { return rows; }
    }

    public int Cols
    {
        get { return cols; }
    }
 
    public Vector2 Return2D(int i)
    {
        return new Vector2(i % cols, i / rows);
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            DontDestroyOnLoad(this);
    }
  
    //Método de inicializacao
    void Start()
    {
        isEndLevel = false;
        InitializeElementValues();
        loadLevelsPerWorld();
        InitializeLevelMap();
        InitializeDialogueElements();
        ui = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (!PlayerPrefs.HasKey("World"))
        {
            PlayerPrefs.SetInt("World", 0);
        }
        if (!PlayerPrefs.HasKey("Level"))
        {
            PlayerPrefs.SetInt("Level", 0);
        }
        if (PlayerPrefs.GetInt("World") >= worldNames.Length)
        {
            PlayerPrefs.SetInt("World", 0);
        }
        if (PlayerPrefs.GetInt("Level") >= worldHolder[worldNames[PlayerPrefs.GetInt("World")]].Count)
        {
            PlayerPrefs.SetInt("Level", 0);
        }
        currentWorld = PlayerPrefs.GetInt("World");
        currentLevel = PlayerPrefs.GetInt("Level");
        levelHolder = worldHolder[worldNames[currentWorld]];
        Debug.Log(currentWorld + " " + currentLevel + " " + levelHolder.Count);
        Debug.Log(levelHolder);
        if (levelHolder[0] != null)
        {
            Debug.Log(levelHolder[0].level);
        }
        else
        {
            Debug.Log("Not yet");
        }
        float count = levelHolder[currentLevel].level.Count;
        rows = (int)Mathf.Sqrt(count);
        cols = rows;

        StartDialogue(worldNames[PlayerPrefs.GetInt("World")]+" "+(PlayerPrefs.GetInt("Level")+1));
        ui.updateCurrentLevelText();
        CreateGrid();
    }

    void InitializeLevelMap()
    {
        Debug.Log("WorldNames size: "+worldNames.Length);
        foreach(String worldName in worldNames){
            List<LevelCreator> auxList = new List<LevelCreator>();
            for (int i = 0; i < levelsPerWorld[worldName]; i++)
            {
                String levelName = "Levels/" + worldName + " " + (i+1);
                Debug.Log(levelName);
                LevelCreator lc = Resources.Load<LevelCreator>(levelName);
                if(lc == null)
                {
                    Debug.Log("Loaded null level");
                }
                auxList.Add(lc);
            }
            Debug.Log(worldName+" has "+auxList.Count+" levels loaded");
            worldHolder.Add(worldName, auxList);
        }
    }

    void loadLevelsPerWorld()
    {
        foreach(String s in worldNames)
        {
            levelsPerWorld.Add(s,1);
        }
    }

    //Método de update
    void Update()
    {
        //Retirar para versão final
        if (Input.GetKeyDown(KeyCode.N))
        {
            NextLevel();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if(currentIguais == totalIguais)
        {
            Debug.Log("Player Won!");
            GridMaker.instance.LoadLevelCompleteBox();
        }
    }

    //Desenha o Mapa
    public void CreateGrid()
    {
        //Coloca paredes
        for (int gI = -1; gI <= rows; gI += 1)
        {
            for (int gJ = -1; gJ <= rows; gJ += 1)
            {
                if (gI == -1 || gJ == -1 || gI == rows || gJ == rows)
                    Instantiate(boundary, new Vector3(gI, gJ, 0), Quaternion.identity);
            }
        }
        int counterBg = 0;
        for (int j = 0; j < levelHolder[currentLevel].level.Count; j++)
        {
            GameObject bg = Instantiate(cellHolder, new Vector3(counterBg % cols, counterBg / rows, 0), Quaternion.identity);
            background.Add(bg);
            ElementTypes currentElementBg = ElementTypes.Empty;
            bg.GetComponent<CellProperty>().AssignInfo(counterBg / rows, counterBg % cols, currentElementBg);
            counterBg++;
        }
        int counter = 0;
        for (int i = 0; i < levelHolder[currentLevel].level.Count; i++)
        {
            GameObject g = Instantiate(cellHolder, new Vector3(counter % cols, counter / rows, 0), Quaternion.identity);
            cells.Add(g);
            ElementTypes currentElement = levelHolder[currentLevel].level[i];
            if (currentElement == ElementTypes.BlocoIgual) totalIguais += 1;
            g.GetComponent<CellProperty>().AssignInfo(counter / rows, counter % cols, currentElement);
            counter++;
        }

    }

    //Verifica se as coordenadas são Parada
    public bool IsStop(int r, int c, Vector2 dir)
    {
        bool isPush = false;
        int curRow = r, curCol = c;
        List<GameObject> atRC = FindObjectsAt(curRow, curCol);
        if (r >= rows || c >= cols || r < 0 || c < 0)
            return true;
        foreach (GameObject g in atRC)
        {
            CellProperty currentCell = g.GetComponent<CellProperty>();

            if (currentCell.IsStop)
            {
                return true;
            }
            else if (currentCell.IsPushable)
            {
                isPush = true;
            }
        }

        if (!isPush)
            return false;

        if (dir == Vector2.right)
        {
            return IsStop(curRow, curCol + 1, Vector2.right);
        }
        else if (dir == Vector2.left)
        {
            return IsStop(curRow, curCol - 1, Vector2.left);
        }
        else if (dir == Vector2.up)
        {
            return IsStop(curRow + 1, curCol, Vector2.up);
        }
        else if (dir == Vector2.down)
        {
            return IsStop(curRow - 1, curCol, Vector2.down);
        }
        return true;
    }

    //Retorna as Celulas do tipo e
    public List<CellProperty> GetAllCellsOf(ElementTypes e)
    {
        List<CellProperty> cellProp = new List<CellProperty>();

        foreach (GameObject g in cells)
        {
            if (g != null && g.GetComponent<CellProperty>().Element == e)
            {
                cellProp.Add(g.GetComponent<CellProperty>());
            }
        }
        return cellProp;

    }

    //verifica se há bloco empurravel na posicao [r,c]
    public bool IsTherePushableObjectAt(int r, int c)
    {
        List<GameObject> objectsAtRC = FindObjectsAt(r, c);

        foreach (GameObject g in objectsAtRC)
        {
            if (g.GetComponent<CellProperty>().IsPushable)
            {
                return true;
            }
        }
        return false;

    }

    //retorna os objetos na posicao [r,c]
    public List<GameObject> FindObjectsAt(int r, int c)
    {
        return cells.FindAll(x => x != null && x.GetComponent<CellProperty>().CurrentRow == r && x.GetComponent<CellProperty>().CurrentCol == c);
    }

    //retorna o objeto empurravel na posicao [r,c]
    public GameObject GetPushableObjectAt(int r, int c)
    {
        List<GameObject> objectsAtRC = FindObjectsAt(r, c);

        foreach (GameObject g in objectsAtRC)
        {
            if (g.GetComponent<CellProperty>().IsPushable)
            {
                return g;
            }
        }

        return null;
    }

    //Carrega a caixa de nivel completo
    public void LoadLevelCompleteBox()
    {
        isEndLevel = true;
        levelCompleteBox.SetBool("isCompleted",true);
    }

    //Return to main menu
    public void ReturnToMainMenu()
    {
        levelCompleteBox.SetBool("isCompleted", false);
        SceneManager.LoadScene("Menu");
    }

    //passa para o proximo nivel
    public void NextLevel()
     {
        totalIguais = 0;
        isEndLevel = false;
        levelCompleteBox.SetBool("isCompleted",false);
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
        if (PlayerPrefs.GetInt("Level") >= levelHolder.Count)
         {
            PlayerPrefs.SetInt("World",PlayerPrefs.GetInt("World")+1);
            if (PlayerPrefs.GetInt("World") >= worldNames.Length)
            {
                SceneManager.LoadScene("Menu");
            }
            else
            {
                PlayerPrefs.SetInt("Level", 0);
                levelHolder = worldHolder[worldNames[PlayerPrefs.GetInt("World")]];
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
             
        }
         else
         {
            Debug.Log("Changing to level "+ PlayerPrefs.GetInt("Level"));
            ui.updateCurrentLevelText();
            StartDialogue(worldNames[PlayerPrefs.GetInt("World")]+" "+(PlayerPrefs.GetInt("Level")+1));
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
         }
     }

    /* 
     * Parte responsável pela gestão de diálogos
     */

    //Verifica se existe um dialogo para o nivel desejado
    public bool VerifyExistingDialogue(String currentLevel)
    {
        Debug.Log("Dialogue for "+currentLevel);
        if (dialogues.ContainsKey(currentLevel))
        {
            Debug.Log(" Found");
            return true;
        }
        else
        {
            Debug.Log(" Not found");
            return false;
        }
    }

    //Verifica e inicia os dialogos caso exista algum
    public void StartDialogue(String currentLevel)
    {
        if (VerifyExistingDialogue(currentLevel))
        {
            Dialogue dialogue = dialogues[currentLevel];
            dialogueTrigger.dialogue = dialogue;
            dialogueTrigger.TriggerDialogue();
        }
    }

    //carrega os dialgos para o Dict dialogues
    public void LoadDialogues()
    {
        Dialogue dialogue = new Dialogue();
        dialogue.name = "Equatron";
        dialogue.sentences = new string[3];
        dialogue.sentences[0] = "Olá, meu nome é Equatron, e estou aqui para guiá-lo pelos comandos do jogo";
        dialogue.sentences[1] = "Primeiramente, para se movimentar, use as setas do teclado.";
        dialogue.sentences[2] = "Monte igualdades para passar para o próximo nível";
        dialogues.Add("Igual 1", dialogue);
    }

    //Carrega os elementos referentes aos dialogos
    void InitializeDialogueElements()
    {
        dialogues = new Dictionary<string, Dialogue>();
        LoadDialogues();
    }

    //Carrega os valores de cada elemento
    void InitializeElementValues()
    {
        elementValues.Add(ElementTypes.Bloco0,'0');
        elementValues.Add(ElementTypes.Bloco1, '1');
        elementValues.Add(ElementTypes.Bloco2, '2');
        elementValues.Add(ElementTypes.Bloco3, '3');
        elementValues.Add(ElementTypes.Bloco4, '4');
        elementValues.Add(ElementTypes.Bloco5, '5');
        elementValues.Add(ElementTypes.Bloco6, '6');
        elementValues.Add(ElementTypes.Bloco7, '7');
        elementValues.Add(ElementTypes.Bloco8, '8');
        elementValues.Add(ElementTypes.Bloco9, '9');
        elementValues.Add(ElementTypes.BlocoSoma, '+');
        elementValues.Add(ElementTypes.BlocoIgual, '=');
        elementValues.Add(ElementTypes.BlocoDivide, '/');
        elementValues.Add(ElementTypes.BlocoSubtrai, '-');
        elementValues.Add(ElementTypes.BlocoAbreP, '(');
        elementValues.Add(ElementTypes.BlocoFechaP, ')');
        elementValues.Add(ElementTypes.BlocoMultiplica, '*');
    }

    
}
[System.Serializable]
public class SpriteLibrary
{

    public ElementTypes element;
    public Sprite sprite;

}
