using Cinemachine;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraManager : Singleton<CameraManager>
{
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineConfiner confiner;

    public float moveSpeed = 5f; // 摄像机移动速度;

    protected override void Awake()
    {
        base.Awake();
        Transform virtualCameraTransform = transform.Find("Virtual Camera");
        Assert.IsNotNull(virtualCameraTransform);
        virtualCamera = virtualCameraTransform.GetComponent<CinemachineVirtualCamera>();
        Assert.IsNotNull(virtualCamera);
        confiner = virtualCameraTransform.GetComponent<CinemachineConfiner>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SelectionManager.Instance.Selected += OnSelected;
        SelectionManager.Instance.Deselected += OnDeselected;
    }

    // Update is called once per frame
    void Update()
    {
        //if (virtualCamera.Follow == null)
        //{
        Vector2 direction = Vector2.zero;

        float horizontal = Input.GetAxis("Horizontal"); // 对应 A (-1) 和 D (+1)
        float vertical = Input.GetAxis("Vertical");     // 对应 W (+1) 和 S (-1)

        direction.x = horizontal;
        direction.y = vertical;

        if (direction != Vector2.zero)
        {
            virtualCamera.Follow = null;
            virtualCamera.transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
        }
        // https://discussions.unity.com/t/how-to-manually-and-smoothly-pan-a-cinemachine-vcam-while-retaining-confiner-functionality/819347
        //virtualCamera.transform.position = virtualCamera.State.CorrectedPosition;
        //}
    }

    private void LateUpdate()
    {
        // 好像放在LateUpdate里合理点
        // 只有CinemachineConfiner有CameraWasDisplaced方法
        if (confiner.CameraWasDisplaced(virtualCamera))
        {
            virtualCamera.transform.position = virtualCamera.State.CorrectedPosition;
        }
    }

    public void OnSelected(Selection selection)
    {
        if (selection.selectedType == SelectionManager.SelectedType.Character)
        {
            virtualCamera.Follow = selection.transform.root;
        }
    }

    public void OnDeselected(Selection selection)
    {
        if (selection.selectedType == SelectionManager.SelectedType.Character)
        {
            virtualCamera.Follow = null;
        }
    }
}
