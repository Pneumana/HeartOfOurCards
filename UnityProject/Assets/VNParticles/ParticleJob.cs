using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class ParticleJob : IJob
{
    private Vector3 _targetDirection;
    float _cooldown;
    float _speed;
    float _rotationSpeed;

    float _deltaTime;

    private uint _seed;

    private float _rotation;
    private Vector3 _position;

    private NativeArray<float> _coolDownResult;
    private NativeArray<Vector3> _directionResult;
    private NativeArray<Vector3> _positionResult;
    private NativeArray<Quaternion> _rotationResult;

    public ParticleJob(
        Vector3 targetDirection,
        float cooldown,
        float speed,
        float rotationSpeed,
        float deltaTime,
        uint seed,
        Quaternion rotation,
        Vector3 position,
        NativeArray<float> coolDownResult,
        NativeArray<Vector3> directionResult,
        NativeArray<Vector3> positionResult,
        NativeArray<Quaternion> rotationResult
        )
    {
        _targetDirection = targetDirection;
        _cooldown = cooldown;
        _speed = speed;
        _rotationSpeed = rotationSpeed;
        _deltaTime = deltaTime;
        _seed = seed;
        //_rotation = rotation;
        _position = position;

        _coolDownResult = coolDownResult;
        _directionResult = directionResult;
        _positionResult = positionResult;
        _rotationResult = rotationResult;
    }


    //these need to jump up when activated.

    // Update is called once per frame
    public void Execute()
    {
        UpdateTargetDirection();
        RotateTowardsTarget();
        SetPosition();
    }
    void UpdateTargetDirection()
    {
        HandleRandomDirection();



    }
    void HandleRandomDirection()
    {
        _cooldown -= _deltaTime;
        if (_cooldown <= 0)
        {
            var random = new Unity.Mathematics.Random(_seed);
            float angleChange = random.NextFloat(-90, 90);
            Quaternion rot = Quaternion.AngleAxis(angleChange, Vector3.forward);
            _targetDirection = rot * _targetDirection;
            _directionResult[0] = _targetDirection;
            Debug.LogWarning("cooldown expired, new direction is " + _targetDirection);
            _cooldown = random.NextFloat(1f, 5f);
        }
        _coolDownResult[0] = _cooldown;
    }
    void RotateTowardsTarget()
    {
        Quaternion targetRot = Quaternion.LookRotation(Vector3.forward, _targetDirection);
        //_rotation = Quaternion.RotateTowards(_rotation, targetRot, _rotationSpeed * _deltaTime);
        Debug.Log("new rotation of " + _rotation);
        //_rotationResult[0] = _rotation;
    }
    void SetPosition()
    {
        Vector3 positionChange = _rotation * Vector3.up * _speed * _deltaTime;
        _position = _position + positionChange;

        _positionResult[0] = _position;
    }
}
