using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealth : AgentHealth
{
    public Slider healthSlider;  // Reference to the UI Slider for the health bar

    public PlayerHealth(string sliderGoName, float maxHealth) : base(maxHealth)
    {
        healthSlider = GameObject.Find(sliderGoName).GetComponent<Slider>();
    }

    // Method to update the health bar with the current health value
    void UpdateHealthBar()
    {
        healthSlider.value = currentHealth / maxHealth;
    }

    public override bool ReduceHealth(float damageAmount)
    {
        bool isDead = base.ReduceHealth(damageAmount);

        UpdateHealthBar();

        return isDead;
    }

    public override void IncreaseHealth(float healAmount)
    {
        base.IncreaseHealth(healAmount);

        UpdateHealthBar();
    }
}

