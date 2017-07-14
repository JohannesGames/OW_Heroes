using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_Genji_Swift_Strike_Target : MonoBehaviour
{
    public GameObject Genji;

    void Start()
    {
        Destroy(gameObject, 1);
        int layermask = 1 << 9;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, new Vector3(0,-1,0), out hit, 1, layermask)) //if there is terrain too close to the target move it accordingly
        {
            Vector3 pos = hit.point + hit.normal;
            transform.position = pos;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject == Genji)
        {
            Genji.GetComponent<Hero_Genji>().Swift_Striking = false;
            Destroy(gameObject);
        }
    }
}
