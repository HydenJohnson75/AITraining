using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Script : MonoBehaviour
{

    [SerializeField] Animator activatorAnim;

    private bool shouldActivate;

    // Start is called before the first frame update
    void Start()
    {
        shouldActivate = false;
    }

    // Update is called once per frame
    void Update()
    {
        activatorAnim.SetBool("ShouldActivate", shouldActivate);
    }

    public void ActivateButton(bool activate)
    {
        shouldActivate = activate;
    }
}
