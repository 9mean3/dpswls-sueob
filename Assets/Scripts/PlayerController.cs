using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Properties")]
    public float walkSpeed = 10f;
    public float gravity = 20f;
    public float jumpSpeed = 15f;
    public float doubleJumpSpeed = 10f;
    public float tripleJumpSpeed = 10f;
    public float xwallJumpSpeed = 15f;
    public float ywallJumpSpeed = 15f;

    [Header("Player Abilities")]
    public bool canDoubleJump;
    public bool canTripleJump;
    public bool canWallJump;

    //player state
    [Header("Player States")]
    public bool isJumping;
    public bool isDoubleJumping;
    public bool isTripleJumping;
    public bool isWallJumping;

    private bool _startJump;
    private bool _releaseJump;

    private Vector2 _input;
    private Vector2 _moveDirection;
    private CharactorController2D _charactorController;
    SpriteRenderer _spriteRenderer;


    private void Awake()
    {
        _charactorController = GetComponent<CharactorController2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (!isWallJumping)
        {
            _moveDirection.x = _input.x * walkSpeed;

            if (_moveDirection.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (_moveDirection.x < 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
        



        if (_charactorController.below)//on the ground
        {
            _moveDirection.y = 0f;
            isJumping = false;
            isDoubleJumping = false;
            isTripleJumping = false;
            isWallJumping = false;

            if (_startJump)
            {
                _startJump = false;
                _moveDirection.y = jumpSpeed;
                isJumping = true;
                _charactorController.DisableGroundCheck(0.1f);
            }
        }
        else// °øÁß¿¡...
        {
            if (_releaseJump)
            {
                _releaseJump = false;
                if (_moveDirection.y > 0)
                {
                    _moveDirection.y *= 0.5f;
                }
            }
            if (_startJump)
            {
                //tripleJump
                if (canTripleJump && !_charactorController.left && !_charactorController.right )
                {
                    if (isDoubleJumping && !isTripleJumping)
                    {
                        _moveDirection.y = tripleJumpSpeed;
                        isTripleJumping = true;
                    }
                }

                //doubleJump
                if (canDoubleJump && !_charactorController.left && !_charactorController.right)
                {
                    if (!isDoubleJumping)
                    {
                        _moveDirection.y = doubleJumpSpeed;
                        isDoubleJumping = true;
                    }
                }

                //wall jump
                if (canWallJump && (_charactorController.left || _charactorController.right))
                {
                    if (_charactorController.left && _moveDirection.x <= 0f) 
                    { 
                        _moveDirection.x = xwallJumpSpeed;
                        _moveDirection.y = ywallJumpSpeed;
                        transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    else if (_charactorController.right && _moveDirection.x >= 0f)
                    {
                        _moveDirection.x = -xwallJumpSpeed;
                        _moveDirection.y = ywallJumpSpeed;
                        transform.rotation = Quaternion.Euler(0, 180, 0);
                    }
                    isWallJumping = true;
                    StartCoroutine("WallJumpWaiter");
                }
                
                _startJump = false;
            }

            GravityCarculation();
            //_moveDirection.y -= gravity * Time.deltaTime;
        }

        _charactorController.Move(_moveDirection * Time.deltaTime);


    }

    void GravityCarculation()
    {
        if (_moveDirection.y > 0 && _charactorController.above)
            _moveDirection.y = 0f;
        _moveDirection.y -= gravity * Time.deltaTime;
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _startJump = true;
            _releaseJump = false;
        }
        else if (context.canceled)
        {
            _startJump = false;
            _releaseJump = true;
        }
    }

    IEnumerator WallJumpWaiter()
    {
        isWallJumping = true;
        yield return new WaitForSeconds(0.4f);
        isWallJumping = false;
    }
}
