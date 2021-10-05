using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NCalc;

public class CellProperty : MonoBehaviour
{

    ElementTypes element;
    bool isPushable;
    bool isPlayer;
    bool isStop;
    bool isEqual;
    int currentRow, currentCol;
    SpriteRenderer spriteRenderer;
    SpriteAnimator animator;

    public ElementTypes Element
    {
        get { return element; }
    }
    public bool IsStop
    {
        get { return isStop; }
    }
    public bool IsEqual
    {
        get { return isEqual; }
    }
    public bool IsPushable
    {
        get { return isPushable; }
    }
    public int CurrentRow
    {
        get { return currentRow; }
    }
    public int CurrentCol
    {
        get { return currentCol; }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<SpriteAnimator>();
    }

    public void AssignInfo(int r, int c, ElementTypes e)
    {
        currentRow = r;
        currentCol = c;
        element = e;
        ChangeSprite();
        if (e == ElementTypes.Wall)
        {
            isStop = true;
        }
        if ((int)element>=3)
        {
            isPushable = true;
            spriteRenderer.sortingOrder = 100;
        }
        if (e == ElementTypes.Robozin)
        {
            isPlayer = true;
            spriteRenderer.sortingOrder = 100;
        }
        if (e == ElementTypes.BlocoIgual)
        {
            isEqual = true;
            spriteRenderer.sortingOrder = 100;
        }
        if (e == ElementTypes.Empty)
        {
            spriteRenderer.sortingOrder = 5;
        }
        if(e!=ElementTypes.Empty && e!=ElementTypes.Robozin && e != ElementTypes.Wall)
        {
            LoadSpriteAnimator(e);
        }
    }

    public void LoadSpriteAnimator(ElementTypes e)
    {
        String spriteName = "Sprites/" + e.ToString()+"_Idle";
        animator.spriteRenderer = spriteRenderer;
        SpriteAnimator.AnimationTrigger animationTrigger = new SpriteAnimator.AnimationTrigger();
        animationTrigger.name = "idle";
        animationTrigger.frame = 0;
        SpriteAnimator.Animation animation = new SpriteAnimator.Animation();
        animation.fps = 4;
        animation.name = "idle";
        animation.triggers = new SpriteAnimator.AnimationTrigger[] { animationTrigger };
        Sprite[] frames = Resources.LoadAll<Sprite>(spriteName); ;
        animation.frames = frames;
        SpriteAnimator.Animation[] animations = new SpriteAnimator.Animation[1] { animation };
        animator.animations = animations;
        animator.Play(animation.name);
    }

    public void Initialize()
    {
        isPushable = false;
        isPlayer = false;
        isStop = false;
        isEqual = false;

        if ((int)element >= 99)
        {
            isPushable = true;
        }
    }


    public void ChangeSprite()
    {
        Sprite s = GridMaker.instance.spriteLibrary.Find(x => x.element == element).sprite;
        spriteRenderer.sprite = s;
        
        if (isPlayer || isPushable)
        {
            spriteRenderer.sortingOrder = 100;
        }
        else
        {
            spriteRenderer.sortingOrder = 10;
        }
    }

    public void IsItPushable(bool isPush)
    {
        isPushable = isPush;
    }

    public void IsItEqual(bool isEqual)
    {
        this.isEqual = isEqual;
    }

    public void IsPlayer(bool isP)
    {
        isPlayer = isP;
    }

    void Update()
    {
        if (isPlayer)
        {
            List<GameObject> movingObject = new List<GameObject>();
            if (Input.GetKeyDown(KeyCode.RightArrow) && currentCol + 1 < GridMaker.instance.Cols && !GridMaker.instance.IsStop(currentRow, currentCol + 1, Vector2.right))
            {
                movingObject.Add(this.gameObject);

                for (int c = currentCol + 1; c < GridMaker.instance.Cols - 1; c++)
                {
                    if (GridMaker.instance.IsTherePushableObjectAt(currentRow, c))
                    {
                        movingObject.Add(GridMaker.instance.GetPushableObjectAt(currentRow, c));
                    }
                    else
                    {
                        break;
                    }
                }
                foreach (GameObject g in movingObject)
                {
                    if (g.GetComponent<CellProperty>().isPlayer)
                    {
                        transform.right = Vector3.right;
                    }
                    g.transform.position = new Vector3(g.transform.position.x + 1, g.transform.position.y, g.transform.position.z);
                    g.GetComponent<CellProperty>().currentCol++;
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) && currentCol - 1 >= 0 && !GridMaker.instance.IsStop(currentRow, currentCol - 1, Vector2.left))
            {
                movingObject.Add(this.gameObject);

                for (int c = currentCol - 1; c > 0; c--)
                {
                    if (GridMaker.instance.IsTherePushableObjectAt(currentRow, c))
                    {
                        movingObject.Add(GridMaker.instance.GetPushableObjectAt(currentRow, c));
                    }
                    else
                    {
                        break;
                    }
                }
                foreach (GameObject g in movingObject)
                {
                    if (g.GetComponent<CellProperty>().isPlayer)
                    {
                        transform.right = Vector3.left;
                    }
                    g.transform.position = new Vector3(g.transform.position.x - 1, g.transform.position.y, g.transform.position.z);
                    g.GetComponent<CellProperty>().currentCol--;
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) && currentRow + 1 < GridMaker.instance.Rows && !GridMaker.instance.IsStop(currentRow + 1, currentCol, Vector2.up))
            {
                movingObject.Add(this.gameObject);

                for (int r = currentRow + 1; r < GridMaker.instance.Rows - 1; r++)
                {
                    if (GridMaker.instance.IsTherePushableObjectAt(r, currentCol))
                    {
                        movingObject.Add(GridMaker.instance.GetPushableObjectAt(r, currentCol));
                    }
                    else
                    {
                        break;
                    }
                }
                foreach (GameObject g in movingObject)
                {
                    if (g.GetComponent<CellProperty>().isPlayer)
                    {
                        transform.right = Vector3.up;
                    }
                    g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y + 1, g.transform.position.z);
                    g.GetComponent<CellProperty>().currentRow++;
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && currentRow - 1 >= 0 && !GridMaker.instance.IsStop(currentRow - 1, currentCol, Vector2.down))
            {
                movingObject.Add(this.gameObject);

                for (int r = currentRow - 1; r >= 0; r--)
                {
                    if (GridMaker.instance.IsTherePushableObjectAt(r, currentCol))
                    {
                        movingObject.Add(GridMaker.instance.GetPushableObjectAt(r, currentCol));
                    }
                    else
                    {
                        break;
                    }
                }
                foreach (GameObject g in movingObject)
                {
                    if (g.GetComponent<CellProperty>().isPlayer)
                    {
                        transform.right = Vector3.down;
                    }
                    g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y - 1, g.transform.position.z);
                    g.GetComponent<CellProperty>().currentRow--;
                }
            }
        }
        if (isEqual)
        {
            List<GameObject> equationPreEqual = new List<GameObject>();
            List<GameObject> equationPostEqual = new List<GameObject>();
            for (int c =currentCol-1; c>=0; c--)
            {
                if (GridMaker.instance.IsTherePushableObjectAt(currentRow, c))
                {
                    equationPreEqual.Add(GridMaker.instance.GetPushableObjectAt(currentRow, c));
                }
                else
                {
                    break;
                }
            }
            for (int c = currentCol+1; c <=GridMaker.instance.Cols-1; c++)
            {
                if(GridMaker.instance.IsTherePushableObjectAt(currentRow, c))
                {
                    equationPostEqual.Add(GridMaker.instance.GetPushableObjectAt(currentRow, c));
                }
                else
                {
                    break;
                }
            }
            if (equationPreEqual.Count == 0)
            {
                equationPreEqual = new List<GameObject>();
                for (int r = currentRow-1; r >= 0; r--)
                {
                    if (GridMaker.instance.IsTherePushableObjectAt(r, currentCol))
                    {
                        equationPreEqual.Add(GridMaker.instance.GetPushableObjectAt(r, currentCol));
                    }
                    else
                    {
                        break;
                    }
                }
                //List<GameObject> aux = new List<GameObject>();
                //for(int j = equationPreEqual.Count-1; j >= 0; j--)
                //{
                //    aux.Add(equationPreEqual[j]);
                //}
                //equationPreEqual = aux;

                for (int r = currentRow + 1; r <= GridMaker.instance.Rows - 1; r++)
                {
                    if (GridMaker.instance.IsTherePushableObjectAt(r, currentCol))
                    {
                        equationPostEqual.Add(GridMaker.instance.GetPushableObjectAt(r, currentCol));
                    }
                    else
                    {
                        break;
                    }
                }
                equationPostEqual.Reverse();
            }
            if (equationPreEqual.Count >= 1)
            {
                string equation = "";
                for (int i =equationPreEqual.Count-1; i >=0; i--)
                {
                    equation += GridMaker.instance.elementValues[equationPreEqual[i].GetComponent<CellProperty>().element];
                }
                equation += "=";
                for(int j = 0; j< equationPostEqual.Count; j++)
                {
                    equation += GridMaker.instance.elementValues[equationPostEqual[j].GetComponent<CellProperty>().element];
                }
                string[] equationParts = equation.Split('=');
                if(equationParts[0].Length>0 && equationParts[1].Length > 0)
                { 
                    Expression ex = new Expression(equationParts[0]);
                    Expression ex2 = new Expression(equationParts[1]);
                    if (ex.HasErrors())
                    {
                        Debug.Log("Erro na equação");
                    }
                    else
                    {
                        if (Convert.ToInt32(ex.Evaluate()) == Convert.ToInt32(ex2.Evaluate()))
                        {
                            Debug.Log("Player Won!");
                            GridMaker.instance.LoadLevelCompleteBox();
                        }
                    }
                }
            }
        }
    }
}
