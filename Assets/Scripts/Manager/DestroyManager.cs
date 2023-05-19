using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyManager : MonoBehaviour
{

    private static DestroyManager instance;

    [Serializable]
    public class line
    {
        public int xIndex;
        public int yIndex;
        public line(int x = 0, int y = 0)
        {
            yIndex = y;
            xIndex = x;
        }
    }

    public List<line> VDestoryLine = new List<line>();

    public List<line> HDestroyLine = new List<line>();

    public static DestroyManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(DestroyManager)) as DestroyManager;
            }
            return instance;
        }
    }

    [HideInInspector]
    public int destroyedLines;
    [HideInInspector]
    private BlockDestroyAnimation[,] blocksAnimations = new BlockDestroyAnimation[GameConstManager.Board_Size, GameConstManager.Board_Size];

    private Vector2Int[] desLinesPos = new Vector2Int[GameConstManager.Board_Size];

    public void Initialized()
    {
        if (!instance)
        {
            instance = this;
        }

    }

    public void SetDestroy()
    {
        destroyedLines = 0;
        for (int i = 0; i < GameConstManager.Board_Size; i++)
            desLinesPos[i] = new Vector2Int(-1, -1);
    }

    public void PrepareToDestroy(int i, bool v)
    {
        Debug.Log("PrepareToDestroy");
        if (v)
        {
            for (int y = 0; y < GameConstManager.Board_Size; y++)
            {
                blocksAnimations[i, y] = BoardManager.Instance.boardBlocks[i, y].GetComponent<BlockDestroyAnimation>();
                VDestoryLine.Add(new line(i, y));
            }
            Vector3 vector2 = DestroyPosition(GetDestoryVPoint(VDestoryLine));
            EventManager.TriggerEvent(GameConstManager.destroyLine, vector2,"v");
        }
        else
        {
            for (int x = 0; x < GameConstManager.Board_Size; x++)
            {
                blocksAnimations[x, i] = BoardManager.Instance.boardBlocks[x, i].GetComponent<BlockDestroyAnimation>();
                HDestroyLine.Add(new line(x, i));
            }
            Vector3 vector2 = DestroyPosition(GetDestoryHPoint(HDestroyLine));
            EventManager.TriggerEvent(GameConstManager.destroyLine,vector2,"h");
        }
        destroyedLines++;
        for (int j = 0; j < GameConstManager.Board_Size; j++)
        {
            if (desLinesPos[j] == new Vector2Int(-1, -1))
            {
                desLinesPos[j] = v ? new Vector2Int(i, -1) : new Vector2Int(-1, i);
                break;
            }
        }
    }

    public IEnumerator DestroyAllBlocks()
    {
        for (int i = 0; i < GameConstManager.Board_Size; i++)
        {
            for (int j = 0; j < GameConstManager.Board_Size; j++)
            {
                if (desLinesPos[j] == new Vector2Int(-1, -1))
                    break;

                int y = GameConstManager.Board_Size - i - 1;
                Vector2Int p = desLinesPos[j];
                if (p.x != -1 && blocksAnimations[p.x, y] && !blocksAnimations[p.x, y].enabled)
                    BoardManager.Instance.boardBlocks[p.x, y] = null;
                else if (p.y != -1 && blocksAnimations[i, p.y] && !blocksAnimations[i, p.y].enabled)
                    BoardManager.Instance.boardBlocks[i, p.y] = null;
            }
        }

        //GameManager.instance.ChangePoints(boardManager.GetEmptyFieldsAmount(), destroyedLines);
        BoardManager.Instance.CheckSpace();

        for (int i = 0; i < GameConstManager.Board_Size; i++)
        {
            for (int j = 0; j < GameConstManager.Board_Size; j++)
            {
                if (desLinesPos[j] == new Vector2Int(-1, -1))
                    break;

                int y = GameConstManager.Board_Size - i - 1;
                Vector2Int p = desLinesPos[j];
                if (p.x != -1 && blocksAnimations[p.x, y] && !blocksAnimations[p.x, y].enabled)
                {
                    blocksAnimations[p.x, y].enabled = true;
                    blocksAnimations[p.x, y].SetAnimation(0.25f);
                    blocksAnimations[p.x, y] = null;

                }
                else if (p.y != -1 && blocksAnimations[i, p.y] && !blocksAnimations[i, p.y].enabled)
                {
                    blocksAnimations[i, p.y].enabled = true;
                    blocksAnimations[i, p.y].SetAnimation(0.25f);
                    blocksAnimations[i, p.y] = null;


                }
            }
            yield return new WaitForSeconds(0.025f);
        }

    }

    public Vector2 GetDestoryVPoint(List<line> lines)
    {
        Vector2 vector2 = new Vector2();
        int destroyLine = lines.Count / 8;
        for (int i = 0; i < destroyLine; i++)
        {
            int index = ((i + 1) * 8) - 1;
            vector2.x = lines[index].xIndex;
            vector2.y = lines[index].yIndex;
            DebugHelper.LogFormat("消除了竖行位置为({0},{1})", vector2.x, vector2.y);
        }
        lines.Clear();
        return vector2;
    }

    public Vector2 GetDestoryHPoint(List<line> lines)
    {
        Vector2 vector2 = new Vector2();
        int destroyLine = lines.Count / 8;
        for (int i = 0; i < destroyLine; i++)
        {
            vector2.x = lines[i * 8].xIndex;
            vector2.y = lines[i * 8].yIndex;
            DebugHelper.LogFormat("消除了横行位置为({0},{1})", vector2.x, vector2.y);
        }
        lines.Clear();
        return vector2;
    }

    public Vector2 DestroyPosition(Vector2 vector2)
    {
        return BoardManager.Instance.boardBlocks[(int)vector2.x, (int)vector2.y].transform.position;
    }
}
