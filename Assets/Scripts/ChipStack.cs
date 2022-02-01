using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ChipStack : MonoBehaviourPunCallbacks
{

    [SerializeField] public Renderer objRenderer;

    void Start()
    {
        //objRenderer = GetComponent<Renderer>();
    }
    public void SetColour(Color colour)
    {
        objRenderer.material = new Material(objRenderer.material);
        objRenderer.material.color = colour;
    }

    public void SetMaterial(Material _mat)
    {
        if(objRenderer.material != _mat)
            objRenderer.material = _mat;
    }
    public void ToggleGameObj(bool isOn) {
        gameObject.SetActive(isOn);
    }
    

}


