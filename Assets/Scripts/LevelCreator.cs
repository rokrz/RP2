using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementTypes
{
    Empty = 0,
    Robozin = 1,
    Wall,

    Bloco0,
    Bloco1,
    Bloco2,
    Bloco3,
    Bloco4,
    Bloco5,
    Bloco6,
    Bloco7,
    Bloco8,
    Bloco9,
    BlocoSubtrai,
    BlocoDivide,
    BlocoMultiplica,
    BlocoSoma,
    BlocoIgual,
    BlocoAbreP,
    BlocoFechaP,
}

[CreateAssetMenu()]
[System.Serializable]
public class LevelCreator : ScriptableObject
{

    [SerializeField]
    public List<ElementTypes> level = new List<ElementTypes>();
    public LevelCreator()
    {
        level = new List<ElementTypes>();
    }
}