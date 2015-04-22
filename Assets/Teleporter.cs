using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour {

    public Transform OtherEnd;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (OtherEnd != null)
            other.gameObject.transform.position = new Vector3(OtherEnd.position.x, other.transform.position.y, other.transform.position.z);
    }

}
