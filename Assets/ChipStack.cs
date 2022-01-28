using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipStack : MonoBehaviour
{

    [SerializeField] Renderer objRenderer;

    void Start()
    {
        //objRenderer = GetComponent<Renderer>();
    }
    public void SetColour(Color colour)
    {
        objRenderer.material = new Material(objRenderer.material);
        objRenderer.material.color = colour;

    }
}


