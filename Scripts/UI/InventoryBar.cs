using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBar : MonoBehaviour {
    public Image[] item_slots;

    public void UpdateInvItems(int index, List<Item> inventory, int inv_count) {
        item_slots[0].sprite = inventory[P_action.NoramlizeIndex(index, inv_count)].sprite;
        item_slots[1].sprite = inventory[P_action.NoramlizeIndex(index+1, inv_count)].sprite;
        item_slots[2].sprite = inventory[P_action.NoramlizeIndex(index+2, inv_count)].sprite;
        item_slots[3].sprite = inventory[P_action.NoramlizeIndex(index-1, inv_count)].sprite;
        item_slots[4].sprite = inventory[P_action.NoramlizeIndex(index-2, inv_count)].sprite;
    }
}
