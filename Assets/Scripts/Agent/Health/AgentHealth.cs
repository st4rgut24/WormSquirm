using UnityEngine;

public class AgentHealth
{
    public float maxHealth;  // Maximum health of the player
    protected float currentHealth;  // Current health of the player

    public AgentHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
    }

    // Method to reduce player's health
    public virtual void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        // Ensure health doesn't go below zero
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // Check if player is dead
        if (currentHealth == 0f)
        {
            // Perform actions when player is dead, e.g., game over
            // You can add your own logic here
            Debug.Log("Player is dead!");
        }
    }

    // Method to increase player's health
    protected virtual void Heal(float healAmount)
    {
        currentHealth += healAmount;

        // Ensure health doesn't exceed the maximum
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }
}
