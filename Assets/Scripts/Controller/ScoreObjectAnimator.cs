using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreObjectAnimator : MonoBehaviour
{
    private float duration;
    private float fraction;

    private Vector3 startScale;
    private Vector3 destination;

    public void PlayAnimator(Vector3 targetScale,float speed=0.4f)
    {
        gameObject.SetActive(true);
        transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        duration = speed;
        fraction = 0;
        startScale = transform.localScale;
        destination = targetScale;
    }

    private void Update()
    {
        if (fraction >= 2f)
        {
            gameObject.SetActive(false);
        }

        fraction += Time.deltaTime / duration;

        transform.localScale = Vector3.Lerp(startScale, destination, fraction);
    }
}
