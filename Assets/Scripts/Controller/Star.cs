using UnityEngine;

public class Star : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public int index;

    public float speed = 5f;

    public float acceleration = 5f;

    private Vector3 targetPosition;

    public Vector3 startPosition;

    private float journeyLength;

    private float currentSpeed;

    private float curveFactor = 1.5f;

    private float startTime;

    private bool isMove;

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void Move(Vector3 targetPos)
    {
        currentSpeed = 0;
        startPosition = transform.position;
        targetPosition = targetPos;
        startTime = Time.time;
        journeyLength = Vector3.Distance(startPosition, targetPosition);
        acceleration = Random.Range(5, 10);
        if (startPosition.x > 3.5)
        {
            curveFactor = 1.5f;
        }
        else
        {
            curveFactor = -1.5f;
        }
        isMove = true;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!isMove)
        {
            return;
        }
        float distanceCovered = (Time.time - startTime) * currentSpeed;
        float journeyFraction = distanceCovered / journeyLength;

        if (journeyFraction >= 1f)
        {
            MoveEnd();
            return;
        }
        currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, 10);
        Vector3 nextPosition = Vector3.Lerp(startPosition, targetPosition, journeyFraction);
        nextPosition.x += Mathf.Sin(journeyFraction * Mathf.PI) * curveFactor;
        transform.position = new Vector3(nextPosition.x, nextPosition.y, 0f);
    }

    private void MoveEnd()
    {
        EventManager.TriggerEvent(GameConstManager.SubtractStarUpdateUI,index);
        isMove = false;
        transform.position = targetPosition;
        TrashMan.despawn(gameObject);
    }
}
