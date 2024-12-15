using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Whirlwind")]
public class WhirlwindSpell : Spell
{
    public GameObject whirlwindPrefab; // Prefab for the whirlwind
    public float liftHeight = 5f; // Height to lift the target
    public float duration = 3f; // Duration of the effect

    public override void Cast(GameObject caster, Vector3 targetPosition)
    {
        // Spawn the whirlwind at the target position
        GameObject whirlwind = Instantiate(
            whirlwindPrefab,
            caster.transform.position,
            Quaternion.identity
        );

        // Move the whirlwind to the target position
        whirlwind.GetComponent<Rigidbody>().velocity =
            (targetPosition - caster.transform.position).normalized * 10f;

        // Attach behavior to detect collision and apply effects
        WhirlwindBehavior behavior = whirlwind.AddComponent<WhirlwindBehavior>();
        behavior.liftHeight = liftHeight;
        behavior.duration = duration;
    }
}
