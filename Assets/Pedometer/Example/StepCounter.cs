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
            OnStep(0, 0);

        }
        private void Update()
        {
            //OnStep(currentSteps, 0);
        }

        private void OnStep (int steps, double distance)
        {
            // Display the values // Distance in feet
            //currentSteps = steps;
            stepText.text = " Steps :" + steps.ToString();
            UnlockLevels(steps);
            distanceText.text = (distance * 3.28084).ToString("F2") + " ft";
        }

        private void UnlockLevels(int steps)
        {
            if (steps > stepsToUnlockLvl1)
            {
                levelButton1.isActive = true;
            }
            if (steps > stepsToUnlockLvl2)
            {
                levelButton2.isActive = true;
            }
            if (steps > stepsToUnlockLvl3)
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