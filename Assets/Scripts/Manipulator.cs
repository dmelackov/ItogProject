using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manipulator : MonoBehaviour
{

    public float offset;
    public float height;
    public float l1;
    public float l2;
    public float l3;
    public GameObject target;

    public GameObject Arm1;
    public GameObject Arm2;
    public GameObject Arm3;
    public GameObject Arm4;
    public GameObject Arm5;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 coord = target.transform.position - this.transform.position;
        Vector2 coord_xz = new Vector2(coord.x, coord.z).normalized;
        Vector3 front = new Vector3(1, 0, 0);
        Vector2 front_xz = new Vector2(front.x, front.z).normalized;
        Quaternion target_;
        if (coord.z > 0)
        {
            target_ = Quaternion.Euler(-90, 0, -Mathf.Rad2Deg * (Mathf.Acos(Vector2.Dot(coord_xz, front_xz))));
        }
        else
        {
            target_ = Quaternion.Euler(-90, 0, Mathf.Rad2Deg * Mathf.Acos(Vector2.Dot(coord_xz, front_xz)));
        }


        Arm1.transform.rotation = target_;
    }
}
