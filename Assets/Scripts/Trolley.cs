using System;
using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;
using UnityEngine;

public class Trolley : MonoBehaviour
{

    public GameObject filling_prefab;
    public Transform[] path;
    public Transform[] item_slots;

    public GameObject[] items;

    public Transform forward;
    private int current_path_point = 0;

    public float available_error = 0.05f;
    public float speed = 3f;
    public float angularSpeed = 2f;

    public bool running = true;
    public bool waiting = false;
    public bool output = false;

    private int direction = 1;
    void Start()
    {
        items = new GameObject[item_slots.Length];
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 self = this.transform.position;
        Vector3 forward_v = (forward.position - this.transform.position).normalized;
        self.y = 0;
       
        forward_v.y = 0;
        if (running)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null) continue;
                items[i].transform.position = item_slots[i].position;
                items[i].transform.rotation = Quaternion.LookRotation(forward_v+100*Vector3.up, Vector3.up);
            }

            if (current_path_point < path.Length && current_path_point >= 0)
            {
                Vector3 target = path[current_path_point].position;
                target.y = 0;
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation((target - self), Vector3.up), Time.deltaTime * angularSpeed);
                this.transform.position += (target - self).normalized * Time.deltaTime * speed;
                if ((target - self).magnitude < available_error) current_path_point+=direction;
            }
            else
            {
                running = false;
            }
        }
        else
        {

            if (current_path_point >= path.Length)
            {
                if (output)
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                        Destroy(items[i]);
                    }
                    Array.Clear(items, 0, items.Length);
                }
                else
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                        Destroy(items[i]);
                    }
                    Array.Clear(items, 0, items.Length);
                    for (int i = 0; i < item_slots.Length; i++)
                    {
                        GameObject go = Instantiate(filling_prefab, item_slots[i].position, Quaternion.AngleAxis(90, Vector3.right));
                        items[i] = go;
                        print(go);
                    }
                }
                Notify();
            } else {
                waiting = true;
            }
        }
    }

    public void Notify()
    {
        direction *= -1;
        current_path_point += direction;
        running = true;
        waiting = false;
    }

    public int get_free_item_slot(){
        for (int i = 0; i < items.Length; i++)
        {
            if(items[i] != null) continue;
            return i;
        }

        return -1;
    }

    public int get_slot_with_item(){
        for (int i = 0; i < items.Length; i++)
        {
            if(items[i] == null) continue;
            return i;
        }

        return -1;
    }

    public GameObject get_slot_item(int i){
        return items[i];
    }

    public Transform get_slot_position(int i) {
        return item_slots[i];
    }

    public void GrabItem(int i) {
        items[i] = null;
    }

    public void PutItem(int i, GameObject item) {
        items[i] = item;
    }

}
