using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCookInterface;
using UnityEngine.SceneManagement;
using System;

public class BodyMover : MonoBehaviour {

    [Range(0.5f, 1.0f)]
    public float moveSpeed = 1.0f;
    private float delayTime = 3f;
    const float speedScalefactor = 2.0f;


    private bool isMoveActive = false;
    public bool isAlive = false;
    public bool movingClockWise = true;
    public bool switchingIdentifier = false;

    private int speedStack;
    public MoverType moverType;
    Transform thisTransform;
    [SerializeField] Animator animator;
    [SerializeField] GameObject particle;
    #region 애니메이션 관련
    readonly int _moveBlendHash = Animator.StringToHash("MoveBlend");
    readonly int _moveBooleanHash = Animator.StringToHash("IsMoveActive");
    readonly int _jumpHash = Animator.StringToHash("Jumping");
    readonly int _waitHash = Animator.StringToHash("Waiting");
    readonly int _hitHash = Animator.StringToHash("Hitting");
    
    private ActionState actionState;
    public ActionState ActionState
    {
        get { return actionState; }
        set
        {
            actionState = value;
            switch (actionState)
            {
                case ActionState.Jumping:
                    if (animator != null && animator.isActiveAndEnabled)
                        animator.SetTrigger(_jumpHash);
                    break;
                case ActionState.hitting:
                    IsMoveActive = false;
                    if (animator != null && animator.isActiveAndEnabled){
                        AudioManager.Instance.PlaySFXSound(SFXSoundType.MoverCrash);
                        animator.SetTrigger(_hitHash);
                    }
                    break;
                case ActionState.Waitng:
                    if (animator != null && animator.isActiveAndEnabled)
                        animator.SetTrigger(_waitHash);
                    break;
            }
        }
    }
    public bool IsMoveActive
    {
        get{
            return isMoveActive;
        }

        set{
            isMoveActive = value;
            if (isMoveActive){
                if (animator != null && animator.isActiveAndEnabled){
                    animator.SetBool(_moveBooleanHash, true);
                    animator.SetFloat(_moveBlendHash, moveSpeed);
                }
            }
            else
            {
                if (animator != null && animator.isActiveAndEnabled)
                {
                    animator.SetBool(_moveBooleanHash, false);
                }
                
            }

        }
    }



    #endregion

    private void OnEnable()
    {
        isAlive = false;
        GameDesignManager.LevelReformEvent += GameReset;
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(RoutineSetInitial());
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (this.gameObject.activeSelf) this.gameObject.SetActive(false);
    }
    public void GameReset()
    {
        StopAllCoroutines();
        this.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        GameDesignManager.LevelReformEvent -= GameReset;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        switch (moverType){
            case MoverType.Lizard:
                moveSpeed = 1.0f;
                break;
            case MoverType.Wolf:
                moveSpeed = 0.5f;
                break;
        }
        IsMoveActive = false;
        thisTransform.position = Vector3.zero;
        thisTransform.GetChild(0).gameObject.SetActive(false);
        thisTransform.GetChild(1).gameObject.SetActive(false);
        thisTransform.GetChild(2).gameObject.SetActive(false);
    }
    private void Awake()
    {
        // 여기서는 Transform이 자주 호출 되므로 미리 캐싱해둔다.
        thisTransform = GetComponent<Transform>();
        if (animator == null) GetComponentInChildren<Animator>();

    }
    private void Update()
    {
        if (IsMoveActive) // 항상 앞 방향으로 가는 움직임 작동 방식
        {
            thisTransform.Translate(Vector3.forward * (moveSpeed * speedScalefactor) * Time.deltaTime);
        }
    }

    public void Initialize(Identifier initial)
    {
        switch (initial)
        {
            case Identifier.Positive:
                movingClockWise = true;
                switchingIdentifier = false;
                isAlive = true;
                thisTransform.GetChild(0).gameObject.SetActive(true);
                thisTransform.GetChild(1).gameObject.SetActive(true);
                thisTransform.GetChild(2).gameObject.SetActive(true);
                break;
            case Identifier.Negative:
                movingClockWise = false;
                switchingIdentifier = true;
                isAlive = true;
                thisTransform.GetChild(0).gameObject.SetActive(true);
                thisTransform.GetChild(1).gameObject.SetActive(true);
                thisTransform.GetChild(2).gameObject.SetActive(true);
                break;
        }
    }
    IEnumerator RoutineSetInitial() 
    {
        ActionState = ActionState.Waitng;
        float elapsedTime = 0f;
        while (elapsedTime < delayTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        IsMoveActive = true;
        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Initialize(Identifier.Positive);
    }

    public void DieMover()
    {
        StopAllCoroutines();
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(12)) //BodyMover Layer
        {
            if (this.moverType == MoverType.Wolf) return;
            if (!other.CompareTag(this.tag) && this.isAlive) 
            {
                isAlive = false;
                ActionState = ActionState.hitting;
                
                GameDesignManager.Instance.GameLife -= 1;
                Invoke("DieMover", 1f);
            }
        }
    }


}
