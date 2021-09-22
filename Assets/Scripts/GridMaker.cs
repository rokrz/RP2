using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GridMaker : MonoBehaviour
{
    public Dictionary<ElementTypes, char> elementValues = new Dictionary<ElementTypes, char>();
    int rows, cols;
    public Text levelText;
    public GameObject cellHolder;
    public List<LevelCreator> levelHolder = new List<LevelCreator>();
    public List<GameObject> cells = new List<GameObject>();
    public List<SpriteLibrary> spriteLibrary = new List<SpriteLibrary>();
    public static GridMaker instance = null;
    public GameObject boundary;
    int currentLevel = 0;

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
  
    void Start()
    {
        initializeElementValues();
        if (!PlayerPrefs.HasKey("Level"))
        {
            PlayerPrefs.SetInt("Level", 0);
        }
        currentLevel = PlayerPrefs.GetInt("Level");
        float count = levelHolder[currentLevel].level.Count;
        rows = (int)Mathf.Sqrt(count);
        cols = rows;

        CreateGrid();
    }

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
        int counter = 0;
        for (int i = 0; i < levelHolder[currentLevel].level.Count; i++)
        {
            if (levelHolder[currentLevel].level[i] != ElementTypes.Empty)
            {
                GameObject g = Instantiate(cellHolder, new Vector3(counter % cols, counter / rows, 0), Quaternion.identity);
                cells.Add(g);
                ElementTypes currentElement = levelHolder[currentLevel].level[i];
                //Pega o Valor do elemento
                //ElementValues currentValue = ElementValues;
                g.GetComponent<CellProperty>().AssignInfo(counter / rows, counter % cols, currentElement);
                //Debug.Log( currentElement.ToString() + "R : " + i / rows + " C : " + i % cols);


            }
            counter++;
        }

    }

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

    public List<GameObject> FindObjectsAt(int r, int c)
    {
        return cells.FindAll(x => x != null && x.GetComponent<CellProperty>().CurrentRow == r && x.GetComponent<CellProperty>().CurrentCol == c);
    }

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

    public void NextLevel()
     {
         if (PlayerPrefs.GetInt("Level") >= levelHolder.Count)
         {
             SceneManager.LoadScene("Menu");
         }
         else
         {
             levelText.text = "Nível " + PlayerPrefs.GetInt("Level");
             SceneManager.LoadScene(SceneManager.GetActiveScene().name);
         }
     }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }


    void initializeElementValues()
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
