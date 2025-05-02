using UnityEngine;
using Mirror;

public class WaterZone : NetworkBehaviour
{
    [SerializeField] private int _damageAmount = 5;
    private float damageTimer = 0f;
    [SerializeField] private float _damageInterval = 1f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out PlayerHealth playerHealth))
                playerHealth.CmdTakeDamage(_damageAmount);
        }
    }

    private void Update()
    {
        Collider[] playersInZone = Physics.OverlapSphere(transform.position, 1f);
        foreach (var player in playersInZone)
        {
            if (player.TryGetComponent(out PlayerHealth playerHealth))
            {
                damageTimer += Time.deltaTime;
                if (damageTimer >= _damageInterval)
                    playerHealth.CmdTakeDamage(_damageAmount);
            }
        }
    }
}