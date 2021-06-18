using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongMove : MonoBehaviour
{
    public Vector3 moveVector;
    public float time;

    // Start is called before the first frame update
    private void Start()
    {
        LeanTween.moveLocal(gameObject, gameObject.transform.position + moveVector, time).setLoopPingPong();
    }

    // Update is called once per frame
    private void Update()
    {
    }
}