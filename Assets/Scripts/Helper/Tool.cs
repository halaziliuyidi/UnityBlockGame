using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public static class Tool
{
    private static readonly string digit = "<sprite={0}>";

    private static readonly StringBuilder sb = new StringBuilder();
    public static string GetText(int num)
    {
        sb.Clear();

        string numString = PrintDigits(num);

        for (int i = 0; i < numString.Length; i++)
        {
            sb.AppendFormat(digit, numString[i]);
        }
        return sb.ToString();
    }

    private static string PrintDigits(int num)
    {
        if (num == 0)
        {
            return "0";
        }

        int numDigits = (int)Mathf.Log10(num) + 1;

        char[] digitsArray = new char[numDigits];

        for (int i = numDigits - 1; i >= 0; i--)
        {
            digitsArray[i] = (char)((num % 10) + '0');
            num /= 10;
        }

        string digitsString = new string(digitsArray);
        List<int> ii = new List<int>();
        return digitsString;
    }

    public static Vector2 WorldToUIPoint(Vector3 worldPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Vector2 uiPos = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height) * 2 - new Vector2(1, 1);
        return uiPos;
    }

}

public static class ArrayExtensions
{
    public static int IndexOf<T>(this T[] array, T value)
    {
        return Array.IndexOf(array, value);
    }

    public static bool Contains<T>(this T[] array, T value)
    {
        if (array == null)
        {
            return false;
        }
        for (int i = 0; i < array.Length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(array[i], value))
            {
                return true;
            }
        }
        return false;

    }
}

public static class HorizontalLayoutGroupExtensions
{
    public static Vector3[] GetChildPos(this Transform trn)
    {
        int childCount = trn.transform.childCount;
        Vector3[] vector3s = new Vector3[childCount];

        for (int i = 0; i < childCount; i++)
        {
            vector3s[i] = trn.transform.GetChild(i).transform.position;
        }
        return vector3s;
    }

    public static Vector3 GetUIToWordPos(GameObject uiObj)
    {
        Vector3 ptScreen = RectTransformUtility.WorldToScreenPoint(Camera.main, uiObj.transform.position);
        ptScreen.z = 0;
        Vector3 ptWorld = Camera.main.ScreenToWorldPoint(ptScreen);
        return ptWorld;
    }
}
