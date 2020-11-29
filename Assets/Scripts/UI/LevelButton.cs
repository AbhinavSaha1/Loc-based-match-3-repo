using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PedometerU.Tests;
using RPG.Saving;

public class LevelButton : MonoBehaviour,ISaveable
{
    public bool isActive;
    public Sprite activeSprite;
    public Sprite lockedSprite;
    private Image buttonImage;
    private Button myButton;
    private StepCounter stepCounter;

    public GameObject stepsNeededToUnlock= null;
    public Text stepsNeededText;
    public GameObject buttonWhenInactive;
    public Button buttonWhenActive;
    public Text levelText;
    public int level;
    public int sceneToLoad;
    // Start is called before the first frame update
    void Start()
    {
        buttonImage = GetComponent<Image>();
        myButton = GetComponent<Button>();
        stepCounter = FindObjectOfType<StepCounter>();
        ShowLevelText();
    }

    // Update is called once per frame
    void Update()
    {
        DecideSprite();
    }
    public void Play()
    {
        PlayerPrefs.SetInt("Current level", level - 1);
        SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
        if(wrapper!= null)
        {
            Debug.Log("Wrapper Available");
        }
        wrapper.Save();
        SceneManager.LoadSceneAsync(sceneToLoad);
    }

    private void DecideSprite()
    {
        if(isActive)
        {
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            wrapper.Save();
            buttonImage.sprite = activeSprite;
            myButton.enabled = true;
            levelText.enabled = true;
            buttonWhenActive.enabled = true;
            buttonWhenInactive.SetActive(false);
        }
        else
        {
            buttonImage.sprite = lockedSprite;
            myButton.enabled = false;
            levelText.enabled = false;
            buttonWhenActive.enabled = false;
            buttonWhenInactive.SetActive(true);
        }
    }

    private void ShowLevelText()
    {
        levelText.text = "" + level;
    }
    public void StepsNeededToUnlock()
    {
        stepsNeededToUnlock.SetActive(true);
        if(level==1)
        {
            stepsNeededText.text = "Steps needed to be unlocked: " + stepCounter.stepsToUnlockLvl1;
            Debug.Log("Level 1 txt");
        }
       else if (level == 2)
        {
            stepsNeededText.text = "Steps needed to be unlocked: " + stepCounter.stepsToUnlockLvl2;
            Debug.Log("Level 2 txt");
        }
        else if (level == 3)
        {
            stepsNeededText.text = "Steps needed to be unlocked: " + stepCounter.stepsToUnlockLvl3;
        }
        else if(level>3)
        {
            Debug.Log("Level not available");
        }
    }

    public object CaptureState()
    {
        return isActive;
    }

    public void RestoreState(object state)
    {
        isActive = (bool)state;
    }
}
