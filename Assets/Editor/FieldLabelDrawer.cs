using System.Reflection;
using System.Text.RegularExpressions;
using System;
using UnityEditor;
using UnityEngine;


//����������ʾ������
[CustomPropertyDrawer(typeof(InspectorShow))]
public class FieldLabelDrawer : PropertyDrawer
{
    private InspectorShow FLAttribute
    {
        get { return (InspectorShow)attribute; }
        //��ȡ����Ҫ���Ƶ��ֶ�
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //���������»���
        EditorGUI.PropertyField(position, property, new GUIContent(FLAttribute.label), true);

    }
}



//��ʾ��ɫ
[CustomPropertyDrawer(typeof(TitleAttribute))]
public class TitleAttributeDrawer : DecoratorDrawer
{
    // �ı���ʽ
    private GUIStyle style = new GUIStyle();

    public override void OnGUI(Rect position)
    {
        // ��ȡAttribute
        TitleAttribute attr = (TitleAttribute)attribute;

        // ת����ɫ
        Color color = htmlToColor(attr.htmlColor);

        // �ػ�GUI
        position = EditorGUI.IndentedRect(position);
        style.normal.textColor = color;
        GUI.Label(position, attr.title, style);
    }

    public override float GetHeight()
    {
        return base.GetHeight() - 4;
    }

    /// <summary> Html��ɫת��ΪColor </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    private Color htmlToColor(string hex)
    {
        // Ĭ�Ϻ�ɫ
        if (string.IsNullOrEmpty(hex)) return Color.black;

        // ת����ɫ
        hex = hex.ToLower();
        if (hex.IndexOf("#") == 0 && hex.Length == 7)
        {
            int r = Convert.ToInt32(hex.Substring(1, 2), 16);
            int g = Convert.ToInt32(hex.Substring(3, 2), 16);
            int b = Convert.ToInt32(hex.Substring(5, 2), 16);
            return new Color(r / 255f, g / 255f, b / 255f);
        }
        else if (hex == "red")
        {
            return Color.red;
        }
        else if (hex == "green")
        {
            return Color.green;
        }
        else if (hex == "blue")
        {
            return Color.blue;
        }
        else if (hex == "yellow")
        {
            return Color.yellow;
        }
        else if (hex == "black")
        {
            return Color.black;
        }
        else if (hex == "white")
        {
            return Color.white;
        }
        else if (hex == "cyan")
        {
            return Color.cyan;
        }
        else if (hex == "gray")
        {
            return Color.gray;
        }
        else if (hex == "grey")
        {
            return Color.grey;
        }
        else if (hex == "magenta")
        {
            return Color.magenta;
        }
        else
        {
            return Color.black;
        }
    }

}

//��ʾ����ö��
[CustomPropertyDrawer(typeof(EnumNameAttribute))]
public class EnumNameDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // �滻��������
        EnumNameAttribute rename = (EnumNameAttribute)attribute;
        label.text = rename.name;

        // �ػ�GUI
        bool isElement = Regex.IsMatch(property.displayName, "Element \\d+");
        if (isElement) label.text = property.displayName;
        if (property.propertyType == SerializedPropertyType.Enum)
        {
            drawEnum(position, property, label);
        }
        else
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    // ����ö������
    private void drawEnum(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginChangeCheck();

        // ��ȡö���������
        Type type = fieldInfo.FieldType;
        string[] names = property.enumNames;
        string[] values = new string[names.Length];
        while (type.IsArray) type = type.GetElementType();

        // ��ȡö������Ӧ������
        for (int i = 0; i < names.Length; i++)
        {
            FieldInfo info = type.GetField(names[i]);
            EnumNameAttribute[] atts = (EnumNameAttribute[])info.GetCustomAttributes(typeof(EnumNameAttribute), false);
            values[i] = atts.Length == 0 ? names[i] : atts[0].name;
        }

        // �ػ�GUI
        int index = EditorGUI.Popup(position, label.text, property.enumValueIndex, values);
        if (EditorGUI.EndChangeCheck() && index != -1) property.enumValueIndex = index;
    }

}
