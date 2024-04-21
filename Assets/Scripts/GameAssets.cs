using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public GameObject Plant1Prefab;
    public GameObject Plant2Prefab;
    public GameObject Plant3Prefab;

}
