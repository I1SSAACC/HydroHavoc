using UnityEngine;
using Mirror;
using System.Collections;

public class WaterZone : NetworkBehaviour
{
    [SerializeField] private float _tickIntervalInSeconds;

    [SerializeField] private float _damageAmount;

    private Coroutine _coroutine;
    private WaitForSeconds _wait;

    private void Awake() =>
        _wait = new(_tickIntervalInSeconds);

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Health health))
            StartDamaging(health);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Health _))
            StopDamaging();
    }

    private void StartDamaging(Health health)
    {
        StopDamaging();
        _coroutine = StartCoroutine(ApplyDamageOverTime(health));
    }

    private void StopDamaging()
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
    }

    [Server]
    private IEnumerator ApplyDamageOverTime(Health player)
    {
        while (true)
        {
            yield return _wait;

            player.CmdTakeDamage(_damageAmount);
        }
    }
}