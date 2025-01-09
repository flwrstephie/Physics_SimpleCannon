using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonLauncher : MonoBehaviour
{
    [SerializeField] private Rigidbody _projectilePrefab;
    [SerializeField] private Transform _launchPoint;

    [Range(0, 90)]
    [SerializeField] private float _angle;
    [SerializeField] private float _power;

    private Vector2 _initialVelocity;
    private Vector2 _initialPosition;
    private float _time;
    private bool _isLaunching = false;

    public static event Action LaunchVisualEvent;

    private void Launch(Rigidbody projectileInstance)
    {
        LaunchVisualEvent?.Invoke();

        float angleInRadians = _angle * Mathf.PI / 180;
        _initialVelocity = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)) * _power;
        _initialPosition = new Vector2(projectileInstance.position.x, projectileInstance.position.y);

        StartCoroutine(HandleProjectileMovement(projectileInstance));
    }

    private float KinematicEquation(float acceleration, float velocity, float position, float time)
    {
        return (0.5f * acceleration * time * time) + (velocity * time) + position;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_isLaunching)
        {
            Rigidbody projectileInstance = Instantiate(_projectilePrefab, _launchPoint.position, Quaternion.identity);
            _isLaunching = true; 
            Launch(projectileInstance);
        }
    }

    private IEnumerator HandleProjectileMovement(Rigidbody projectileInstance)
    {
        _time = 0;
        while (true)
        {
            _time += Time.deltaTime;

            float newProjectileX = KinematicEquation(0, _initialVelocity.x, _initialPosition.x, _time);
            float newProjectileY = KinematicEquation(-9.81f, _initialVelocity.y, _initialPosition.y, _time);

            projectileInstance.position = new Vector3(newProjectileX, Mathf.Max(newProjectileY, 0), projectileInstance.position.z);

            if (newProjectileY <= 0) break;

            yield return null;
        }

        float distanceTravelled = Mathf.Abs(projectileInstance.position.x - _initialPosition.x);
        Debug.Log($"Distance Travelled: {distanceTravelled:F2}m");

        Destroy(projectileInstance.gameObject);
        _isLaunching = false; 
    }
}
