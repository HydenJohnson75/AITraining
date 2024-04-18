using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentInputs : MonoBehaviour
{
    public float HorizontalAxis => _horizontalAxis;
    private float _horizontalAxis = 0f;

    public float VerticalAxis => _verticalAxis;
    private float _verticalAxis = 0f;

    public float HorizontalAxis2 => _horizontalAxis2;
    private float _horizontalAxis2 = 0f;

    public float VerticalAxis2 => _verticalAxis2;
    private float _verticalAxis2 = 0f;

    public bool IsShooting => isShooting;
    private bool isShooting = false;

    private void Update()
    {
        _horizontalAxis = Input.GetAxisRaw("Horizontal");
        _verticalAxis = Input.GetAxisRaw("Vertical");
        _horizontalAxis2 = Input.GetAxisRaw("Horizontal2");
        _verticalAxis2 = Input.GetAxisRaw("Vertical2");
        isShooting = Input.GetButton("Fire1");
    }
}
