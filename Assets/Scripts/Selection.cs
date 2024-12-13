using Unity.VisualScripting;
using UnityEngine;
using static SelectionManager;

public class Selection : MonoBehaviour
{
    public SelectionManager.SelectedType selectedType;

    Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        int LayerSelectable = LayerMask.NameToLayer("Selectable");
        gameObject.layer = LayerSelectable;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSelectedColor()
    {
        if (selectedType == SelectedType.None)
        {
            if (_renderer != null)
            {
                _renderer.material.color = Color.red;
            }    
        }  
    }

    public void SetDeselectedColor()
    {
        if (selectedType == SelectedType.None)
        {
            _renderer.material.color = Color.white;
        }
    }
}
