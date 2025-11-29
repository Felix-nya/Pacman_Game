using UnityEngine;

public class PacmanVisual : MonoBehaviour
{
    private static readonly int Right = Animator.StringToHash(IsRight);
    private static readonly int Down = Animator.StringToHash(IsDown);
    private static readonly int Up = Animator.StringToHash(IsUp);
    private static readonly int Left = Animator.StringToHash(IsLeft);
    private static readonly int Death = Animator.StringToHash(IsDeath);

    private Animator animator;
    private Vector2 _curDirection;
    private bool _isDeath = false;
    private bool _isMoving = true;

    private const string IsRight = "IsRight";
    private const string IsDown = "IsDown";
    private const string IsUp = "IsUp";
    private const string IsLeft = "IsLeft";
    private const string IsDeath = "IsDeath";


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        SettingAnimations();
        if (!_isMoving && !_isDeath)
        {
            PauseAnimation();
        }
        else
        {
            ResumeAnimation();
        }
    }

    private void SettingAnimations()
    {
        _curDirection = Player.Instance._currentDirection;
        _isDeath = Player.Instance._isDeath;
        _isMoving = Player.Instance._isMoving;

        if (_isDeath)
        {
            animator.SetBool(Death, true);
        }
        else
        {
            animator.SetBool(Death, false);
        }

        if (_curDirection == Vector2.right)
        {
            animator.SetBool(Right, true);
            animator.SetBool(Down, false);
            animator.SetBool(Up, false);
            animator.SetBool(Left, false);
        }
        else if (_curDirection == Vector2.left)
        {
            animator.SetBool(Left, true);
            animator.SetBool(Right, false);
            animator.SetBool(Up, false);
            animator.SetBool(Down, false);
        }
        else if (_curDirection == Vector2.up)
        {
            animator.SetBool(Up, true);
            animator.SetBool(Right, false);
            animator.SetBool(Left, false);
            animator.SetBool(Down, false);
        }
        else
        {
            animator.SetBool(Down, true);
            animator.SetBool(Right, false);
            animator.SetBool(Up, false);
            animator.SetBool(Left, false);
        }
    }

    private void PauseAnimation()
    {
        animator.speed = 0f;
    }

    private void ResumeAnimation()
    {
        animator.speed = 1f;
    }

}
