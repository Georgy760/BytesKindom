using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance; 
    public delegate void PlayerAction();

    public PlayerMovement myPlayerMovement;
    public ModelHolder modelHolder;

    public float validateTweenDuration;
    public float hideTweenDuration;
    public float fallTweenDuration;


    public float delayBetweenTpTween;
    public float tpDespawnDurationTween;

    public float tpRespawnDurationTween;

    /* public float tpRespawnForceXTween;
     public float tpRespawnForceZTween;
     public float tpRespawnForceMinZTween;
     public float tpRespawnForceMinXTween;
     public float tpRespawnRotationAmountTween;*/
    public Ease tpRespawnEase;

    public bool alive;

    public List<char> moves;
    [SerializeField] private TMP_Text _score;
    public AudioSource _audioSourceOthers;
    public AudioSource _audioSourcePlayerActions;
    public List<AudioClip> AudioClips;

    private Vector3 newPos;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Reset()
    {
        Show();
        transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        moves = new List<char>();
        if (_score != null) _score.text = "Moves: " + moves.Count;
    }

    public void Start()
    {
        alive = true;
        _audioSourceOthers.volume = SettingsMenu.Instance._actionVolume;
        _audioSourcePlayerActions.volume = SettingsMenu.Instance._actionVolume;
    }

    public event PlayerAction OnPlayerDie;

    public void AddMove(char move)
    {
        if (!(move == 'u' || move == 'd' || move == 'l' || move == 'r')) return;

        moves.Add(move);
        if (_audioSourcePlayerActions.clip != AudioClips[(int) AudioClipsPresets.Action])
            _audioSourcePlayerActions.clip = AudioClips[(int) AudioClipsPresets.Action];
        _audioSourcePlayerActions.Play();
        if (_score != null) _score.text = "Moves: " + moves.Count;
    }

    [ContextMenu("Validate")]
    public void ValidateLevel()
    {
        transform.DOMoveY(-12, validateTweenDuration).SetEase(Ease.InBack).OnComplete(Hide);
        LevelManager.Instance.AddLevelSolutions(moves);
    }

    public void Hide()
    {
        transform.DOScale(Vector3.zero, hideTweenDuration);
    }

    public void Show(bool pop = false)
    {
        if (pop)
            transform.DOScale(Vector3.one, hideTweenDuration);
        else
            transform.localScale = Vector3.one;
    }

    public void Kill()
    {
        alive = false;
        LevelManager.Instance.AddLevelMoves(moves);

        if (OnPlayerDie != null) OnPlayerDie.Invoke();
    }

    public void TeleportTo(Vector2 newPos)
    {
        myPlayerMovement.SetCanMove(false);
        this.newPos = new Vector3(newPos.x, transform.position.y, newPos.y);

        var mySequence = DOTween.Sequence();

        mySequence.PrependInterval(0.15f);
        var block = LevelManager.Instance.GetTileAt(myPlayerMovement.GetPos1());
        if (block is TeleporterBlock)
        {
            ((TeleporterBlock) block).Teleport();
            myPlayerMovement.SetPos(newPos, newPos);
        }
        else
        {
            return;
        }
        // Add a rotation tween as soon as the previous one is finished

        //mySequence.Append(transform.DOScale(Vector3.zero, hideTweenDuration)).OnComplete(TeleportToBis);
        /* mySequence.Insert(0, transform.DORotate(new Vector3(0, tpRespawnRotationAmountTween, 0), tpRespawnDurationTween * 1.125f, RotateMode.WorldAxisAdd));
         mySequence.Insert(0,transform.DOScaleX(tpRespawnForceXTween, tpRespawnDurationTween / 2f));
         mySequence.Insert(0,transform.DOScaleZ(tpRespawnForceMinZTween, tpRespawnDurationTween / 2f));
         mySequence.Insert(tpRespawnDurationTween / 2f,transform.DOScaleZ(tpRespawnForceZTween, tpRespawnDurationTween / 2f));
         mySequence.Insert(tpRespawnDurationTween / 2f, transform.DOScaleX(tpRespawnForceMinXTween, tpRespawnDurationTween / 2f));
         mySequence.Insert(tpRespawnDurationTween, transform.DOScaleZ(0, tpRespawnDurationTween / 4f));
         mySequence.Insert(tpRespawnDurationTween, transform.DOScaleX(0, tpRespawnDurationTween / 4f));
         mySequence.Insert(tpRespawnDurationTween *1.125f, transform.DOScaleY(0, tpRespawnDurationTween / 8f));*/
        mySequence.Insert(0, transform.DOScaleZ(0f, tpDespawnDurationTween));
        mySequence.Insert(0, transform.DOScaleX(0f, tpDespawnDurationTween));
        mySequence.OnComplete(TeleportToBis);
    }

    public void TeleportToBis()
    {
        transform.position = newPos;
        //Show(true);
        var mySequence = DOTween.Sequence();
        var block = LevelManager.Instance.GetTileAt(myPlayerMovement.GetPos1());
        if (block is TeleporterBlock) ((TeleporterBlock) block).ReceiveTeleport();
        mySequence.PrependInterval(delayBetweenTpTween);
        mySequence.OnComplete(TeleportToBisBis);
    }

    public void TeleportToBisBis()
    {
        var mySequence = DOTween.Sequence();
        /* mySequence.Insert(0, transform.DORotate(new Vector3(0, tpRespawnRotationAmountTween, 0), tpRespawnDurationTween*1.125f, RotateMode.WorldAxisAdd));
         mySequence.Insert(0, transform.DOScaleZ(tpRespawnForceZTween, tpRespawnDurationTween / 2f));
         mySequence.Insert(0, transform.DOScaleX(tpRespawnForceMinXTween, tpRespawnDurationTween / 2f));
         mySequence.Insert(0, transform.DOScaleY(1, tpRespawnDurationTween / 8f));
         mySequence.Insert(tpRespawnDurationTween / 2f, transform.DOScaleX(tpRespawnForceXTween, tpRespawnDurationTween / 2f));
         mySequence.Insert(tpRespawnDurationTween / 2f, transform.DOScaleZ(tpRespawnForceMinZTween, tpRespawnDurationTween / 2f));
         mySequence.Insert(tpRespawnDurationTween, transform.DOScaleZ(1, tpRespawnDurationTween / 4f));
         mySequence.Insert(tpRespawnDurationTween, transform.DOScaleX(1, tpRespawnDurationTween / 4f));*/
        mySequence.Insert(0, transform.DOScaleX(1, tpRespawnDurationTween).SetEase(tpRespawnEase));
        mySequence.Insert(0, transform.DOScaleZ(1, tpRespawnDurationTween).SetEase(tpRespawnEase));
        /* mySequence.Insert(0, transform.DOScaleZ(tpForceMinZTween, tpDurationTween / 2f));
         mySequence.Insert(0, transform.DOScaleX(tpForceXTween, tpDurationTween / 2f));
         mySequence.Insert(0, transform.DOScaleY(1, tpDurationTween / 8f));
         mySequence.Insert(tpDurationTween / 2f, transform.DOScaleX(tpForceMinXTween, tpDurationTween / 2f));
         mySequence.Insert(tpDurationTween / 2f, transform.DOScaleZ(tpForceZTween, tpDurationTween / 2f));
         mySequence.Insert(tpDurationTween, transform.DOScaleZ(1, tpDurationTween / 4f));
         mySequence.Insert(tpDurationTween, transform.DOScaleX(1, tpDurationTween / 4f));*/
        mySequence.OnComplete(ActivateMovement);
    }

    public void ActivateMovement()
    {
        myPlayerMovement.SetCanMove(true);
    }

    public void Fall()
    {
        var mySequence = DOTween.Sequence();
        
        mySequence.PrependInterval(0.15f);
        // Add a rotation tween as soon as the previous one is finished
        mySequence.Append(transform.DOScale(Vector3.zero, fallTweenDuration));
    }

    public bool isMoving()
    {
        return myPlayerMovement.isRotate;
    }

    public void ProcessAction(PlayerActions action)
    {
        float x = 0;
        float y = 0;
        switch (action)
        {
            case PlayerActions.Up:
                x = 1;
                y = 0;
                break;
            case PlayerActions.Down:
                x = -1;
                y = 0;
                break;
            case PlayerActions.Left:
                x = 0;
                y = 1;
                break;
            case PlayerActions.Right:
                x = 0;
                y = -1;
                break;
            case PlayerActions.Validate:
                ValidateLevel();
                break;
        }

        if (x != 0 || y != 0) myPlayerMovement.ProcessInput(x, y);
    }
}

public enum PlayerActions
{
    Up,
    Down,
    Left,
    Right,
    Validate
}

public enum AudioClipsPresets
{
    Action = 0,
    Button = 1,
    Fall = 2,
    Goal = 3,
    Teleport = 4,
    TilesFall = 5,
    Unstable = 6
}