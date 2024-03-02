using UnityEngine;

public class AgentHealth
{
    public float maxHealth;  // Maximum health of the player
    public float currentHealth;  // Current health of the player

    public AgentHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
    }

    // Method to reduce player's health,
    // returns if the player's health has reduced to zero
    public virtual bool ReduceHealth(float damageAmount)
    {
        currentHealth -= damageAmount;

        // Ensure health doesn't go below zero
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        return currentHealth == 0f;
    }

    // Method to increase player's health
    public virtual void IncreaseHealth(float healAmount)
    {
        currentHealth += healAmount;

        // Ensure health doesn't exceed the maximum
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }
}
