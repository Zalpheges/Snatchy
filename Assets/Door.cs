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
        children[1].GetComponent<PolygonCollider2D>().enabled = false;
        children[2].GetComponent<Animator>().SetTrigger("Ouverture");
        children[2].GetComponent<BoxCollider2D>().enabled = false;
    }
}
