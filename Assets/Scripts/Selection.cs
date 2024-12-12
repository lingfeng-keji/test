using UnityEngine;

public class Selection : MonoBehaviour
{
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
        _renderer.material.color = Color.red;
    }

    public void SetDeselectedColor()
    {
        _renderer.material.color = Color.white;
    }
}
