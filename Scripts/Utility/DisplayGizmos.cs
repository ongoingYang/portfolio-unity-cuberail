using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayGizmos : MonoBehaviour {

    //public Transform target;
    public bool displayGizmos;

    [Range(0f, 50)]
    public float range;
    void OnDrawGizmosSelected()
    {
        if (!displayGizmos) return;
        DrawGizmos();
    }
    public void DrawGizmos()
    {
        Gizmos.color = Color.gray;
        

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * range);
        Gizmos.color = Color.HSVToRGB(0.05f, 0.9f, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position - transform.right * range);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * range);
        Gizmos.color = Color.HSVToRGB(0.7f, 0.9f, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position - transform.forward * range);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * range);
        Gizmos.color = Color.HSVToRGB(0.35f, 0.9f, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position - transform.up * range);

        //Gizmos.color = Color.gray;
        //Gizmos.DrawWireSphere(transform.position, range);
        //Gizmos.DrawLine(target.position, target.position + target.up * 10);

    }
}
