using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTile : MonoBehaviour
{
    public Sprite defaultSprite;

    public SpriteRenderer spriteRenderer;

    public void Init()
    {
        spriteRenderer= gameObject.GetComponent<SpriteRenderer>();
    }

    public void SetDefaultSprite(Sprite sprite)
    {
        defaultSprite= sprite;
        spriteRenderer.sprite = sprite;
    }

    public void ResetSprite()
    {
        spriteRenderer.sprite=defaultSprite;
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void HightLight(Color color,Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = color;
    }

    public void RemoveHightLight()
    {
        spriteRenderer.sprite = defaultSprite;
        spriteRenderer.color = BoardManager.Instance.boardColor;
    }
}
