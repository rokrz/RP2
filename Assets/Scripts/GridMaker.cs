using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GridMaker : MonoBehaviour
{
    public GameObject levelInfo;
    private LevelInfo li;
    public Dictionary<ElementTypes, char> elementValues = new Dictionary<ElementTypes, char>();
    int rows, cols;
    public String[] worldNames;
    public Dictionary<String, int> levelsPerWorld;
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
            Destroy(this);
    }
  
    //Método de inicializacao
    void Start()
    {
        li = levelInfo.GetComponent<LevelInfo>();
        worldNames = li.worldNames;
        li.loadLevelsPerWorld();
        levelsPerWorld = li.levelsPerWorld;

        isEndLevel = false;
        InitializeElementValues();
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
            Debug.Log("Indexing "+worldName);
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

    //Método de update
    void Update()
    {
        if (isEndLevel && Input.GetKeyDown(KeyCode.Space))
        {
            NextLevel();
        }
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
        //Igual 1
        dialogue.name = "Equatron";
        dialogue.sentences = new string[3];
        dialogue.sentences[0] = "Olá, meu nome é Equatron, e estou aqui para guiá-lo pelos comandos do jogo";
        dialogue.sentences[1] = "Primeiramente, para se movimentar, use as setas do teclado.";
        dialogue.sentences[2] = "Monte igualdades para passar para o próximo nível";
        dialogues.Add("Igual 1", dialogue);
        //Igual 2
        dialogue = new Dialogue();
        dialogue.name = "Equatron";
        dialogue.sentences = new string[1];
        dialogue.sentences[0] = "Dá para arrastar mais de um bloco por vez!";
        dialogues.Add("Igual 2", dialogue);
        //Igual 3
        dialogue = new Dialogue();
        dialogue.name = "Equatron";
        dialogue.sentences = new string[1];
        dialogue.sentences[0] = "Para completar as fases, faça com que os dois lados do igual (=) estejam equivalentes!";
        dialogues.Add("Igual 3", dialogue);
        //Igual 4
        dialogue = new Dialogue();
        dialogue.name = "Equatron";
        dialogue.sentences = new string[1];
        dialogue.sentences[0] = "Você pode substituir os blocos, empurrando eles!";
        dialogues.Add("Igual 4", dialogue);
        //Igual 5
        dialogue = new Dialogue();
        dialogue.name = "Equatron";
        dialogue.sentences = new string[1];
        dialogue.sentences[0] = "Você pode montar tanto na horizontal quanto na vertical!";
        dialogues.Add("Igual 5", dialogue);
        //Soma 1
        dialogue = new Dialogue();
        dialogue.name = "Equatron";
        dialogue.sentences = new string[2];
        dialogue.sentences[0] = "Quando voce vir o símbolo de soma, +, você pode juntar os valores de dois números";
        dialogue.sentences[1] = "Por exemplo, 1+1=2, 1+2=3!";
        dialogues.Add("Soma 1", dialogue);
        //Soma 2
        dialogue = new Dialogue();
        dialogue.name = "Equatron";
        dialogue.sentences = new string[2];
        dialogue.sentences[0] = "A ordem dos números não altera o resultado final!";
        dialogue.sentences[1] = "Complete as duas equações para passar de fase";
        dialogues.Add("Soma 2", dialogue);
        //Soma 3
        dialogue = new Dialogue();
        dialogue.name = "Equatron";
        dialogue.sentences = new string[2];
        dialogue.sentences[0] = "As vezes, é necessário somar mais de dois números para chegar no resultado necessário";
        dialogue.sentences[1] = "Por exemplo, 1+1+1=3, 1+2+3=6!";
        dialogues.Add("Soma 3", dialogue);
        //Soma 4
        dialogue = new Dialogue();
        dialogue.name = "Equatron";
        dialogue.sentences = new string[1];
        dialogue.sentences[0] = "Tente mais uma vez agora! Lembra do nível anterior!";
        dialogues.Add("Soma 4", dialogue);
        //Soma 5
        dialogue = new Dialogue();
        dialogue.name = "Equatron";
        dialogue.sentences = new string[2];
        dialogue.sentences[0] = "O 0 é o elemento neutro da adição! Qualquer número somado com ele, resulta nele mesmo!";
        dialogue.sentences[1] = "Por exemplo, 1+0=1, 2+0=2, e por ai vai!";
        dialogues.Add("Soma 5", dialogue);
        //Subtracao 1
        dialogue = new Dialogue();
        dialogue.name = "Equatron";
        dialogue.sentences = new string[3];
        dialogue.sentences[0] = "Para subtrair,você pega o segundo elemento, e tira ele do primeiro!";
        dialogue.sentences[1] = "Como resultado você tem o que sobra do primeiro";
        dialogue.sentences[2] = "Como por exemplo: se eu tenho 3 balas, e dou 2 para um amigo, fico com 1 só!";
        dialogues.Add("Subtracao 1", dialogue);
        //Subtracao 2
        dialogue = new Dialogue();
        dialogue.name = "Equatron";
        dialogue.sentences = new string[3];
        dialogue.sentences[0] = "Assim como na soma, a subtração também tem um elemento neutro!";
        dialogue.sentences[1] = "Esse elemento é o 0. Toda vez que você tira 0 de um número, ele continua igual.";
        dialogue.sentences[2] = "Como por exemplo: se eu tenho 5 balas, e dou 0 para um amigo, eu continuo com 5!";
        dialogues.Add("Subtracao 2", dialogue);
        //Subtracao 3
        dialogue = new Dialogue();
        dialogue.name = "Equatron";
        dialogue.sentences = new string[2];
        dialogue.sentences[0] = "Se você remove um número dele mesmo, você fica com 0!";
        dialogue.sentences[1] = "Como por exemplo: se eu tenho 10 balas, e dou 10 para meus amigos, fico nenhuma bala!";
        dialogues.Add("Subtracao 3", dialogue);
        //Subtracao 4
        dialogue = new Dialogue();
        dialogue.name = "Equatron";
        dialogue.sentences = new string[3];
        dialogue.sentences[0] = "Diferente da soma, a ordem na subtração importa sim!";
        dialogue.sentences[1] = "Já que sempre retiro o segundo número do primeiro, mudar a ordem deles muda também nosso resultado!";
        dialogue.sentences[2] = "Como por exemplo: se eu tenho 5 balas, e dou 7 para um amigo, fico devendo 2 balas para ele ainda!";
        dialogues.Add("Subtracao 4", dialogue);
        //Subtracao 5
        dialogue = new Dialogue();
        dialogue.name = "Equatron";
        dialogue.sentences = new string[2];
        dialogue.sentences[0] = "Sempre que você retira de um número uma quantidade maior do que ele, você acaba com um número negativo.";
        dialogue.sentences[1] = "O número negativo representa uma falta. Algo que nãoexiste, mas precisa ser preenchido";
        dialogues.Add("Subtracao 5", dialogue);
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
