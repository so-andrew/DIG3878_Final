using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public enum ItemType
    {
        Plant1,
        Plant2,
        Plant3,
        Decoration,
        Tool
    }

    public static int GetCost(ItemType itemType)
    {
        switch (itemType)
        {
            default:
            case ItemType.Plant1: return 50;
            case ItemType.Plant2: return 75;
            case ItemType.Plant3: return 100;
            case ItemType.Decoration: return 150;
            case ItemType.Tool: return 50;
        }
    }

    public static GameObject GetGameObject(ItemType itemType)
    {
        switch (itemType)
        {
            default:
            case ItemType.Plant1: return GameAssets.Instance.Plant1Prefab;
            case ItemType.Plant2: return GameAssets.Instance.Plant2Prefab;
            case ItemType.Plant3: return GameAssets.Instance.Plant3Prefab;
        }
    }
}
