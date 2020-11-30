/* 
*   Pedometer
*   Copyright (c) 2018 Yusuf Olokoba
*/
using RPG.Saving;

namespace PedometerU.Tests {

    using UnityEngine;
    using UnityEngine.UI;


    public class StepCounter : MonoBehaviour, ISaveable
    {
        public int currentSteps;
        public int stepDifference;
        public Text stepText, distanceText;
        private Pedometer pedometer;
        public LevelButton levelButton1;
        public LevelButton levelButton2;
        public LevelButton levelButton3;
        public int stepsToUnlockLvl1= 30;
        public int stepsToUnlockLvl2 = 60;
        public int stepsToUnlockLvl3= 90;

        private void Awake()
        {
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            wrapper.Load();

        }

        private void Start () {
            // Create a new pedometer
            pedometer = new Pedometer(OnStep);
            // Reset UI
           
            OnStep(currentSteps, 0);
          

        }
        private void Update()
        {
            //OnStep(0, 0);
           
        }

        private void OnStep (int steps, double distance)
        {
            // Display the values // Distance in feet
            stepDifference = currentSteps - steps;
            currentSteps =((currentSteps+ steps)- stepDifference)/2;
            stepText.text = " Steps: " + currentSteps.ToString();
            UnlockLevels();
            distanceText.text = (distance * 3.28084).ToString("F2") + " ft";
        }

        private void UnlockLevels()
        {
            if (currentSteps > stepsToUnlockLvl1)
            {
                levelButton1.isActive = true;
            }
            if (currentSteps > stepsToUnlockLvl2)
            {
                levelButton2.isActive = true;
            }
            if (currentSteps > stepsToUnlockLvl3)
            {
                levelButton3.isActive = true;
            }
        }

        private void OnDisable () {
            // Release the pedometer
            pedometer.Dispose();
            pedometer = null;
        }

        public object CaptureState()
        {
           return currentSteps;
        }

        public void RestoreState(object state)
        {
            currentSteps = (int)state;
        }

    }
}