using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct ParticleJob : IJob
{
    private Vector3 _targetDirection;


    float _deltaTime;

    private NativeArray<uint> _seed;

    private NativeArray<Vector2> _positionInput;
    private NativeArray<Vector2> _directionInput;

    private NativeArray<Vector2> _directionResult;
    private NativeArray<Vector2> _positionResult;

    public ParticleJob(
        Vector3 targetDirection,
        //float rotationSpeed,
        float deltaTime,
        NativeArray<uint> seed,
        NativeArray<Vector2> positionInput,
        NativeArray<Vector2> directionResult,
        NativeArray<Vector2> positionResult,
        NativeArray<Vector2> directionInput
        )
    {
        _targetDirection = targetDirection;
        //_rotationSpeed = rotationSpeed;
        _deltaTime = deltaTime;
        _seed = seed;
        //_rotation = rotation;
        _positionInput = positionInput;

        //_coolDownResult = coolDownResult;
        _directionResult = directionResult;
        _positionResult = positionResult;
        _directionInput = directionInput;
        //_rotationResult = rotationResult;
    }


    //these need to jump up when activated.

    // Update is called once per frame
    public void Execute()
    {
        Loop();
    }
    void Loop()
    {


        for (int i = 0; i < _positionInput.Length; i++)
        {

            //_position = _matrixInput[i].GetRow(0);
            HandleRandomDirection(i);
            RotateTowardsTarget(i);
            //_rotation = Quaternion.Euler(_matrixInput[i].GetRow(1));
            //Debug.LogWarning("position is " + _position);
            //UpdateTargetDirection();
            //RotateTowardsTarget();


            //Debug.LogWarning("position is now " + _position);
            // Build matrix.
            SetPosition(i);

/*            var rot = Quaternion.Euler(Vector3.zero);
            var mat = Matrix4x4.TRS(_positionResult[i], rot, Vector3.one);*/
            //_matrixResult[i] = mat;
            //_rotationResult[0] = _rotation;

        }
    }
    void HandleRandomDirection(int i)
    {
                /*var random = new Unity.Mathematics.Random(_seed[i]);
                float angleChange = random.NextFloat(-90, 90);
                float rot = random.NextFloat(-180, 180);
                _targetDirection[i] = rot * _targetDirection[i];
                //look at disruptor
                Vector3 direction = pos - _position[i];
                _velocity[i] = Vector3.Lerp(_velocity[i], (Vector3.up) - (new Vector3(direction.x, 0, direction.z) * 3), 5 * _deltaTime);
                _directionResult[i] = _targetDirection[i];
                _speed[i] = 1;*/

    }
    void RotateTowardsTarget(int i)
    {
        var velocity = _directionInput[i] + Vector2.down;

        //Debug.Log(realTarget + " is lerped between " + _gravity + " and " + _targetDirection[i]);
        //_targetDirection[i] = rot * _targetDirection[1];
        //Debug.DrawLine(_position[i], _position[i] + realTarget[i], Color.green, Time.deltaTime);
        //Quaternion targetRot = Quaternion.LookRotation(Vector3.forward, _targetDirection[i]);
        _directionResult[i] = velocity;
        //var Rot = Quaternion.LookRotation(targetRot * Vector3.forward, _gravity);
        //_rotation[i] = Quaternion.RotateTowards(_rotation[i], targetRot, _rotationSpeed * _deltaTime);

        //Debug.Log("new rotation of " + _rotation);
        //_rotationResult[i] = _rotation[i];
    }
    void SetPosition(int i)
    {
        Vector2 positionChange = _directionResult[i] * _deltaTime;
        //_position[i] = _position[i] + positionChange;
        //Debug.Log("position changed by " + positionChange);
        _positionResult[i] = _positionInput[i] + positionChange;
    }
}
