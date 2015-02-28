using UnityEngine;
using System.Collections;

public class Remover : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        // If the player hits the trigger...
        if (col.gameObject.tag == "Player")
        {
            PlayerControl pc = col.gameObject.GetComponent<PlayerControl>();
            pc.Die();
        }
        else
        {
            Destroy(col.gameObject);
        }
    }
}
