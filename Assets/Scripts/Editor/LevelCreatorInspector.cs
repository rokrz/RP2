using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelCreator))]
[ExecuteInEditMode]
public class LevelCreatorInspector : Editor
{
    Dictionary<ElementTypes, Texture> textureHolder = new Dictionary<ElementTypes, Texture>();
    private void OnEnable()
    {
        textureHolder.Add(ElementTypes.Robozin, (Texture)EditorGUIUtility.Load("Assets/Resources/Sprites/Robozin.png"));
        textureHolder.Add(ElementTypes.Bloco1, (Texture)EditorGUIUtility.Load("Assets/Resources/Sprites/Bloco1_Idle.png"));
        textureHolder.Add(ElementTypes.Bloco2, (Texture)EditorGUIUtility.Load("Assets/Resources/Sprites/Bloco2_Idle.png"));
        textureHolder.Add(ElementTypes.Bloco3, (Texture)EditorGUIUtility.Load("Assets/Resources/Sprites/Bloco3_Idle.png"));
        textureHolder.Add(ElementTypes.Bloco4, (Texture)EditorGUIUtility.Load("Assets/Resources/Sprites/Bloco4_Idle.png"));
        textureHolder.Add(ElementTypes.Bloco5, (Texture)EditorGUIUtility.Load("Assets/Resources/Sprites/Bloco5_Idle.png"));
        textureHolder.Add(ElementTypes.Bloco6, (Texture)EditorGUIUtility.Load("Assets/Resources/Sprites/Bloco6_Idle.png"));
        textureHolder.Add(ElementTypes.Bloco7, (Texture)EditorGUIUtility.Load("Assets/Resources/Sprites/Bloco7_Idle.png"));
        textureHolder.Add(ElementTypes.Bloco8, (Texture)EditorGUIUtility.Load("Assets/Resources/Sprites/Bloco8_Idle.png"));
        textureHolder.Add(ElementTypes.Bloco9, (Texture)EditorGUIUtility.Load("Assets/Resources/Sprites/Bloco9_Idle.png"));
        textureHolder.Add(ElementTypes.Bloco0, (Texture)EditorGUIUtility.Load("Assets/Resources/Sprites/Bloco0_Idle.png"));
        textureHolder.Add(ElementTypes.BlocoSoma, (Texture)EditorGUIUtility.Load("Assets/Resources/Sprites/BlocoSoma_Idle.png"));
        textureHolder.Add(ElementTypes.BlocoSubtrai, (Texture)EditorGUIUtility.Load("Assets/Resources/Sprites/BlocoSubtrai_Idle.png"));
        textureHolder.Add(ElementTypes.BlocoDivide, (Texture)EditorGUIUtility.Load("Assets/Resources/Sprites/BlocoDivide_Idle.png"));
        textureHolder.Add(ElementTypes.BlocoMultiplica, (Texture)EditorGUIUtility.Load("Assets/Resources/Sprites/BlocoMultiplica_Idle.png"));
        textureHolder.Add(ElementTypes.BlocoIgual, (Texture)EditorGUIUtility.Load("Assets/Resources/Sprites/BlocoIgual_Idle.png"));
        textureHolder.Add(ElementTypes.BlocoAbreP, (Texture)EditorGUIUtility.Load("Assets/Resources/Sprites/BlocoAbreP_Idle.png"));
        textureHolder.Add(ElementTypes.BlocoFechaP, (Texture)EditorGUIUtility.Load("Assets/Resources/Sprites/BlocoFechaP_Idle.png"));
        textureHolder.Add(ElementTypes.Empty, (Texture)EditorGUIUtility.Load("Assets/Resources/Sprites/Empty.png"));
        textureHolder.Add(ElementTypes.Wall, (Texture)EditorGUIUtility.Load("Assets/Resources/Sprites/wall_2.png"));
    }
    ElementTypes currentSelected = ElementTypes.Empty;
    public override void OnInspectorGUI()
    {
        // emptyTexture = (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/empty.png");

        base.OnInspectorGUI();
        GUILayout.Label("Current Selected : " + currentSelected.ToString());

        LevelCreator levelCreator = (LevelCreator)target;
        int rows = (int)Mathf.Sqrt(levelCreator.level.Count);
        //int currentI = levelCreator.level.Count-1;
        GUILayout.BeginVertical();
        for (int r = rows - 1; r >= 0; r--)
        {
            GUILayout.BeginHorizontal();
            for (int c = 0; c < rows; c++)
            {
                if (GUILayout.Button(textureHolder[levelCreator.level[c + ((rows) * r)]], GUILayout.Width(50), GUILayout.Height(50)))
                {
                    //Adiciona o valor ao Nivel
                    //if (valuesHolder.Contains(currentSelected.ToString()))
                    //{
                    //    levelCreator.levelValues[c + ((rows) * r)] = ;
                    //}
                    levelCreator.level[c + ((rows) * r)] = currentSelected;
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        GUILayout.Space(20);
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        int count = 0;
        foreach (KeyValuePair<ElementTypes, Texture> e in textureHolder)
        {
            count++;
            if (GUILayout.Button(e.Value, GUILayout.Width(50), GUILayout.Height(50)))
            {
                currentSelected = e.Key;
            }
            if (count % 4 == 0)
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.Space(20);
        GUILayout.BeginVertical();
        if (GUILayout.Button("Save Level", GUILayout.Width(100), GUILayout.Height(50)))
        {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }
        GUILayout.EndVertical();

    }
}
