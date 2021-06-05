using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Items;

namespace Assets.Scripts.Inventory
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Player Inventory")]
    public class PlayerInventory : ScriptableObject
    {

        public List<InventoryItem> myInventory = new List<InventoryItem>();

    }

}