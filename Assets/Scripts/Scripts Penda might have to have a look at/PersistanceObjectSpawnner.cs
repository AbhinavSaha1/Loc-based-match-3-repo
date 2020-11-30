using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistanceObjectSpawnner : MonoBehaviour
{
    [SerializeField] GameObject persistentObjectPrefab;

    static bool hasSpawned = false;

    private void Awake()
    {
        if (hasSpawned) return;

        SpawnPersistentObjects();

        hasSpawned = true;
    }

    private void SpawnPersistentObjects()
    {
        GameObject persistentObject = Instantiate(persistentObjectPrefab);
        DontDestroyOnLoad(persistentObject);
    }
}

