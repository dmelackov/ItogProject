using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public Transform[] item_slots;

    public GameObject[] items;
    void Start()
    {
        items = new GameObject[item_slots.Length];
    }

    public int get_free_item_slot()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null) continue;
            return i;
        }

        return -1;
    }

    public int get_slot_with_item()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null) continue;
            return i;
        }

        return -1;
    }

    public GameObject get_slot_item(int i)
    {
        return items[i];
    }

    public Transform get_slot_position(int i)
    {
        return item_slots[i];
    }

    public void GrabItem(int i) {
        items[i] = null;
    }

    public void PutItem(int i, GameObject item) {
        items[i] = item;
    }
}
