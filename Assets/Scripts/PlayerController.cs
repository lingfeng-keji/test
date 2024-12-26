using System;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Assertions;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private new Rigidbody2D rigidbody2D = null;
    public float speed = 1.0f;
    public Vector2 motionVector;

    Animator animator;
    private string currentState;

    const string WALK_DOWN = "WalkDown";
    const string WALK_LEFT = "WalkLeft";
    const string WALK_UP = "WalkUp";
    const string WALK_RIGHT = "WalkRight";
    const string IDLE_DOWN = "IdleDown";
    const string IDLE_LEFT = "IdleLeft";
    const string IDLE_UP = "IdleUp";
    const string IDLE_RIGHT = "IdleRight";

    private Vector2Int FaceDirection = new Vector2Int(0, 0);

    public bool isPlayerControlled = true;

    private Seeker seeker;
    private List<Vector3> pathPointList;
    private int currentIndex = 0;
    public float stopDistance = 0.5f;
    public float reachPathPointDist = 0.1f;
    private float distToPathPoint = 100.0f;

    public Vector3 destPos;
    //public Vector3 lastDestPos;

    //private Vector3 lastPos;

    bool isMovingToDest = false;
    bool reachDest = false;

    public int health = 0;
    public int stamina = 0;

    Transform hudLogicTransform;

    GameObject uiActionBubble = null;

    public bool actionSuccess = false;

    public string playerName = null;

    public CharacterActionStateMachine _characterActionStateMachine;
    public CharacterActionStateMachine CharActionStateMachine => _characterActionStateMachine;

    void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();

        Transform graphicsTransform = transform.Find("Graphics");
        Assert.IsNotNull(graphicsTransform);
        animator = graphicsTransform.GetComponent<Animator>();
        Assert.IsNotNull(animator);

        seeker  = GetComponent<Seeker>();

        hudLogicTransform = transform.Find("HudLogic");

        _characterActionStateMachine = new CharacterActionStateMachine(this);
    }

    public void ShowBubble(String text)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(hudLogicTransform.position);
        Debug.Log(screenPosition);

        // 也许可以放到初始化里面
        if (uiActionBubble == null)
        {
            var canvas = UIManager.Instance.transform.Find("Canvas");
            uiActionBubble = Instantiate(UIManager.Instance.uiActionBubblePrefab, screenPosition, Quaternion.identity, canvas);
        }
        else
        {
            uiActionBubble.transform.position = screenPosition;
        }

        // 也许气泡逻辑应该写到单独的气泡脚本里
        var actionText = uiActionBubble.transform.Find("ActionText");
        var textComponent = actionText.GetComponent<TextMeshProUGUI>();
        textComponent.text = text;

        uiActionBubble.SetActive(true);

    }

    public void HideBubble()
    {
        if (uiActionBubble != null)
        {
            uiActionBubble.SetActive(false);
        }
    }

    private void Start()
    {
        //lastPos = transform.position;
        CharActionStateMachine.Initialize(CharActionStateMachine.idleState);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerControlled)
        {
            motionVector = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );
        }

        if (motionVector != Vector2.zero)
        {
            FaceDirection.x = GetSignOrZero(motionVector.x);
            FaceDirection.y = GetSignOrZero(motionVector.y);
        }
    }

    private void FixedUpdate()
    {
        // 仅测试
        //if (uiActionBubble != null)
        //{
        //    uiActionBubble.transform.position -= Vector3.up;
        //}
        Move();
    }

    public void StartMovingToDest(Vector2 dest)
    {
        CharActionStateMachine.TransitionTo(CharActionStateMachine.idleState);

        destPos = dest;

        if (CloseToDest())
        {
            isMovingToDest = false;
            reachDest = true;
        }
        else
        {
            GeneratePath(dest);

            isMovingToDest = true;
            reachDest = false;
        }
    }

    public void StopMovingToDest(bool reachDest)
    {
        isMovingToDest = false;
        this.reachDest = reachDest;
    }

    private bool CloseToDest()
    {
        bool isClose = false;

        float distance = Vector2.Distance(transform.position, destPos);

        if ( distance <= stopDistance)
        {
            isClose = true;
        }

        return isClose;
    }

    public bool ReachDest()
    {
        return reachDest;
    }

    public bool IsMovingToDest()
    {
        return isMovingToDest;
    }

    int GetSignOrZero(float value)
    {
        if (value > 0)
            return 1;
        else if (value < 0)
            return -1;
        else
            return 0;
    }

    private void Move()
    {
        if (!isPlayerControlled)
        {
            if (isMovingToDest)
            {
                if (CloseToDest())
                {
                    StopMovingToDest(true);
                    motionVector = Vector2.zero;
                }

                if (!reachDest)
                {
                    if (pathPointList != null && pathPointList.Count > 0)
                    {
                        if (currentIndex < pathPointList.Count)
                        {
                            distToPathPoint = Vector2.Distance(transform.position, pathPointList[currentIndex]);
                        }

                        if (distToPathPoint <= reachPathPointDist)
                        {
                            ++currentIndex;
                        }

                        if (currentIndex < pathPointList.Count)
                        {
                            motionVector = VectorUtils.GetDominantAxisDirection(pathPointList[currentIndex] - transform.position);
                        }
                        else
                        {
                            motionVector = Vector2.zero;

                            if (CloseToDest())
                            {
                                StopMovingToDest(true);
                            }
                            else
                            {
                                StopMovingToDest(false);
                            }
                        }
                    }
                }

                //AutoPath();
            }
        }

        Vector2 velocity = motionVector * speed;

        rigidbody2D.linearVelocity = velocity;

        bool shouldSetLeftOrRightAnimation = false;
        bool shouldSetUpOrDownAnimation = false;

        if (!Mathf.Approximately(velocity.x, 0.0f))
        {
            shouldSetLeftOrRightAnimation = true;
        }
        else if (!Mathf.Approximately(velocity.y, 0.0f))
        {
            shouldSetUpOrDownAnimation = true;
        }
 
        if (shouldSetLeftOrRightAnimation)
        {
            if (velocity.x > 0.0f)
            {
                ChangeAnimationState(WALK_RIGHT); // right
                //animator.Play(ANN_WALK_RIGHT, -1, 0.25f);
                //animator.speed = 0.01f;
            }
            else
            {
                ChangeAnimationState(WALK_LEFT); // left
            }
        }
        else if (shouldSetUpOrDownAnimation)
        {
            if (velocity.y > 0.0f)
            {
                ChangeAnimationState(WALK_UP); // up
            }
            else if (velocity.y < 0.0f)
            {
                ChangeAnimationState(WALK_DOWN); // down
            }
        }
        else
        {
            if (FaceDirection.x > 0)
            {
                ChangeAnimationState(IDLE_RIGHT);
            }
            else if (FaceDirection.x < 0)
            {
                ChangeAnimationState(IDLE_LEFT);
            }
            else if (FaceDirection.y > 0)
            {
                ChangeAnimationState(IDLE_UP);
            }
            else if (FaceDirection.y < 0)
            {
                ChangeAnimationState(IDLE_DOWN);
            }
            else
            {
                ChangeAnimationState(IDLE_DOWN);
            }
        }

        //lastPos = transform.position;
    }

    void ChangeAnimationState(string newState)
    {
        if (currentState == newState)
        {
            Assert.IsTrue(animator.GetCurrentAnimatorStateInfo(0).IsName(currentState));
            return;
        }

        animator.Play(newState);

        currentState = newState;
    }

    //private void AutoPath()
    //{
    //    if (isMovingToDest)
    //    {
    //        if (pathPointList == null || pathPointList.Count <= 0)
    //        {
    //            GeneratePath(destPos);
    //        }
    //        else if (Vector2.Distance(transform.position, pathPointList[currentIndex]) <= 0.1f)
    //        {
    //            ++currentIndex;
    //            if (currentIndex >= pathPointList.Count)
    //            {
    //                FinishMovingToDest();
    //                pathPointList.Clear();
    //            }
    //        }
    //    }



    //    //if (pathPointList == null || pathPointList.Count <= 0)
    //    //{
    //    //    // GeneratePath(destination)
    //    //}
    //}

    private void GeneratePath(Vector3 target)
    {
        currentIndex = 0;

        if (pathPointList != null)
        {
            pathPointList.Clear();
        }

        seeker.StartPath(transform.position, target, path =>
        {
            pathPointList = path.vectorPath;

            // 为了防止开始移动时抖动
            //float distX = Mathf.Abs(pathPointList[1].x - pathPointList[0].x);
            //float distY = Mathf.Abs(pathPointList[1].y - pathPointList[0].y);

            //Vector2 firstPathPoint;
            //if (distX >= distY)
            //{
            //    firstPathPoint = new Vector2(pathPointList[1].x, transform.position.y);
            //}
            //else
            //{
            //    firstPathPoint = new Vector2(transform.position.x, pathPointList[1].y);
            //}

            //pathPointList[0] = firstPathPoint;
        });
    }

    public static class VectorUtils
    {
        public static Vector2 GetDominantAxisDirection(Vector2 input)
        {
            if (Mathf.Approximately(input.sqrMagnitude, 0.0f))
            {
                return Vector2.zero;
            }

            if (Mathf.Abs(input.x) >= Mathf.Abs(input.y))
            {
                return new Vector2(input.x < 0.0f ? -1.0f : 1.0f, 0.0f);
            }
            else
            {
                return new Vector2(0.0f, input.y < 0.0f ? -1.0f : 1.0f);
            }
        }
    }
}
