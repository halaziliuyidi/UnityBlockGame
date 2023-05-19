using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScaler : MonoBehaviour
{
    public const float TABLET_ASPECT_RATIO = 1.4f;
    public const float TABLET_HALF_WIDTH = 6.5f;
    public const float HALF_WIDTH = 5.35f;
    public const float BLOCK_SIZE = 1.0f;
    public const int BOARD_TILE_DISTANCE = 3;
    public const int SCALED_BLOCK_TILE_DISTANCE = 1;
    public const float SCALED_BLOCK_SCALE = 0.6f;

    public static Color[] blockColors;
    public static float GetAspectRatio()
    {
        return (float)Screen.height / Screen.width;
    }

    public static float GetOrthographicSize()
    {
        float ar = GetAspectRatio();
        return ar > TABLET_ASPECT_RATIO ? ar * HALF_WIDTH : ar * TABLET_HALF_WIDTH;
    }

    public static float ScreenToWorld(float y)
    {
        return y / Screen.height * GetOrthographicSize() * 2;
    }

    public static float GetBlockY()
    {
        return (-GetOrthographicSize() + Camera.main.transform.position.y - BLOCK_SIZE / 2) / 2;
    }

    public static Vector3 GetBoardTileScale()
    {
        float scale = BLOCK_SIZE - ScreenToWorld(BOARD_TILE_DISTANCE);
        return new Vector3(scale, scale, scale);
    }

    public static Vector3 GetScaledBlockTileScale()
    {
        float scale = BLOCK_SIZE - ScreenToWorld(SCALED_BLOCK_TILE_DISTANCE) / SCALED_BLOCK_SCALE;
        return new Vector3(scale, scale, scale);
    }
}
