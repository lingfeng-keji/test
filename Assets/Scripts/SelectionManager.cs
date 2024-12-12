using UnityEngine;

public class SelectionManager : Singleton<SelectionManager>
{
    public const string LAYER_SELECTABLE = "Selectable";

    public GameObject currentSelected = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            // https://discussions.unity.com/t/set-ray-only-when-raycast-a-specific-layer/154483
            //int layerMask = LayerMask.GetMask("Water");
            int layerMask = LayerMask.GetMask(LAYER_SELECTABLE);

            // https://discussions.unity.com/t/unity-2d-raycast-from-mouse-to-screen/520480
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, layerMask); // distance 0.0f也行？

            Selection selection = null;
            if (hit)
            {
                if (currentSelected != null)
                {
                    selection = currentSelected.GetComponent<Selection>();
                    selection.SetDeselectedColor();
                }

                currentSelected = hit.transform.gameObject;

                selection = currentSelected.GetComponent<Selection>();
                selection.SetSelectedColor();
            }
            else
            {
                if (currentSelected != null)
                {
                    selection = currentSelected.GetComponent<Selection>();
                    selection.SetDeselectedColor();
                }
            }
        }
    }
}
