using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

public class SavingWrapper : MonoBehaviour
{
    const string defaultSaveFile = "save";
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Save();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Delete();
        }
    }

    private void Delete()
    {
        GetComponent<SavingSystem>().Delete(defaultSaveFile);
    }

    public void Load()
    {
        GetComponent<SavingSystem>().Load(defaultSaveFile);
        Debug.Log("Scene Loaded");
    }

    public void Save()
    {
        GetComponent<SavingSystem>().Save(defaultSaveFile);
    }
}

