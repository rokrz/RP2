using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GridMaker : MonoBehaviour
{
    public Dictionary<ElementTypes, char> elementValues = new Dictionary<ElementTypes, char>();
    int rows, cols;
    public GameObject cellHolder;
    public List<LevelCreator> levelHolder = new List<LevelCreator>();
    public List<GameObject> cells = new List<GameObject>();
    public List<GameObject> background = new List<GameObject>();
    public List<SpriteLibrary> spriteLibrary = new List<SpriteLibrary>();
    public static GridMaker instance = null;
    public GameObject boundary;
    int currentLevel = 0;
    private UIManager ui;
    public Dictionary<string, Dialogue> dialogues;
    public DialogueTrigger dialogueTrigger;

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
        InitializeElementValues();
        InitializeDialogueElements();
        ui = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (!PlayerPrefs.HasKey("Level"))
        {
            PlayerPrefs.SetInt("Level", 0);
        }
        if (PlayerPrefs.GetInt("Level") >= levelHolder.Count)
        {
            PlayerPrefs.SetInt("Level", 0);
        }
        currentLevel = PlayerPrefs.GetInt("Level");
        float count = levelHolder[currentLevel].level.Count;
        rows = (int)Mathf.Sqrt(count);
        cols = rows;

        StartDialogue(PlayerPrefs.GetInt("Level"));

        CreateGrid();
    }

    //Método de update
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    //passa para o proximo nivel
    public void NextLevel()
     {
         if (PlayerPrefs.GetInt("Level") >= levelHolder.Count)
         {
             SceneManager.LoadScene("Menu");
         }
         else
         {
            Debug.Log("Changing to level "+ PlayerPrefs.GetInt("Level"));
             ui.updateCurrentLevelText();
            StartDialogue(PlayerPrefs.GetInt("Level"));
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
         }
     }

    /* 
     * Parte responsável pela gestão de diálogos
     */

    //Verifica se existe um dialogo para o nivel desejado
    public bool VerifyExistingDialogue(int currentLevel)
    {
        if (dialogues.ContainsKey(currentLevel+ ""))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Verifica e inicia os dialogos caso exista algum
    public void StartDialogue(int currentLevel)
    {
        int key = currentLevel + 1;
        if (VerifyExistingDialogue(key))
        {
            Dialogue dialogue = dialogues[key.ToString()];
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
        dialogues.Add("1", dialogue);
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
