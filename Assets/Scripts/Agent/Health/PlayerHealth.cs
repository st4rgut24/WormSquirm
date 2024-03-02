using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealth : AgentHealth
{
    public Slider slider;  // Reference to the UI Slider for the health bar

    public PlayerHealth(float maxHealth) : base(maxHealth)
    {
        slider = GameObject.Find("HealthBar").GetComponent<Slider>();
    }

    // Method to update the health bar with the current health value
    void UpdateHealthBar()
    {
        slider.value = currentHealth / maxHealth;
    }

    public override void TakeDamage(float damageAmount)
    {
        base.TakeDamage(damageAmount);

        UpdateHealthBar();
    }

    protected override void Heal(float healAmount)
    {
        base.Heal(healAmount);

        UpdateHealthBar();
    }
}

