using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BlocksEditor : EditorWindow
{
    string rootPath = "Assets/Prefab/Blocks";

    //[MenuItem("关卡编辑器/修复所有Blocks")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<BlocksEditor>("修复所有Block");

    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        GUILayout.FlexibleSpace(); // 在垂直方向上添加空的可伸缩区域

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace(); // 在水平方向上添加空的可伸缩区域

        if (GUILayout.Button("修复所有Block", GUILayout.Width(200), GUILayout.Height(50)))
        {
            FixAllBlocks();
        }

        GUILayout.FlexibleSpace(); // 在水平方向上添加空的可伸缩区域
        EditorGUILayout.EndHorizontal();

        GUILayout.FlexibleSpace(); // 在垂直方向上添加空的可伸缩区域
        EditorGUILayout.EndVertical();
    }

    private void FixAllBlocks()
    {
        string starPrefabPath = "Assets/Prefab/star.prefab";
        string starImagePath = "Assets/Sprites/Star/star1.png";
        Sprite starSprite = AssetDatabase.LoadAssetAtPath<Sprite>(starImagePath);
        GameObject star = AssetDatabase.LoadAssetAtPath<GameObject>(starPrefabPath);
        string[] resFiles = AssetDatabase.FindAssets("t:Prefab"/*"t:" + extension*/, new string[] { rootPath });
        for (int i = 0; i < resFiles.Length; i++)
        {
            resFiles[i] = AssetDatabase.GUIDToAssetPath(resFiles[i]);
            GameObject prefab = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(resFiles[i]));
            if (prefab != null)
            {
                foreach (Transform item in prefab.transform)
                {
                    if (item.GetComponent<SpriteRenderer>())
                    {
                        foreach (Transform item2 in item.transform)
                        {
                            if (item2.name == "star")
                            {
                                item2.gameObject.SetActive(false);
                            }
                        }
                    }
                }
            }
            prefab.name = prefab.name.Replace("(Clone)","");
        }

        
    }
}
