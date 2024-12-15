using UnityEngine;

public class WhirlwindBehavior : MonoBehaviour
{
    public float liftHeight;
    public float duration;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Lift the player and disable their movement
            PlayerCharacter playerController = other.GetComponent<PlayerCharacter>();
            if (playerController != null)
            {
                playerController.DisableMovement(duration);
                Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
                if (playerRigidbody != null)
                {
                    playerRigidbody.velocity = Vector3.up * liftHeight;
                }

                // Destroy the whirlwind after applying the effect
                Destroy(gameObject);
            }
        }
    }
}
