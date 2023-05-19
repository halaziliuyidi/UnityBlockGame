using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTile : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public Sprite defaultSprite;

    public bool isStar;

    public int starIndex = 0;

    public Sprite starSprite;

    GameObject starObject;
    public void Fade(float d, Color c)
    {
        BlockFadeAnimation anim = GetComponent<BlockFadeAnimation>();
        anim.enabled = true;
        anim.SetAnimation(d, c);
    }

    public void FadeSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void Fall(float d, BlockFallAnimation.Direction dir)
    {
        BlockFallAnimation anim = GetComponent<BlockFallAnimation>();
        anim.enabled = true;
        anim.SetAnimation(d, dir);
    }

    public void Destroy(float d)
    {
        BlockDestroyAnimation anim = GetComponent<BlockDestroyAnimation>();
        anim.enabled = true;
        anim.SetAnimation(d);
    }

    public void GetSpriteRender()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (transform.Find("star"))
        {
            starObject = transform.Find("star").gameObject;
        }
    }

    public void SetDefalutColor(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
        defaultSprite = sprite;
    }

    public void SetStarOrder(int ord)
    {
        starObject.GetComponent<SpriteRenderer>().sortingOrder = ord;
    }

    public void Initialized(bool IsStar = false, int StarIndex = 0)
    {
        isStar = IsStar;
        starIndex = StarIndex;
        if (isStar)
        {
            starSprite = GameConstManager.Instance.GetStarSprite(starIndex);
            starObject.GetComponent<SpriteRenderer>().sprite = starSprite;
            starObject.GetComponent<SpriteRenderer>().sortingOrder = 10;
            starObject.SetActive(true);
        }
    }
}
