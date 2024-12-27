using System;
using UnityEngine;

public class SelectionManager : Singleton<SelectionManager>
{
    public event Action<Selection> Selected;
    public event Action<Selection> Deselected;

    public enum SelectedType
    {
        None,
        Character
    }

    public const string LAYER_SELECTABLE = "Selectable";

    [ReadOnly] public Selection currentSelected = null;

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

            Selection newSelected = null;
            if (hit)
            {
                newSelected = hit.collider.GetComponent<Selection>();
                if (newSelected != null)
                {
                    if (newSelected != currentSelected)
                    {
                        if (currentSelected != null)
                        {
                            currentSelected.SetDeselectedColor();
                            Deselected?.Invoke(currentSelected);
                        }

                        newSelected.SetSelectedColor();
                        Selected?.Invoke(newSelected);

                        currentSelected = newSelected;
                    }
                }
            }
            else
            {
                if (currentSelected != null)
                {
                    currentSelected.SetDeselectedColor();
                    Deselected?.Invoke(currentSelected);
                    currentSelected = null;
                }
            }
        }
    }

    public static T FindComponentInChildren<T>(GameObject gameObject) where T : Component
    {
        // Check if the component exists on the current GameObject
        T component = gameObject.GetComponent<T>();
        if (component != null)
            return component;

        // Search in all child GameObjects
        foreach (Transform child in gameObject.transform)
        {
            component = FindComponentInChildren<T>(child.gameObject);
            if (component != null)
                return component;
        }

        // Return null if no component is found
        return null;
    }
}
