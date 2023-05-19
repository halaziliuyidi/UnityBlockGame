using UnityEngine;
using UnityEngine.UI;
using FancyScrollView;
using System;
using System.Collections.Generic;


class LevelItem : FancyScrollRectCell<ItemData, Context>
{
    [SerializeField] Text message = default;

    [SerializeField] List<LevelObject> levelObjects;

    private void Awake()
    {
        levelObjects = new List<LevelObject>();
        foreach (Transform levelTrn in transform.GetChild(0))
        {
            if (levelTrn.name.Equals("levelObject"))
            {
                LevelObject level = levelTrn.GetComponent<LevelObject>();
                levelObjects.Add(level);
                level.Init();
            }
        }
    }

    public override void UpdateContent(ItemData itemData)
    {
        message.text = Index.ToString();
        for (int i = 0; i < levelObjects.Count; i++)
        {
            int index = (GameConstManager.LevelCount - 1 - Index) * GameConstManager.SinglePageLevelItem + i + 1;
            levelObjects[i].UpdateLevelInfo(index);
        }
    }

    protected override void UpdatePosition(float normalizedPosition, float localPosition)
    {
        base.UpdatePosition(normalizedPosition, localPosition);

        transform.localPosition += new Vector3(0, normalizedPosition + 1784 * 0.5f, 0);
    }
}

