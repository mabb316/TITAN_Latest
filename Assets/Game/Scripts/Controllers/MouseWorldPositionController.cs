using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorldPositionController : MonoBehaviour
{
    public static MouseWorldPositionController Instance {get; private set;}

    [SerializeField] private Transform _mouseWorldPosRefTransform;
    [SerializeField] private LayerMask _groundLayer;
    private Vector3 _mouseWorldPosition;

    private void Awake()
    {
        if(Instance != null){
            Destroy(Instance);
        }
        Instance = this;
    }

    void Update()
    {
        CalculateMouseWorldPosition();
        MoveReference();
    }

    private void CalculateMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;
        if(Physics.Raycast(ray, out raycastHit, 40f, _groundLayer))
        {
            _mouseWorldPosition = raycastHit.point;
        }
    }

    private void MoveReference()
    {
        _mouseWorldPosRefTransform.position = _mouseWorldPosition;
    }

    public Transform GetCursorPosTransform()
    {
        return _mouseWorldPosRefTransform;
    }

}
