using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMainMenu : MonoBehaviour
{
   public void OK()
    {
        SceneManager.LoadSceneAsync(1);

        SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
        wrapper.Load();
    }
}
