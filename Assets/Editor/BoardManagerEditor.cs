using AnnulusGames.LucidTools.RandomKit;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Utilities;

[CustomEditor(typeof(BoardManager))]
public class BoardManagerEditor : Editor
{
    private void OnValidate()
    {
        AddPrefab();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Add Prefab To List"))
        {
            AddPrefab();
        }
        if (GUILayout.Button("Initialization BoardManager"))
        {
            Initialization();
        }
    }

    public void Initialization()
    {
        BoardManager myScript = (BoardManager)target;
        string BoardTilePrefabPath = "Assets/Prefab/BoardTile.prefab";
        string BlockTilePrefabPath = "Assets/Prefab/Block tile.prefab";
        GameObject BoardTilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BoardTilePrefabPath);
        GameObject BlockTilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BlockTilePrefabPath);
        if (BoardTilePrefab != null)
        {
            myScript.boardTilePrefab = BoardTilePrefab;
        }
        if (BlockTilePrefab != null)
        {
            myScript.blockTilePrefab = BlockTilePrefab;
        }
        AddPrefab();
        myScript.boardColor = Color.white;
        myScript.highlightColor = Color.white;
        string darkTitleBgPath = "Assets/Sprites/dark.png";
        string brightTitleBg = "Assets/Sprites/bright.png";
        Sprite darkSprite = AssetDatabase.LoadAssetAtPath<Sprite>(darkTitleBgPath);
        Sprite brightSprite = AssetDatabase.LoadAssetAtPath<Sprite>(brightTitleBg);
        if (darkSprite != null)
        {
            myScript.DarkTitleBg = darkSprite;
        }
        if (brightSprite != null)
        {
            myScript.BrightTitleBg = brightSprite;
        }
    }

    public void AddPrefab()
    {
        string blockImagePath = "Assets/Sprites/BlocksImage/blockbg3.png";
        Sprite blockSprite = AssetDatabase.LoadAssetAtPath<Sprite>(blockImagePath);

        BoardManager myScript = (BoardManager)target;
        string rootPath = "Assets/Prefab/Blocks";

        string[] resFiles = AssetDatabase.FindAssets("t:Prefab"/*"t:" + extension*/, new string[] { rootPath });
        DebugHelper.Log(string.Format("The number of blocks in the folder is {0}", resFiles.Length));

        WeightedList<GameObject> weightedListItems = myScript.blockPrefabs;

        myScript.blockPrefabs = new WeightedList<GameObject>();
        for (int i = 0; i < resFiles.Length; i++)
        {
            //GUID
            resFiles[i] = AssetDatabase.GUIDToAssetPath(resFiles[i]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(resFiles[i]);
            if (prefab != null)
            {
                if (myScript.blockPrefabs.Contains(prefab))
                {
                    //´ý¶¨
                }
                else
                {
                    myScript.blockPrefabs.Add(prefab, 1);
                    prefab.GetComponent<Block>().prefabIndex = i;
                    foreach (Transform item in prefab.transform)
                    {
                        if (item.GetComponent<SpriteRenderer>())
                        {
                            item.GetComponent<SpriteRenderer>().color = Color.white;
                            item.GetComponent<SpriteRenderer>().sprite = blockSprite;
                        }
                    }
                }
            }
            else
            {
                DebugHelper.LogFormatError("prefab {0} is no exist", resFiles[i]);
            }
        }
        GameObject[] blockObjects = new GameObject[myScript.blockPrefabs.Count];
        for (int i = 0; i < blockObjects.Length; i++)
        {
            blockObjects[i] = myScript.blockPrefabs[i].value;
        }
        GameObject[] wegObjects = new GameObject[weightedListItems.Count];
        for (int i = 0; i < wegObjects.Length; i++)
        {
            wegObjects[i] = weightedListItems[i].value;
        }

        for (int k = 0; k < blockObjects.Length; k++)
        {
            if (wegObjects.Contains(blockObjects[k]))
            {
                int index = wegObjects.IndexOf(blockObjects[k]);
                myScript.blockPrefabs[k].weight = weightedListItems[index].weight;
            }
        }
        AssetDatabase.Refresh();
    }
}
