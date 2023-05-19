using System;
using UnityEngine;

//��ʾ����
[AttributeUsage(AttributeTargets.Field)]
public class InspectorShow : PropertyAttribute
{
    public string label;        //Ҫ��ʾ���ַ�
    public InspectorShow(string label)
    {
        this.label = label;
    }

}


//��ʾ��ɫ
public class TitleAttribute : PropertyAttribute
{
    /// <summary> �������� </summary>
    public string title;
    /// <summary> ������ɫ </summary>
    public string htmlColor;

    /// <summary> �������Ϸ����һ������ </summary>
    /// <param name="title">��������</param>
    /// <param name="htmlColor">������ɫ</param>
    public TitleAttribute(string title, string htmlColor = "#FFFFFF")
    {
        this.title = title;
        this.htmlColor = htmlColor;
    }

}


//��ʾö������
[AttributeUsage(AttributeTargets.Field)]
public class EnumNameAttribute : PropertyAttribute
{
    /// <summary> ö������ </summary>
    public string name;
    public new int[] order = new int[0];

    public EnumNameAttribute(string name)
    {
        this.name = name;
    }

    public EnumNameAttribute(string label, params int[] order)
    {
        this.name = label;
        this.order = order;
    }

}
