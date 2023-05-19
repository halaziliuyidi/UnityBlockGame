using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameDataManager))]
public class GameDataManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Clear All GameData"))
        {
            ClearAllGameData();
        }

        if (GUILayout.Button("Clear Level Data"))
        {
            ClearLevelData();
        }
    }

    private void ClearAllGameData()
    {
        GameDataManager gameDataManager= (GameDataManager)target;
        gameDataManager.ClearAllGameData();
        DebugHelper.Log("<color=#FF0E0E>Clear All Game Data</color>");
    }

    private void ClearLevelData()
    {
        GameDataManager gameDataManager = (GameDataManager)target;
        gameDataManager.ClearLevelData();
        DebugHelper.Log("<color=#FF0E0E>Clear Level Data</color>");
    }
}
