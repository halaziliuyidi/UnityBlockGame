using UnityEngine;
using System.Collections;
using TMPro;
using System;

public class ComboManager : MonoBehaviour
{
    private static ComboManager instance;
    public static ComboManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(ComboManager)) as ComboManager;
            }
            return instance;
        }
    }

    private float comboTimeLimit; // 连击时间限制

    private int currentCombo = 0; // 当前连击次数

    private bool isCombo = false; // 是否处于连击状态

    private float timeElapsed = 0f;

    private Coroutine combCor;

    public GameObject comboFX;

    public void Initialized()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    public int CheckCombo()
    {
        currentCombo++;
        int comboNum = 0;
        int length = GameConstManager.Instance.comboTimeLimits.Length;
        if (currentCombo >= length)
        {
            comboTimeLimit = GameConstManager.Instance.comboTimeLimits[length - 1];
        }
        else
        {
            comboTimeLimit = GameConstManager.Instance.comboTimeLimits[currentCombo];
        }
        if (currentCombo > 1)
        {
            //开始连击，连击次数为currenCombo-1
            comboNum = currentCombo - 1;
            comboFX.gameObject.SetActive(true);
        }
        isCombo = true;
        StartCombIE();
        return comboNum;
    }

    private void EndCombo()
    {
        comboFX.gameObject.SetActive(false);
        isCombo = false;
        currentCombo = 0;
        EndComboIE();
    }

    private IEnumerator IEComb()
    {
        if (isCombo)
        {
            timeElapsed = 0;
            while (timeElapsed <= comboTimeLimit)
            {
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            EndCombo();
        }
    }

    private void StartCombIE()
    {
        if (combCor != null)
        {
            StopCoroutine(combCor);
        }
        combCor = StartCoroutine(IEComb());
    }

    private void EndComboIE()
    {
        if (combCor != null)
        {
            StopCoroutine(combCor);
        }
        combCor = null;
    }

    public void ResetCombo()
    {
        EndCombo();
    }
}
