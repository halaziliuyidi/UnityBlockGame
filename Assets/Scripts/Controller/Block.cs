using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Block : MonoBehaviour
{
    public int prefabIndex;
    public Vector2 size;
    public Vector2Int[] structure;

    [HideInInspector]
    public Sprite defaultSprite;
    [HideInInspector]
    public Sprite defaultFoodSprite;

    //[HideInInspector]
    public bool movable = true;
    [HideInInspector]
    public int posIndex;
    [HideInInspector]
    public Vector3 basePosition;
    [HideInInspector]
    public Vector3 baseScale;
    [HideInInspector]
    public Vector3 scaledScale;
    [HideInInspector]
    public List<BlockTile> childBlockTitles;

    private bool isHaveFake;

    public void Init(bool isFirstGame = false, int spriteIndex = 0)
    {
        baseScale = BoardManager.Instance.boardTileScale;
        scaledScale = BoardManager.Instance.scaledBlockTileScale;
        ScaleTiles(scaledScale);
        childBlockTitles = new List<BlockTile>(transform.childCount) { };
        for (int i = 0; i < transform.childCount; i++)
        {
            BlockTile blockTitle = transform.GetChild(i).GetComponent<BlockTile>();
            if (blockTitle)
            {
                blockTitle.GetSpriteRender();
                childBlockTitles.Add(blockTitle);
            }
            else
            {
                isHaveFake = true;
            }
        }
        if (isFirstGame)
        {

            for (int i = 0; i < size.y; i++)
            {
                Sprite[] sprites = GameConstManager.Instance.GetFirstGameSprite(i);
                defaultSprite = sprites[0];
                defaultFoodSprite = sprites[1];
                for (int j = 0; j < childBlockTitles.Count; j++)
                {
                    int index;
                    if (isHaveFake)
                    {
                        index = structure[j + 1].y;
                    }
                    else
                    {
                        index = structure[j].y;
                    }
                    if (index == i)
                    {
                        childBlockTitles[j].spriteRenderer.color = GameConstManager.Instance.BlockColor;
                        childBlockTitles[j].SetDefalutColor(defaultSprite);
                    }
                }
            }
        }
        else
        {
            Sprite[] sprites = new Sprite[2];
            if (GameManager.Instance.nowLevel.isStarLevel)
            {
                sprites = GameConstManager.Instance.GetBlockClearSprites(spriteIndex);
            }
            else
            {
                sprites = GameConstManager.Instance.GetBlockSprites(spriteIndex);
            }
            defaultSprite = sprites[0];
            defaultFoodSprite = sprites[1];
            foreach (BlockTile bt in childBlockTitles)
            {
                bt.spriteRenderer.color = GameConstManager.Instance.BlockColor;
                bt.SetDefalutColor(defaultSprite);
            }
            if (GameManager.Instance.nowLevel.isStarLevel)
            {
                if (Random.Range(0, 200) >50)
                {
                    int index = Random.Range(0, childBlockTitles.Count);
                    int starIndex = GameManager.Instance.nowLevel.RandomGetStarIndex();
                    GameManager.Instance.nowLevel.sceneStarsNum[starIndex] += 1;
                    if (GameManager.Instance.nowLevel.IsEnough(starIndex))
                    {
                        return;
                    }
                    childBlockTitles[index].Initialized(true,starIndex);
                }
            }
        }

    }

    public void SetBasePosition(int i, bool cp = true)
    {
        Vector2 scale = transform.localScale;
        Vector2 colliderSize = GetComponent<BoxCollider>().size * scale;
        float distanceX = 0.8f;
        Vector3 position = new Vector3(colliderSize.x / 2 - 0.5f - 1 + (1 - distanceX) * 3 + colliderSize.x * i * distanceX, GameScaler.GetBlockY() + GameConstManager.Instance.BlockOffsetY, 0);

        basePosition = position;

        if (cp)
            transform.position = basePosition;

        posIndex = i;

    }

    public void Scale(bool s, float t)
    {
        GetComponent<BlockScaleAnimation>().enabled = true;
        GetComponent<BlockScaleAnimation>().SetAnimation(s, t);
    }

    public void Layer(bool s)
    {
        for (int i = 0; i < childBlockTitles.Count; i++)
        {
            int ord= s ? 12: 5;
            childBlockTitles[i].spriteRenderer.sortingOrder = ord;
            if (ord == 5)
            {
                childBlockTitles[i].SetStarOrder(6);
            }
            else if (ord == 12)
            {
                childBlockTitles[i].SetStarOrder(13);
            }
        }
    }

    public void ScaleTiles(Vector3 s)
    {
        foreach (Transform t in transform)
            t.localScale = s;
    }

    public void ChangeSprite(Sprite c)
    {
        foreach (Transform t in transform)
            if (t.name == "Block tile")
                t.GetComponent<SpriteRenderer>().sprite = c;
    }

    public Sprite GetSprite()
    {
        return defaultSprite;

    }

    public bool IsMoving()
    {
        return GetComponent<BlockMovingAnimation>().enabled;
    }

    public void Move(float t, Vector3 d)
    {
        GetComponent<BlockMovingAnimation>().enabled = true;
        GetComponent<BlockMovingAnimation>().SetAnimation(t, d);
    }

    public Vector2Int GetFirstCoords()
    {
        Vector3 p;
        p = transform.GetChild(0).transform.position;
        return new Vector2Int((int)(p.x + 0.5f), (int)(p.y + 0.5f));
    }

    public void DestroyObject()
    {
        DestroyImmediate(gameObject, true);
    }

    public int GetBaseScore()
    {
        return childBlockTitles.Count;
    }
}
