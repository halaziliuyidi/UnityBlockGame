using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;

    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(InputManager)) as InputManager;
            }
            return instance;
        }
    }

    private bool isMouseInput;
    [HideInInspector]
    public Vector3 lastPosition;
    [HideInInspector]
    public Block draggedBlock;

    private ScreenOrientation screenOrientation;

    private Vector3 startPos;
    private Vector2Int lastPos = new Vector2Int(-1, -1);
    private BoardTile[] highlightedTiles = new BoardTile[9];

    private Coroutine InputCor;

    public void Initialized()
    {
#if UNITY_EDITOR
        isMouseInput = true;
#else
isMouseInput = false;
#endif
        if (InputCor != null)
        {
            StopCoroutine(InputCor);
            InputCor = null;
        }
        InputCor = StartCoroutine(IEInput());
    }

    public void ResetBlock()
    {
        if (draggedBlock)
        {
            MoveDraggedBlock();
            ResetDraggedBlock();
        }
    }

    private IEnumerator IEInput()
    {
        while (true)
        {

            if (screenOrientation != Screen.orientation || GameManager.Instance.paused)
            {
                ResetBlock();
            }
            //判断是否是编辑器测试状态
            if (isMouseInput)
            {
                if (Input.GetMouseButtonDown(0) && !GameManager.Instance.paused && !GameManager.Instance.gameOver)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    ShowTouchFx(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit, 15))
                    {
                        screenOrientation = Screen.orientation;

                        Collider c = hit.collider;

                        if (c.tag == "Block" && !c.GetComponent<Block>().IsMoving())
                        {
                            draggedBlock = c.GetComponent<Block>();

                            draggedBlock.Scale(true, 0.2f);
                            draggedBlock.Layer(true);
                            draggedBlock.transform.position += new Vector3(0, 2, 0);
                            Sprite cl = draggedBlock.defaultSprite;
                            draggedBlock.ChangeSprite(cl);

                            Vector3 p = draggedBlock.transform.position;
                            startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            startPos = new Vector3(startPos.x - p.x, startPos.y - p.y, 0);
                            GameManager.Instance.audioController.PlayDragAudio();
                        }
                    }
                }
                //拖拽移动
                if (Input.GetMouseButton(0) && draggedBlock)
                {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    pos = new Vector3(pos.x, pos.y, -2);

                    draggedBlock.transform.position = pos - startPos;

                    Vector3 size = draggedBlock.GetComponent<Block>().size;
                    size = new Vector3(size.x - 1, size.y - 1, 0);

                    Vector2 origin = draggedBlock.transform.GetChild(0).position;
                    Vector2 end = draggedBlock.transform.GetChild(0).position + size;
                    if (IsInRange(origin, end) && IsEmpty(draggedBlock, RoundVector2(origin)))
                    {
                        Vector2Int start = RoundVector2(origin);
                        if (lastPos != start)
                        {
                            RemoveAllHightLights();
                            BoardManager.Instance.HighlightBlocks();
                            for (int i = 0; i < draggedBlock.structure.Length; i++)
                            {
                                if (draggedBlock.transform.GetChild(i).name == "Block tile")
                                {
                                    Vector2Int coords = draggedBlock.structure[i];
                                    highlightedTiles[i] = BoardManager.Instance.boardTiles[start.x + coords.x, start.y + coords.y];
                                    Color cl = GameConstManager.Instance.BlockColor;
                                    cl.a = GameConstManager.Instance.boardDragColor_A;
                                    highlightedTiles[i].HightLight(cl, draggedBlock.defaultSprite);
                                }
                            }
                        }
                        lastPos = start;
                    }
                    else
                    {
                        RemoveAllHighlights();
                        lastPos = new Vector2Int(-1, -1);
                    }
                }
                else if (Input.GetMouseButtonUp(0) && draggedBlock)
                {
                    Vector3 size = draggedBlock.size;
                    size = new Vector3(size.x - 1, size.y - 1, 0);

                    Vector2 origin = draggedBlock.transform.GetChild(0).position;
                    Vector2 end = draggedBlock.transform.GetChild(0).position + size;

                    //在board范围内，可以吸附到board上
                    if (IsInRange(origin, end) && IsEmpty(draggedBlock, RoundVector2(origin)))
                    {
                        lastPosition = BlockPosition(origin, size);

                        draggedBlock.Move(0.08f, lastPosition);
                        draggedBlock.ChangeSprite(draggedBlock.defaultSprite);
                        draggedBlock.GetComponent<BoxCollider>().enabled = false;
                        draggedBlock.enabled = false;

                        Vector2Int start = RoundVector2(origin);

                        for (int i = 0; i < draggedBlock.structure.Length; i++)
                        {
                            Vector2Int coords = draggedBlock.structure[i];

                            if (draggedBlock.transform.GetChild(i).name == "Block tile")
                            {
                                BlockTile b = draggedBlock.transform.GetChild(i).GetComponent<BlockTile>();
                                BoardManager.Instance.boardBlocks[start.x + coords.x, start.y + coords.y] = b;
                            }
                        }
                        BoardManager.Instance.MoveBlocks(draggedBlock);
                        BoardManager.Instance.CheckBoard(draggedBlock.transform);

                    }
                    else
                    {
                        MoveDraggedBlock();
                    }

                    ResetDraggedBlock();
                }
            }
            else
            {
                if (Input.touchCount > 0)
                {
                    Touch t = Input.GetTouch(0);

                    if (t.phase == TouchPhase.Began && !GameManager.Instance.paused && !GameManager.Instance.gameOver)
                    {
                        ShowTouchFx(t.position);
                        Ray ray = Camera.main.ScreenPointToRay(t.position);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, 15))
                        {
                            screenOrientation = Screen.orientation;

                            Collider c = hit.collider;

                            if (c.tag == "Block" && !c.GetComponent<Block>().IsMoving())
                            {
                                draggedBlock = c.GetComponent<Block>();

                                draggedBlock.Scale(true, 0.2f);
                                draggedBlock.Layer(true);
                                draggedBlock.transform.position += new Vector3(0, 2, 0);
                                Sprite cl = draggedBlock.defaultSprite;
                                draggedBlock.ChangeSprite(cl);

                                Vector3 p = draggedBlock.transform.position;
                                startPos = Camera.main.ScreenToWorldPoint(t.position);
                                startPos = new Vector3(startPos.x - p.x, startPos.y - p.y, 0);
                                GameManager.Instance.audioController.PlayDragAudio();
                            }
                        }
                    }
                    //拖拽移动
                    if (t.phase == TouchPhase.Moved && draggedBlock)
                    {
                        Vector3 pos = Camera.main.ScreenToWorldPoint(t.position);
                        pos = new Vector3(pos.x, pos.y, -2);

                        draggedBlock.transform.position = pos - startPos;

                        Vector3 size = draggedBlock.GetComponent<Block>().size;
                        size = new Vector3(size.x - 1, size.y - 1, 0);

                        Vector2 origin = draggedBlock.transform.GetChild(0).position;
                        Vector2 end = draggedBlock.transform.GetChild(0).position + size;
                        if (IsInRange(origin, end) && IsEmpty(draggedBlock, RoundVector2(origin)))
                        {
                            Vector2Int start = RoundVector2(origin);
                            if (lastPos != start)
                            {
                                RemoveAllHightLights();
                                BoardManager.Instance.HighlightBlocks();
                                for (int i = 0; i < draggedBlock.structure.Length; i++)
                                {
                                    if (draggedBlock.transform.GetChild(i).name == "Block tile")
                                    {
                                        Vector2Int coords = draggedBlock.structure[i];
                                        highlightedTiles[i] = BoardManager.Instance.boardTiles[start.x + coords.x, start.y + coords.y];
                                        Color cl = GameConstManager.Instance.BlockColor;
                                        cl.a = GameConstManager.Instance.boardDragColor_A;
                                        highlightedTiles[i].HightLight(cl, draggedBlock.defaultSprite);
                                    }
                                }
                            }
                            lastPos = start;
                        }
                        else
                        {
                            RemoveAllHighlights();
                            lastPos = new Vector2Int(-1, -1);
                        }
                    }
                    else if (t.phase == TouchPhase.Ended && draggedBlock)
                    {
                        Vector3 size = draggedBlock.size;
                        size = new Vector3(size.x - 1, size.y - 1, 0);

                        Vector2 origin = draggedBlock.transform.GetChild(0).position;
                        Vector2 end = draggedBlock.transform.GetChild(0).position + size;

                        //在board范围内，可以吸附到board上
                        if (IsInRange(origin, end) && IsEmpty(draggedBlock, RoundVector2(origin)))
                        {
                            lastPosition = BlockPosition(origin, size);
                            draggedBlock.Move(0.08f, lastPosition);
                            draggedBlock.ChangeSprite(draggedBlock.defaultSprite);
                            draggedBlock.GetComponent<BoxCollider>().enabled = false;
                            draggedBlock.enabled = false;
                            Vector2Int start = RoundVector2(origin);
                            for (int i = 0; i < draggedBlock.structure.Length; i++)
                            {
                                Vector2Int coords = draggedBlock.structure[i];

                                if (draggedBlock.transform.GetChild(i).name == "Block tile")
                                {
                                    BlockTile b = draggedBlock.transform.GetChild(i).GetComponent<BlockTile>();
                                    BoardManager.Instance.boardBlocks[start.x + coords.x, start.y + coords.y] = b;
                                }
                            }
                            BoardManager.Instance.MoveBlocks(draggedBlock);
                            BoardManager.Instance.CheckBoard(draggedBlock.transform);
                        }
                        else
                        {
                            MoveDraggedBlock();
                        }

                        ResetDraggedBlock();
                    }
                }
            }

            yield return null;
        }
    }

    private bool IsInRange(Vector2 o, Vector2 e)
    {
        if (GameDataManager.Instance.firstGame)
        {
            return BoardManager.Instance.IsInFirstGameRange(o);
        }
        return BoardManager.Instance.IsInRange(o, e);
    }

    private bool IsEmpty(Block b, Vector2 o)
    {
        return BoardManager.Instance.IsEmpty(b, o);
    }

    private Vector2Int RoundVector2(Vector2 v)
    {
        return new Vector2Int((int)(v.x + 0.5f), (int)(v.y + 0.5f));
    }

    private void RemoveAllHightLights()
    {
        if (!GameManager.Instance.gameOver)
        {
            foreach (BlockTile b in BoardManager.Instance.boardBlocks)
            {
                if (b)
                {
                    b.Fade(0.2f, Color.white);
                    b.FadeSprite(b.defaultSprite);
                }
            }
        }

        for (int i = 0; i < 9; i++)
        {
            if (highlightedTiles[i])
            {
                highlightedTiles[i].RemoveHightLight();
                highlightedTiles[i] = null;
            }
        }
    }

    private void MoveDraggedBlock()
    {
        draggedBlock.Scale(false, 0.2f);
        draggedBlock.Move(0.25f, draggedBlock.basePosition);
        draggedBlock.ChangeSprite(draggedBlock.defaultSprite);
        if (GameDataManager.Instance.firstGame)
        {
            BoardManager.Instance.teachObjects[BoardManager.Instance.firstGameIndex].SetActive(true);
        }
    }

    private void ResetDraggedBlock()
    {
        startPos = Vector3.zero;
        draggedBlock = null;
        RemoveAllHighlights();
    }

    private void RemoveAllHighlights()
    {
        if (!GameManager.Instance.gameOver)
            foreach (BlockTile b in BoardManager.Instance.boardBlocks)
            {
                if (b)
                {
                    b.Fade(0.2f, Color.white);
                    b.FadeSprite(b.defaultSprite);
                }
            }

        for (int i = 0; i < 9; i++)
        {
            if (highlightedTiles[i])
            {
                highlightedTiles[i].RemoveHightLight();
                highlightedTiles[i] = null;
            }
        }
    }

    private void ShowTouchFx(Vector2 pos)
    {
        Vector3 vector3 = Camera.main.ScreenToWorldPoint(pos)+new Vector3(0,0,3);
        GameManager.Instance.fxController.SpawnTouchFX(vector3);
    }

    private Vector3 BlockPosition(Vector2 o, Vector2 s)
    {
        Vector3 off = Vector3.zero;

        if (s.x % 2 == 1)
            off.x = 0.5f;
        if (s.y % 2 == 1)
            off.y = 0.5f;

        return new Vector3((int)(o.x + 0.5f) + (int)(s.x / 2), (int)(o.y + 0.5f) + (int)(s.y / 2), -1) + off;
    }

    private void OnDestroy()
    {
        if (InputCor != null)
        {
            StopCoroutine(InputCor);
            InputCor = null;
        }
    }
}
