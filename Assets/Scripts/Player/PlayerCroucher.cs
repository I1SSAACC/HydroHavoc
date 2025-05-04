using UnityEngine;

public class PlayerCroucher
{
    private const float ControllerResizeSpeed = 4f;
    private const float ModelRepositionSpeed = 4f;

    private readonly Rutine _rutine;
    private readonly CharacterController _controller;
    private readonly Transform _model;

    private readonly float _valueRisingController;
    private readonly float _valueSneackingController;
    private readonly float _sneackingPositionModel;
    private readonly float _risingPositionModel;

    private float _controllerTarget;
    private float _modelTarget;
    private bool _iSneacking;

    public bool IsCrouching => _iSneacking;

    public PlayerCroucher(Transform transform)
    {
        _rutine = new(transform.GetComponent<MonoBehaviour>(), UpdateSize);
        _controller = transform.GetComponent<CharacterController>();
        _model = transform.GetComponentInChildren<PlayerModel>().transform;

        _valueRisingController = _controller.height;
        _valueSneackingController = _valueRisingController * PlayerParams.CrouhingHeightMultiplier;
        _risingPositionModel = 0;
        _sneackingPositionModel = _risingPositionModel + _valueSneackingController * 0.5f;
    }

    public void EnableCrouching() =>
        StartRutine(true);

    public void DisableCrouching() =>
        StartRutine(false);

    private void StartRutine(bool isSneacking)
    {
        _iSneacking = isSneacking;
        _controllerTarget = isSneacking ? _valueSneackingController : _valueRisingController;
        _modelTarget = isSneacking ? _sneackingPositionModel : _risingPositionModel;

        _rutine.Start();
    }

    private void UpdateSize()
    {
        if (_controller.height == _controllerTarget && _model.localPosition.y == _modelTarget)
        {
            _rutine.Stop();

            return;
        }

        _controller.height = Mathf.MoveTowards(_controller.height, _controllerTarget, ControllerResizeSpeed * Time.deltaTime);

        Vector3 tempPosition = _model.localPosition;
        tempPosition.y = Mathf.MoveTowards(tempPosition.y, _modelTarget, ModelRepositionSpeed * Time.deltaTime);
        _model.localPosition = tempPosition;
    }
}