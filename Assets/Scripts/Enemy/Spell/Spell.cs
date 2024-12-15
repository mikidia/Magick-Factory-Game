using Unity.VisualScripting;
using UnityEngine;

public abstract class Spell : ScriptableObject
{
    public string spellName;
    public float cooldown;
    public float cooldownTimer = 0;

    public abstract void Cast(GameObject caster, Vector3 targetPosition);

    public bool IsReady()
    {
        return cooldownTimer <= 0;
    }

    public void UpdateCooldown(float deltaTime)
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= deltaTime;
        }
    }

    public void setCooldown()
    {
        cooldownTimer = cooldown;
    }
}
