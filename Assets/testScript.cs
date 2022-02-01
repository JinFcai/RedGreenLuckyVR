using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    public GameObject aObj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(aObj.transform.position - new Vector3(1, 1, 1));

        if (Vector3.Distance(aObj.transform.position, new Vector3(0, 0, 0)) > 5)
            aObj.transform.position = new Vector3(0, 0, 0);
        else
            aObj.transform.position += new Vector3(1, 1, 1);
    }
}
