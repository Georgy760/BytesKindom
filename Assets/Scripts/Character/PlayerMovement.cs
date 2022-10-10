using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    public float rotationPeriod = 0.3f;
    public bool isRotate;

    [SerializeField] private Vector2 pos1, pos2;

    [SerializeField] private bool canMove;

    [SerializeField] private float boxScaleX;

    [SerializeField] private float boxScaleY;

    [SerializeField] private float boxScaleZ;

    [SerializeField] private Ease spawnEase = Ease.InQuad;

    [SerializeField] private float spawnDur = 1f, spawnHeight = 10f;

    [SerializeField] private AnimationCurve animationCurve;
    private bool playing = false;
    [SerializeField] private float timer = 0f;

    public float fallinSpeed;
    public float fallinRotSpeed;
    private float directionX;
    private float directionZ;
    public Coroutine fallinCoroutine;
    private Quaternion fromRotation;
    private Vector2 lastPos1, lastPos2;
    private float radius = 1;
    private float rotationTime;
    private Vector3 scale;

    private float startAngleRad;
    private Vector3 startPos;
    private Quaternion toRotation;
    private bool win = false;

    public void Reset()
    {
        isRotate = false;
        //GetComponent<Rigidbody>().useGravity = false;
        playerController.Reset();
        //GetComponent<Rigidbody>().velocity = Vector3.zero;
        //GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        if (fallinCoroutine != null) StopCoroutine(fallinCoroutine);
    }

    // Use this for initialization
    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        // 直方体のサイズを取得
        scale = transform.lossyScale;
        //Debug.Log ("[x, y, z] = [" + boxScaleX + ", " + boxScaleY + ", " + boxScaleZ + "]");
    }

    // Update is called once per frame
    public void Update()
    {
        float x = 0;
        float y = 0;


        x = Input.GetAxisRaw("Vertical");
        if (x == 0) y = -Input.GetAxisRaw("Horizontal");

        if (canMove) ProcessInput(x, y);
    }

    private void FixedUpdate()
    {
        if (isRotate)
        {
            rotationTime += Time.fixedDeltaTime; // Augmenter le temps écoulé
            var ratio = Mathf.Lerp(0, 1,
                rotationTime / rotationPeriod); // Le rapport du temps écoulé actuel au temps de rotation


            var thetaRad = Mathf.Lerp(0, Mathf.PI / 2f, ratio);
            var distanceX = -directionX * radius * (Mathf.Cos(startAngleRad) - Mathf.Cos(startAngleRad + thetaRad));
            var distanceY = radius * (Mathf.Sin(startAngleRad + thetaRad) - Mathf.Sin(startAngleRad));
            var distanceZ = directionZ * radius * (Mathf.Cos(startAngleRad) - Mathf.Cos(startAngleRad + thetaRad));
            transform.position = new Vector3(startPos.x + distanceX, startPos.y + distanceY, startPos.z + distanceZ);
            //Debug.DrawRay(transform.position, new Vector3(-directionX, 0, directionZ)*3,Color.red);
            //Debug.LogError(rotationTime);

            transform.rotation = Quaternion.Lerp(fromRotation, toRotation, ratio);


            if (ratio == 1)
            {
                if (LevelManager.Instance.IsPlayerPosOutOfBound(pos1) &&
                    LevelManager.Instance.IsPlayerPosOutOfBound(pos2))
                {
                    playerController._audioSourceOthers.clip = playerController.AudioClips[(int) AudioClipsPresets.Fall];
                    playerController._audioSourceOthers.Play();
                    win = false;
                    playing = false;
                    ComputeFallBothNotValide(pos1, pos2, new Vector3(-directionX, 0, directionZ));
                    playerController.Kill();
                    Debug.Log("Both");
                }
                else if (LevelManager.Instance.IsPlayerPosOutOfBound(pos2))
                {
                    playerController._audioSourceOthers.clip = playerController.AudioClips[(int) AudioClipsPresets.Fall];
                    playerController._audioSourceOthers.Play();
                    win = false;
                    playing = false;
                    ComputeFall(pos2, pos1);
                    playerController.Kill();
                    Debug.Log("Pos2");
                }
                else if (LevelManager.Instance.IsPlayerPosOutOfBound(pos1))
                {
                    playerController._audioSourceOthers.clip = playerController.AudioClips[(int) AudioClipsPresets.Fall];
                    playerController._audioSourceOthers.Play();
                    win = false;
                    playing = false;
                    ComputeFall(pos1, pos2);
                    playerController.Kill();
                    Debug.Log("Pos1");
                }
                else if (LevelManager.Instance.IsOnUnstableTile(pos1, pos2))
                {
                    playerController._audioSourceOthers.clip = playerController.AudioClips[(int) AudioClipsPresets.Unstable];
                    playerController._audioSourceOthers.Play();
                    win = false;
                    playing = false;
                    StartFallFromUnstableTile();
                    playerController.Kill();
                    Debug.Log("Unstable Tile");
                }
                else if (LevelManager.Instance.HasPlayerWin(pos1, pos2))
                {
                    playerController._audioSourceOthers.clip = playerController.AudioClips[(int) AudioClipsPresets.Goal];
                    playerController._audioSourceOthers.Play();
                    SetCanMove(false);
                    win = true;
                    playing = false;
                    playerController.ValidateLevel();
                    //LevelManager.Instance.StartRestartLevelCoroutine();
                    LevelManager.Instance.StartNextLevelCoroutine();
                }
                else if (LevelManager.Instance.IsPlayerOnTeleporter(pos1, pos2))
                {
                    var tp = (TeleporterBlock) LevelManager.Instance.GetTileAt(pos1);
                    var newPos = LevelManager.Instance.GetOtherTeleporter(tp);
                    if (newPos != Vector2.one * -1)
                    {
                        //playerController.TeleportTo(new Vector3(newPos.x, transform.position.y, newPos.y));
                        playerController._audioSourceOthers.clip =
                            playerController.AudioClips[(int) AudioClipsPresets.Teleport];
                        playerController._audioSourceOthers.Play();
                        playerController.TeleportTo(newPos);
                    }
                }
                else if (LevelManager.Instance.IsPlayerOnSoftSwitch(pos1, pos2))
                {
                    var tile = LevelManager.Instance.GetTileAt(pos1);
                    PresurePlateBlock button = null;
                    if (tile is PresurePlateBlock)
                        button = (PresurePlateBlock) tile;
                    else
                        button = (PresurePlateBlock) LevelManager.Instance.GetTileAt(pos2);
                    playerController._audioSourceOthers.clip = playerController.AudioClips[(int) AudioClipsPresets.Button];
                    playerController._audioSourceOthers.Play();
                    button.TriggerBridge();
                }
                else if (LevelManager.Instance.IsPlayerOnHardSwitch(pos1, pos2))
                {
                    var tile = LevelManager.Instance.GetTileAt(pos1);
                    PresurePlateBlock button = null;
                    if (tile is PresurePlateBlock) button = (PresurePlateBlock) tile;
                    playerController._audioSourceOthers.clip = playerController.AudioClips[(int) AudioClipsPresets.Button];
                    playerController._audioSourceOthers.Play();
                    button.TriggerBridge();
                } /*
				else if (LevelManager.Instance.IsPlayerOnHardSwitch(pos1, pos2))
				{
					LevelManager.Instance.myLevel.TriggerBridge();
				}*/

                SetIsPlayerOnTile(true);

                isRotate = false;
                directionX = 0;
                directionZ = 0;
                rotationTime = 0;
            }
        }
    }

    public void SetPos(Vector2 newPos1, Vector2 newPos2)
    {
        pos1 = newPos1;
        pos2 = newPos2;
    }

    public Vector2 GetPos1()
    {
        return pos1;
    }

    public Vector2 GetPos2()
    {
        return pos2;
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    public bool CanMove()
    {
        return canMove;
    }
    
    public IEnumerator Spawn(int x, int z)
    {
        Reset();
        playerController.alive = true;
        var newPos = new Vector3(x, 1, z);
        transform.position = newPos;
        pos1 = pos2 = new Vector2(x, z);
        SetCanMove(false);
        playerController.modelHolder.gameObject.SetActive(true);
        yield return StartCoroutine(SpawnAnim());
        var block = LevelManager.Instance.GetTileAt(pos1);
        if (block != null) block.SetIsPlayerOn(true);
    }

    

    private IEnumerator SpawnAnim()
    {
        Debug.Log("SpawnAnim");
        Tween t = transform.DOMoveY(spawnHeight, spawnDur).From().SetEase(spawnEase);
        yield return t.WaitForCompletion();
        playing = true;
        win = false;
        StartCoroutine(GameTimer());
        SetCanMove(true);
    }

    private IEnumerator GameTimer()
    {
        var timeStamp = Time.time;
        yield return new WaitUntil(() => !playing);
        Debug.Log("Finish: " + timer);
        timer = Time.time - timeStamp;
        Debug.Log("ID: " + LevelLoader.Instance.PlayerID + "\nLevelIndex: " + LevelLoader.Instance.LevelIndex +
                  "\nMovesCount: " + playerController.moves.Count + "\nWin: " + win + "\nTime: " + timer);
        if (win)
        {
            Debug.Log("win");
            WebManager.WM.SaveLevel(LevelLoader.Instance.PlayerID, LevelLoader.Instance.LevelIndex, playerController.moves.Count, 1, 0, timer.ToString());

        }
        else
        {
            Debug.Log("loose");
            WebManager.WM.SaveLevel(LevelLoader.Instance.PlayerID, LevelLoader.Instance.LevelIndex, playerController.moves.Count, 0, 1, timer.ToString());

        }
        
    }

    public void ProcessInput(float x, float y)
    {
        // S'il y a une entrée de touche et que le Cube ne tourne pas, faites pivoter le Cube.
        if ((x != 0 || y != 0) && !isRotate && playerController.alive && canMove)
        {
            //Debug.Log("x : " + x + " y  : " + y );
            if (x > 0)
            {
                playerController.AddMove('u');
                Debug.Log("u");
            }
            else if (x < 0)
            {
                playerController.AddMove('d');
                Debug.Log("d");
            }
            else if (y > 0)
            {
                playerController.AddMove('l');
                Debug.Log("l");
            }
            else if (y < 0)
            {
                playerController.AddMove('r');
                Debug.Log("r");
            }

            directionX = y; // Sens de rotation défini (x ou y vaut toujours 0)
            directionZ = x; // Sens de rotation défini (x ou y vaut toujours 0)
            startPos = transform.position; // Conserver les coordonnées avant la rotation
            fromRotation = transform.rotation; // Conserve le quaternion avant la rotation
            transform.Rotate(directionZ * 90, 0, directionX * 90,
                Space.World); // Rotation de 90 degrés dans le sens de rotation
            toRotation = transform.rotation; // Conserve les quaternions après rotation
            transform.rotation =
                fromRotation; // Réglez la rotation du cube sur avant la rotation. (Je me demande s'il peut s'agir d'une copie superficielle de transform ...)
            setRadius(); // Calculer le rayon de giration
            rotationTime = 0; // Temps écoulé pendant la rotation à 0
            isRotate = true; // Peut commencer la rotation

            // for some reasons : WARNING
            // x = -y
            // y = x


            //calcul les nouvel position
            SetIsPlayerOnTile(false);

            if (pos1 == pos2)
            {
                pos1 += new Vector2(-y, x).normalized;
                pos2 += new Vector2(-y, x).normalized * 2;
            }
            else
            {
                if (pos2.y == pos1.y && pos1.x != pos2.x)
                {
                    if (x != 0)
                    {
                        pos1 += new Vector2(-y, x).normalized;
                        pos2 += new Vector2(-y, x).normalized;
                    }
                    else
                    {
                        var closerPos = pos1;
                        if (-y > 0)
                        {
                            if (pos2.x > pos1.x) closerPos = pos2;
                        }
                        else
                        {
                            if (pos2.x < pos1.x) closerPos = pos2;
                        }

                        pos2 = closerPos + new Vector2(-y, x).normalized;
                        pos1 = closerPos + new Vector2(-y, x).normalized;
                    }
                }
                else if (pos2.x == pos1.x && pos1.y != pos2.y)
                {
                    if (-y != 0)
                    {
                        pos1 += new Vector2(-y, x).normalized;
                        pos2 += new Vector2(-y, x).normalized;
                    }
                    else
                    {
                        var closerPos = pos1;
                        if (x > 0)
                        {
                            if (pos2.y > pos1.y) closerPos = pos2;
                        }
                        else
                        {
                            if (pos2.y < pos1.y) closerPos = pos2;
                        }

                        pos2 = closerPos + new Vector2(-y, x).normalized;
                        pos1 = closerPos + new Vector2(-y, x).normalized;
                    }
                }
            }

            //Debug.Log("VALIDE MOVE : " + (!LevelManager.Instance.IsPlayerPosOutOfBound(pos1) && !LevelManager.Instance.IsPlayerPosOutOfBound(pos2)));
        }
    }

    public void SetIsPlayerOnTile(bool value)
    {
        var block = LevelManager.Instance.GetTileAt(pos1);
        if (block != null) block.SetIsPlayerOn(value);
        if (pos1 != pos2)
        {
            block = LevelManager.Instance.GetTileAt(pos2);
            if (block != null) block.SetIsPlayerOn(value);
        }
    }

    public void StartFallFromUnstableTile()
    {
        if (fallinCoroutine != null) StopCoroutine(fallinCoroutine);

        fallinCoroutine = StartCoroutine(FallFromUnstableTile());
    }

    public IEnumerator FallFromUnstableTile()
    {
        var block = LevelManager.Instance.GetTileAt(pos1);
        block.Fall();
        transform.DOPunchRotation(block.shakefallTweenForce * 2 * Vector3.one, block.shakefallTweenDuration);
        yield return new WaitForSeconds(block.shakefallTweenDuration);
        while (true)
        {
            transform.position = transform.position + Vector3.down * Time.deltaTime * fallinSpeed * 1.5f;
            yield return null;
        }
    }

    public void ComputeFall(Vector2 pos1, Vector2 pos2)
    {
        var pos = new Vector3(pos1.x, 0, pos1.y);
        var dir = new Vector3((pos1 - pos2).x, 0, (pos1 - pos2).y).normalized;
        var rotAround = pos - dir / 2;
        var axis = Vector3.right;

        if (dir.z < 0)
            axis = -axis;
        //Debug.Log("Axis inverse");
        if (Vector3.Cross(Vector3.right, dir).magnitude == 0)
        {
            axis = Vector3.forward;
            //Debug.Log("Axis CHange");
            if (dir.x > 0)
                //Debug.Log("Axis inverse");
                axis = -axis;
        }

        //transform.RotateAround(rotAround, axis, 90);
        StartFallingCoroutine(rotAround, axis, 90);
        Debug.DrawRay(rotAround, axis * 3, Color.green);
        Debug.DrawRay(rotAround, dir * 5, Color.yellow);
        Debug.DrawRay(rotAround, dir * 1, Color.blue);
    }

    public void ComputeFallBothNotValide(Vector2 pos1, Vector2 pos2, Vector3 dir)
    {
        var pos = new Vector3(pos1.x - pos2.x, 0, pos1.y - pos2.y);
        pos = new Vector3(pos2.x, 0, pos2.y) + pos;
        var rotAround = Vector3.zero;
        if (pos1 == pos2)
            rotAround -= new Vector3(0, 1, 0);
        else
            rotAround -= new Vector3(0, 0.5f, 0);
        rotAround = pos - dir / 2;
        var axis = Vector3.right;


        if (dir.z < 0)
            axis = -axis;
        //Debug.Log("Axis inverse");
        if (Vector3.Cross(Vector3.right, dir).magnitude == 0)
        {
            axis = Vector3.forward;
            //Debug.Log("Axis CHange");
            if (dir.x > 0)
                //Debug.Log("Axis inverse");
                axis = -axis;
        }

        /*if (pos1 != pos2)
        {
            if (dir.z < 0)
            {
                axis = -axis;
                Debug.Log("Axis inverse");
            }
            if (Vector3.Cross(Vector3.right, dir).magnitude == 0)
            {
                axis = Vector3.forward;
                Debug.Log("Axis CHange");
                if (dir.x > 0)
                {
                    Debug.Log("Axis inverse");
                    axis = -axis;
                }
            }
        }
        else
        {
            if (Vector3.Cross(Vector3.right, dir).magnitude == 0)
            {
                axis = Vector3.forward;
                Debug.Log("Axis CHange");
            }
        }*/

        //transform.RotateAround(rotAround, axis, 90);
        StartFallingCoroutine(rotAround, axis, 45);
        Debug.DrawRay(rotAround, axis * 3, Color.green);
        Debug.DrawRay(rotAround, dir * 5, Color.yellow);
        Debug.DrawRay(rotAround, dir * 1, Color.blue);
        //Debug.LogError("both");
    }

    public void StartFallingCoroutine(Vector3 rotAround, Vector3 axis, float angle)
    {
        if (fallinCoroutine != null) StopCoroutine(fallinCoroutine);

        fallinCoroutine = StartCoroutine(Fall(rotAround, axis, angle));
    }

    public IEnumerator Fall(Vector3 rotAround, Vector3 axis, float angle)
    {
        var rotationLeft = angle;
        float rotAmount = 0;
        while (rotationLeft > 0)
        {
            //rotAmount = Mathf.Min(rotationLeft, fallinRotSpeed * Time.deltaTime);
            rotAmount = fallinRotSpeed * Time.deltaTime;
            transform.RotateAround(rotAround, axis, rotAmount);
            rotationLeft -= rotAmount;
            yield return null;
        }

        SetIsPlayerOnTile(false);
        while (true)
        {
            rotAmount = fallinRotSpeed * Time.deltaTime;
            transform.RotateAround(transform.position, axis, rotAmount);
            transform.position = transform.position + Vector3.down * Time.deltaTime * fallinSpeed;
            yield return null;
        }
    }

    private void setRadius()
    {
        var dirVec = new Vector3(0, 0, 0); // Vecteur de direction du mouvement
        var nomVec = Vector3.up; // (0,1,0)

        // Convertir la direction du mouvement en vecteur
        if (directionX != 0) // Déplacer dans la direction X
            dirVec = Vector3.right; // (1,0,0)
        else if (directionZ != 0) // Déplacer dans la direction Z
            dirVec = Vector3.forward; // (0,0,1)

        // Calculez le rayon et l'angle de départ dans la direction du mouvement à partir du produit intérieur du vecteur de direction et de la direction de l'objet.
        if (Mathf.Abs(Vector3.Dot(transform.right, dirVec)) >
            0.99) // La direction du mouvement est la direction x de l'objet
        {
            if (Mathf.Abs(Vector3.Dot(transform.up, nomVec)) > 0.99) // L'axe y de global est la direction y de l'objet
            {
                radius = Mathf.Sqrt(Mathf.Pow(boxScaleX / 2f, 2f) + Mathf.Pow(boxScaleY / 2f, 2f)); // Rayon de giration
                startAngleRad =
                    Mathf.Atan2(boxScaleY,
                        boxScaleX); // Angle du centre de gravité avant rotation à partir du plan horizontal
            }
            else if (Mathf.Abs(Vector3.Dot(transform.forward, nomVec)) >
                     0.99) //L'axe y de global est la direction z de l'objet
            {
                radius = Mathf.Sqrt(Mathf.Pow(boxScaleX / 2f, 2f) + Mathf.Pow(boxScaleZ / 2f, 2f));
                startAngleRad = Mathf.Atan2(boxScaleZ, boxScaleX);
            }
        }
        else if
            (Mathf.Abs(Vector3.Dot(transform.up, dirVec)) >
             0.99) //La direction du mouvement est la direction y de l'objet
        {
            if (Mathf.Abs(Vector3.Dot(transform.right, nomVec)) > 0.99)
            {
                radius = Mathf.Sqrt(Mathf.Pow(boxScaleY / 2f, 2f) + Mathf.Pow(boxScaleX / 2f, 2f));
                startAngleRad = Mathf.Atan2(boxScaleX, boxScaleY);
            }
            else if (Mathf.Abs(Vector3.Dot(transform.forward, nomVec)) > 0.99)
            {
                radius = Mathf.Sqrt(Mathf.Pow(boxScaleY / 2f, 2f) + Mathf.Pow(boxScaleZ / 2f, 2f));
                startAngleRad = Mathf.Atan2(boxScaleZ, boxScaleY);
            }
        }
        else if (Mathf.Abs(Vector3.Dot(transform.forward, dirVec)) > 0.99)
        {
            if (Mathf.Abs(Vector3.Dot(transform.right, nomVec)) > 0.99)
            {
                radius = Mathf.Sqrt(Mathf.Pow(boxScaleZ / 2f, 2f) + Mathf.Pow(boxScaleX / 2f, 2f));
                startAngleRad = Mathf.Atan2(boxScaleX, boxScaleZ);
            }
            else if (Mathf.Abs(Vector3.Dot(transform.up, nomVec)) > 0.99)
            {
                radius = Mathf.Sqrt(Mathf.Pow(boxScaleZ / 2f, 2f) + Mathf.Pow(boxScaleY / 2f, 2f));
                startAngleRad = Mathf.Atan2(boxScaleY, boxScaleZ);
            }
        }
        //Debug.Log (radius + ", " + startAngleRad);
    }
}