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
    public Transform target;

    public GameObject Arm1;
    public GameObject Arm2;
    public GameObject Arm3;
    public GameObject Arm4;
    public GameObject Arm5;

    private float smooth = 3f;

    public Table unprocessed_table;
    public Table processed_table;
    public Trolley output_trolley;
    public Trolley input_trolley;
    public Machine machine;

    private bool grabbed = false;
    private GameObject grabbed_object = null;

    public GameObject idle_target;
    public float available_error = 0.05f;
    private int task_id = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(grabbed){
            grabbed_object.transform.position = Arm5.transform.position - new Vector3(0, 1, 0) * l3;
        }
        if (target != null)
        {
            Vector3 coord = target.transform.position - this.transform.position;
            Vector2 coord_xz = new Vector2(coord.x, coord.z).normalized;
            Vector3 front = new Vector3(1, 0, 0);
            Vector2 front_xz = new Vector2(front.x, front.z).normalized;

            Vector2 obj = new Vector2(0, 0);
            Vector2 target_obj = new Vector2(new Vector2(coord.x, coord.z).magnitude, coord.y);

            Quaternion target_;
            if (coord.z > 0)
            {
                target_ = Quaternion.Euler(0, 0, -Mathf.Rad2Deg * (Mathf.Acos(Vector2.Dot(coord_xz, front_xz))));
            }
            else
            {
                target_ = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Acos(Vector2.Dot(coord_xz, front_xz)));
            }
            Arm1.transform.localRotation = Quaternion.Lerp(Arm1.transform.localRotation, target_, Time.deltaTime * smooth);

            Vector2 A = obj + height * new Vector2(0, 1);
            Vector2 B = target_obj + l3 * new Vector2(0, 1);
            Vector2 C = A + new Vector2(1, 0) * offset;

            float c = (B - C).magnitude;
            float b = l1;
            float a = l2;

            float alpha = Mathf.Acos((a * a + b * b - c * c) / (2 * a * b));
            float a2 = Mathf.PI - alpha;

            float beta = Mathf.Asin(Mathf.Sin(alpha) * a / c);
            float a1 = Mathf.Acos(Vector2.Dot(new Vector2(0, 1), (B - C).normalized)) - beta;

            float a3 = Mathf.PI - a1 - a2;

            Quaternion target_a1 = Quaternion.Euler(0, Mathf.Rad2Deg * a1, 0);
            Quaternion target_a2 = Quaternion.Euler(0, -90 + Mathf.Rad2Deg * a2, 0);
            Quaternion target_a3 = Quaternion.Euler(0, Mathf.Rad2Deg * a3, 0);

            Arm2.transform.localRotation = Quaternion.Lerp(Arm2.transform.localRotation, target_a1, Time.deltaTime * smooth);
            Arm3.transform.localRotation = Quaternion.Lerp(Arm3.transform.localRotation, target_a2, Time.deltaTime * smooth);
            Arm5.transform.localRotation = Quaternion.Lerp(Arm5.transform.localRotation, target_a3, Time.deltaTime * smooth);
        }

        if(input_trolley.waiting && input_trolley.get_slot_with_item() == -1) input_trolley.Notify();
        if(output_trolley.waiting && output_trolley.get_free_item_slot() == -1) output_trolley.Notify();

        if (task_id == 0) {
            target = idle_target.transform;
            if(machine.ready && processed_table.get_free_item_slot() != -1) {
                target = machine.item.transform;
                task_id = 4;
                return;
            }
            if(machine.waiting && unprocessed_table.get_slot_with_item() != -1) {
                int slot = unprocessed_table.get_slot_with_item();
                target = unprocessed_table.get_slot_item(slot).transform;
                task_id = 3;
                return;
            }
            if(output_trolley.waiting && processed_table.get_slot_with_item() != -1) {
                int slot;
                if((slot = output_trolley.get_free_item_slot()) != -1){
                    target = processed_table.get_slot_item(processed_table.get_slot_with_item()).transform;
                    task_id = 2;
                    return;
                }
                return;
            }
            if(unprocessed_table.get_free_item_slot() != -1 && input_trolley.waiting) {
                int slot;
                if((slot = input_trolley.get_slot_with_item()) != -1){
                    target = input_trolley.get_slot_item(slot).transform;
                    task_id = 1;
                    return;
                }
            }
        }
        if (task_id == 1) {
            if ((Arm5.transform.position - new Vector3(0, 1, 0) * l3 - target.transform.position).magnitude <= available_error) {
                int slot = input_trolley.get_slot_with_item();
                grabbed_object = input_trolley.get_slot_item(slot);
                input_trolley.GrabItem(slot);
                grabbed = true;
                int tabble_slot = unprocessed_table.get_free_item_slot();
                target = unprocessed_table.get_slot_position(tabble_slot);
                task_id = 11;
            }
        }
        if (task_id == 2) {
            if ((Arm5.transform.position - new Vector3(0, 1, 0) * l3 - target.transform.position).magnitude <= available_error) {
                int slot = processed_table.get_slot_with_item();
                grabbed_object = processed_table.get_slot_item(slot);
                processed_table.GrabItem(slot);
                grabbed = true;
                int tabble_slot = output_trolley.get_free_item_slot();
                target = output_trolley.get_slot_position(tabble_slot);
                task_id = 21;
            }
        }
        if (task_id == 3) {
            if ((Arm5.transform.position - new Vector3(0, 1, 0) * l3 - target.transform.position).magnitude <= available_error) {
                int slot = unprocessed_table.get_slot_with_item();
                grabbed_object = unprocessed_table.get_slot_item(slot);
                unprocessed_table.GrabItem(slot);
                grabbed = true;
                target = machine.slot;
                task_id = 31;
            }
        }
        if (task_id == 4) {
            if ((Arm5.transform.position - new Vector3(0, 1, 0) * l3 - target.transform.position).magnitude <= available_error) {
                grabbed_object = machine.item;
                machine.GrabItem();
                grabbed = true;
                int slot = processed_table.get_free_item_slot();
                target = processed_table.get_slot_position(slot);
                task_id = 41;
            }
        }
        if (task_id == 11) {
            if ((Arm5.transform.position - new Vector3(0, 1, 0) * l3 - target.transform.position).magnitude <= available_error) {
                int tabble_slot = unprocessed_table.get_free_item_slot();
                unprocessed_table.PutItem(tabble_slot, grabbed_object);
                grabbed_object = null;
                grabbed = false;
                task_id = 0;
            }
        }
        if (task_id == 21) {
            if ((Arm5.transform.position - new Vector3(0, 1, 0) * l3 - target.transform.position).magnitude <= available_error) {
                int tabble_slot = output_trolley.get_free_item_slot();
                output_trolley.PutItem(tabble_slot, grabbed_object);
                grabbed_object = null;
                grabbed = false;
                task_id = 0;
            }
        }
        if (task_id == 31) {
            if ((Arm5.transform.position - new Vector3(0, 1, 0) * l3 - target.transform.position).magnitude <= available_error) {
                machine.PutItem(grabbed_object);
                grabbed_object = null;
                grabbed = false;
                task_id = 0;
            }
        }
        if (task_id == 41) {
            if ((Arm5.transform.position - new Vector3(0, 1, 0) * l3 - target.transform.position).magnitude <= available_error) {
                int slot = processed_table.get_free_item_slot();
                processed_table.PutItem(slot, grabbed_object);
                grabbed_object = null;
                grabbed = false;
                task_id = 0;
            }
        }
    }
}
