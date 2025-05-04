using System;
using System.Collections;
using UnityEngine;

public class Rutine
{
    private readonly MonoBehaviour _mono;
    private readonly Action _methodToUpdate;
    private Coroutine _coroutine;
    private bool _isOn;

    public Rutine(MonoBehaviour mono, Action methodToUpdate)
    {
        _mono = mono;
        _methodToUpdate = methodToUpdate;
    }

    public Rutine(Transform transform, Action methodToUpdate)
    {
        _mono = transform.GetComponent<MonoBehaviour>();
        _methodToUpdate = methodToUpdate;
    }

    public bool IsOn => _isOn;

    public void Start()
    {
        if (_isOn)
            return;

        _isOn = true;

        if (_mono == null)
            return;

        _coroutine = _mono.StartCoroutine(Updating());
    }

    public void Stop()
    {
        if (_coroutine == null)
            return;

        _mono.StopCoroutine(Updating());
        _coroutine = null;
        _isOn = false;
    }

    private IEnumerator Updating()
    {
        while (_isOn)
        {
            _methodToUpdate?.Invoke();

            yield return null;
        }
    }
}