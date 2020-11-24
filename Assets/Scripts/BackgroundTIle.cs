using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTIle : MonoBehaviour
{
    public int healthpoints;
    private SpriteRenderer sprite;
    private GoalManager goalManager;
    private void Start()
    {
        goalManager = FindObjectOfType<GoalManager>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(healthpoints <= 0)
        {
            if(goalManager!= null)
            {
                goalManager.CompareGoals(this.gameObject.tag);
                goalManager.UpdateGoals();
            }
            Destroy(this.gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        healthpoints -= damage;
        MakeLighter();
    }

    private void MakeLighter()
    {
        Color color = sprite.color;
        float newAlpha = color.a * 0.5f;
        sprite.color = new Color(color.r, color.g, color.b, newAlpha);
    }
}
