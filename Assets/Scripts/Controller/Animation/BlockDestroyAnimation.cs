using UnityEngine;

public class BlockDestroyAnimation : MonoBehaviour
{
    public Sprite destroyedBlock;
    public Vector3 rotation = new Vector3(0, 0, 360);
    public Vector3 scale = new Vector3(0, 0, 0);
    public Color color = new Color(1, 1, 1, 1);

    private float duration;
    private float fraction;

    public void SetAnimation(float t)
    {
        duration = t;
        fraction = 0;
    }

    private void Update()
    {
        if (fraction >= 1)
        {
            if (GameManager.Instance.nowLevel.isStarLevel)
            {
                BlockTile blockTile = GetComponent<BlockTile>();
                if (blockTile&&blockTile.isStar)
                {
                    GameManager.Instance.fxController.SpawnStar(blockTile.starIndex, GameConstManager.Instance.StarSprites[blockTile.starIndex],transform.position, GameManager.Instance.GetStarPos(blockTile.starIndex));
                    GameManager.Instance.nowLevel.SubtractStar(blockTile.starIndex);
                    GameManager.Instance.nowLevel.sceneStarsNum[blockTile.starIndex] -= 1;
                    DebugHelper.Log("减索引为:"+blockTile.starIndex+"的分数");
                }
            }
            else
            {
                GameManager.Instance.fxController.SpawnFood(transform.parent.GetComponent<Block>().defaultFoodSprite, transform.position);
            }
            if (transform.parent.childCount > 0)
                Destroy(gameObject);
            else
                Destroy(transform.parent.gameObject);
        }

        fraction += Time.deltaTime / duration;

        transform.eulerAngles = Vector3.Lerp(new Vector3(0, 0, 0), rotation, fraction);
        transform.localScale = Vector3.Lerp(new Vector3(1, 1, 1), scale, fraction);
    }
}
