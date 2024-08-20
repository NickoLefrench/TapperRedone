using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeerMiniGame : MonoBehaviour
{
    private RectTransform tickRectTransform;
    private RectTransform stationaryTickRectTransform;
    private float barEndPosition;
    private float timeToMove;

    public void BeerMiniGameController(RectTransform tick, RectTransform stationaryTick, float barEnd, float moveTime)
    {
        tickRectTransform = tick;
        stationaryTickRectTransform = stationaryTick;
        barEndPosition = barEnd;
        timeToMove = moveTime;
    }

    public void StartBeerMiniGame(MonoBehaviour context)
    {
        tickRectTransform.anchoredPosition = new Vector2(0, 0);
        context.StartCoroutine(MoveTickCoroutine(true));
    }

    private IEnumerator MoveTickCoroutine(bool movingRight)
    {
        while (true)
        {
            float targetX = movingRight ? barEndPosition : 0;

            bool completed = false;
            LeanTween.moveX(tickRectTransform, targetX, timeToMove).setOnComplete(() => {
                completed = true;
            });

            yield return new WaitUntil(() => completed);

            movingRight = !movingRight;

            yield return CheckForHitCoroutine();
        }
    }

    private IEnumerator CheckForHitCoroutine()
    {
        while (LeanTween.isTweening(tickRectTransform))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                float distance = Mathf.Abs(tickRectTransform.anchoredPosition.x - stationaryTickRectTransform.anchoredPosition.x);
                float hitThreshold = 10f;

                if (distance <= hitThreshold)
                {
                    Debug.Log("Hit!");
                }
                else
                {
                    Debug.Log("Missed!");
                }

                yield break;
            }

            yield return null;
        }
    }
}

