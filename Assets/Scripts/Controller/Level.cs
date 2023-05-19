using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public bool isStarLevel;

    public int targetScore;

    public int[] startTargetNum=new int[5];

    public int start1Num = 0;

    public int start2Num = 0;

    public int start3Num = 0;

    public int start4Num = 0;

    public int start5Num = 0;

    public int[] nowStarsNum;

    public int[] sceneStarsNum=new int[5];

    public void Awake()
    {
        nowStarsNum = new int[5];
        sceneStarsNum=new int[5];
        for (int i = 0; i < 5; i++)
        {
            nowStarsNum[i] = startTargetNum[i];
        }
    }

    public List<int> GetStarSurplusKinds()
    {
        List<int> result = new List<int>();
        for (int i = 0; i < startTargetNum.Length;i++)
        {
            if (startTargetNum[i] != 0)
            {
                result.Add(i);
            }
        }
        return result;
    }

    public List<int> GetStarKinds()
    {
        List<int> result = new List<int>();
        for (int i = 0; i < nowStarsNum.Length; i++)
        {
            if (nowStarsNum[i] != 0)
            {
                result.Add(i);
            }
        }
        return result;
    }

    public bool IsEnough(int index)
    {
        if (sceneStarsNum[index] < startTargetNum[index])
            return false;
        return true;
    }



    public int RandomGetStarIndex()
    {
        List<int> ints=GetStarSurplusKinds();

        return ints[Random.Range(0, ints.Count)];
    }

    public void SubtractStar(int index)
    {
        startTargetNum[index]--;
        if (startTargetNum[index]<=0)
            startTargetNum[index] = 0;
        GameManager.Instance.SubtractStarNum(index, startTargetNum[index]);
        IsWin();
    }

    public bool IsWin()
    {
        for (int i = 0; i < startTargetNum.Length; i++)
        {
            if (startTargetNum[i] > 0)
            {
                return false;
            }
        }
        GameManager.Instance.GameStarWin();
        return true;
    }
}
