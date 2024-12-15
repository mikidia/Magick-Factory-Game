using System.Collections.Generic;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    public List<Spell> spells;
    public Transform spellSpawnPoint;
    public float visionRange = 15f;
    public LayerMask playerLayer;

    private void Update()
    {
        foreach (Spell spell in spells)
        {
            spell.UpdateCooldown(Time.deltaTime);

            if (spell.IsReady() && PlayerInSight(out Vector3 playerPosition))
            {
                // Cast the spell at the last known player position
                spell.Cast(gameObject, playerPosition);
                spell.setCooldown();
                break; // Cast only one spell per frame
            }
        }
    }

    private bool PlayerInSight(out Vector3 playerPosition)
    {
        playerPosition = Vector3.zero;

        Collider[] players = Physics.OverlapSphere(transform.position, visionRange, playerLayer);
        if (players.Length > 0)
        {
            playerPosition = players[0].transform.position;
            return true;
        }

        return false;
    }
}
