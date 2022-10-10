using DG.Tweening;
using UnityEngine;

public class TeleporterModel : MonoBehaviour
{
    public float idleMinYValue;
    public Ease idleMinEase;
    public float idleMaxYValue;
    public Ease idleMaxEase;
    public float idleTweenDuration;
    public float idleMinZXValue;
    public float idleMaxZXValue;

    public float tpTweenDuration;
    public float tpMinYValue;
    public float tpMaxYValue;
    public float tpMinZXValue;
    public float tpMaxZXValue;
    public Ease tpMinEase;
    public Ease tpMaxEase;


    public float rtpTweenDuration;
    public float rtpYValue;
    public float rtpXZValue;
    public Sequence idleTween;

    // Start is called before the first frame update
    private void Start()
    {
        StartTween();
    }

    private void StartTween()
    {
        var mySequence = DOTween.Sequence();
        mySequence.Insert(0, transform.DOScaleZ(idleMinZXValue, 0));
        mySequence.Insert(0, transform.DOScaleX(idleMinZXValue, 0));

        mySequence.Insert(0, transform.DOScaleY(idleMaxYValue, idleTweenDuration / 2).SetEase(idleMinEase));
        mySequence.Insert(idleTweenDuration / 2,
            transform.DOScaleY(idleMinYValue, idleTweenDuration / 2).SetEase(idleMaxEase));
        /*  mySequence.Insert(0, transform.DOScaleZ(idleMinZXValue, idleTweenDuration / 2).SetEase(idleMinEase));
          mySequence.Insert(idleTweenDuration / 2, transform.DOScaleZ(idleMaxZXValue, idleTweenDuration / 2).SetEase(idleMaxEase));
          mySequence.Insert(0, transform.DOScaleX(idleMinZXValue, idleTweenDuration / 2).SetEase(idleMinEase));
          mySequence.Insert(idleTweenDuration / 2, transform.DOScaleX(idleMaxZXValue, idleTweenDuration / 2).SetEase(idleMaxEase));*/
        // mySequence.SetLoops(-1);
        mySequence.OnComplete(StartTween);
        idleTween = mySequence;
    }

    private void ReStartTween()
    {
        var mySequence = DOTween.Sequence();
        mySequence.PrependInterval(0.2f);
        mySequence.Insert(0, transform.DOScaleZ(idleMinZXValue, 0));
        mySequence.Insert(0, transform.DOScaleX(idleMinZXValue, 0));
        mySequence.Insert(0, transform.DOScaleY(0, 0));
        mySequence.OnComplete(StartTween);
        idleTween = mySequence;
    }

    public void StartTeleportationTween()
    {
        idleTween.Kill();
        var mySequence = DOTween.Sequence();
        mySequence.Insert(0, transform.DOScaleY(tpMinYValue, tpTweenDuration / 2).SetEase(tpMinEase));
        mySequence.Insert(tpTweenDuration / 2, transform.DOScaleY(tpMaxYValue, tpTweenDuration / 2).SetEase(tpMaxEase));

        mySequence.Insert(0, transform.DOScaleZ(tpMaxZXValue, tpTweenDuration / 2).SetEase(tpMaxEase));
        mySequence.Insert(tpTweenDuration / 2,
            transform.DOScaleZ(tpMinZXValue, tpTweenDuration / 2).SetEase(tpMinEase));

        mySequence.Insert(0, transform.DOScaleX(tpMaxZXValue, tpTweenDuration / 2).SetEase(tpMaxEase));
        mySequence.Insert(tpTweenDuration / 2,
            transform.DOScaleX(tpMinZXValue, tpTweenDuration / 2).SetEase(tpMinEase));

        mySequence.OnComplete(ReStartTween);
    }

    public void StartReceiveTeleportationTween()
    {
        idleTween.Kill();
        var mySequence = DOTween.Sequence();
        /*mySequence.Insert(0, transform.DOScaleY(rtpYValue, rtpTweenDuration));
        mySequence.Insert(0, transform.DOScaleX(rtpXZValue, rtpTweenDuration));
        mySequence.Insert(0, transform.DOScaleZ(rtpXZValue, rtpTweenDuration));*/
        mySequence.Insert(tpTweenDuration / 2, transform.DOScaleY(0.1f, tpTweenDuration / 2).SetEase(tpMinEase));
        mySequence.Insert(0, transform.DOScaleY(tpMaxYValue, tpTweenDuration / 2).SetEase(tpMaxEase));

        mySequence.Insert(tpTweenDuration / 2,
            transform.DOScaleZ(tpMaxZXValue, tpTweenDuration / 2).SetEase(tpMaxEase));
        mySequence.Insert(0, transform.DOScaleZ(0.1f, tpTweenDuration / 2).SetEase(tpMinEase));

        mySequence.Insert(tpTweenDuration / 2,
            transform.DOScaleX(tpMaxZXValue, tpTweenDuration / 2).SetEase(tpMaxEase));
        mySequence.Insert(0, transform.DOScaleX(0.1f, tpTweenDuration / 2).SetEase(tpMinEase));

        mySequence.Insert(tpTweenDuration,
            transform.DOScaleZ(idleMinZXValue, idleTweenDuration / 4).SetEase(idleMinEase));
        mySequence.Insert(tpTweenDuration,
            transform.DOScaleX(idleMinZXValue, idleTweenDuration / 4).SetEase(idleMinEase));

        mySequence.OnComplete(StartTween);
    }
}