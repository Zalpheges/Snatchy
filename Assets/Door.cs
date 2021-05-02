using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Transform[] children;

    void Start()
    {
        children = GetComponentsInChildren<Transform>();
    }

    public void Push()
    {
        children[1].gameObject.layer = 0;
        Destroy(children[2].gameObject);
    }
}
