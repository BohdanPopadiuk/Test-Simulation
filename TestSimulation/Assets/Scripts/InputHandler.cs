using Interfaces;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private LayerMask agentMask;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, agentMask))
            {
                hitInfo.collider.GetComponent<IClickable>()?.OnMouseClick();
            }
        }
    }
}
