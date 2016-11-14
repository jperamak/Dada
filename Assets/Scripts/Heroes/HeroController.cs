using UnityEngine;
using System.Collections;

public class HeroController : MonoBehaviour {
   
    public bool IsFacingRight
    {
        get { return transform.localScale.x >= 0; }
    }
}
