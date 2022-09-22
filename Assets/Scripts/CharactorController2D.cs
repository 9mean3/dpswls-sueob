using GlobalType;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorController2D : MonoBehaviour
{
    public float raycastDistance = 0.2f;
    public LayerMask layerMask;

    public bool below;
    public GroundType groundType;

    public Vector2 _slopNormal;
    public float _slopAngle;
    public float _slopAngleLimit;

    private Vector2 _moveAmount;
    private Vector2 _currentPosition;
    private Vector2 _lastPosition;

    private Rigidbody2D _rigidbody;
    private CapsuleCollider2D _capsuleCollider;

    private bool _disableGroundCheck;

    private Vector2[] _raycastPosition = new Vector2[3];
    private RaycastHit2D[] _raycastHits = new RaycastHit2D[3];

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void FixedUpdate()
    {
        _lastPosition = _rigidbody.position;

        if(_slopAngle != 0 && below)
        {
            if((_moveAmount.x > 0f && _slopAngle > 0) || (_moveAmount.y < 0 && _slopAngle < 0))
            {
                _moveAmount.y = -Mathf.Abs(Mathf.Tan(_slopAngle * Mathf.Deg2Rad) * _moveAmount.x);
            }
        }

        _currentPosition = _lastPosition + _moveAmount;
        
        _rigidbody.MovePosition(_currentPosition);

        _moveAmount = Vector2.zero;

        if (!_disableGroundCheck) CheckGrounded();
    }

    public void Move(Vector2 movement)
    {
        _moveAmount += movement;
    }

    private void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.CapsuleCast(_capsuleCollider.bounds.center, _capsuleCollider.size, 
            CapsuleDirection2D.Vertical, 0f, Vector2.down, raycastDistance, layerMask);
        /*Vector2 raycastOrigin = _rigidbody.position - new Vector2(0, _capsuleCollider.size.y * 0.5f);

        _raycastPosition[0] = raycastOrigin + (Vector2.left * _capsuleCollider.size.x * 0.25f + Vector2.up * 0.1f);
        _raycastPosition[1] = raycastOrigin;
        _raycastPosition[2] = raycastOrigin + (Vector2.right * _capsuleCollider.size.x * 0.25f + Vector2.up * 0.1f);

        DrawDebugRays(Vector2.down, Color.green);

        int numberofGroundHits = 0;

        for (int i = 0; i < _raycastPosition.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(_raycastPosition[i], Vector2.down, raycastDistance, layerMask);

            if (hit.collider)
            {
                _raycastHits[i] = hit;
                numberofGroundHits++;
                groundType = DetermineGroundType(hit.collider);
                _slopNormal = hit.normal;
                _slopAngle = Vector2.SignedAngle(_slopNormal, Vector2.up);
            }
        }*/

        if (numberofGroundHits > 0)
        {
            if(_slopAngle>_slopAngleLimit || _slopAngle > -_slopAngleLimit)
            {
                below = false;
            }
            below = true;
        }
        else
        {
            below = false;
            groundType = GroundType.none;
        }

        System.Array.Clear(_raycastHits, 0, _raycastHits.Length);
    }

    private GroundType DetermineGroundType(Collider2D collider)
    {
        if (collider.GetComponent<GroundEffacter>())
        {
            GroundEffacter groundEffacter = collider.GetComponent<GroundEffacter>();
            return groundEffacter.groundType;
        }
        else return GroundType.LevelGeom;
    }

    private void DrawDebugRays(Vector2 direction, Color color)
    {
        for (int i = 0; i < _raycastPosition.Length; i++)
        {
            Debug.DrawRay(_raycastPosition[i], direction * raycastDistance, color);
        }
    }

    public void DisableGroundCheck(float delayTime)
    {
        below = false;
        _disableGroundCheck = true;
        StartCoroutine("EnableGroundCheck",delayTime);
    }
    IEnumerator EnableGroundCheck(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        _disableGroundCheck = false;
    }
}
