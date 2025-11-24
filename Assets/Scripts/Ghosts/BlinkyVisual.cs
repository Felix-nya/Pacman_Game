using System.ComponentModel;
using UnityEngine;

public class BlinkyVisual : MonoBehaviour
{
    private static readonly int Right = Animator.StringToHash(IsRight);
    private static readonly int Down = Animator.StringToHash(IsDown);
    private static readonly int Up = Animator.StringToHash(IsUp);
    private static readonly int Left = Animator.StringToHash(IsLeft);
    private static readonly int ExitFrightened = Animator.StringToHash(IsExitFrightened);
    private static readonly int Frightened = Animator.StringToHash(IsFrightened);

    private Animator animator;
    [SerializeField] private Blinky ghost;
    Vector2 _curDirection;
    bool _isFrightened;
    bool _isExitFrightenedVis;

    private const string IsRight = "IsRight";
    private const string IsDown = "IsDown";
    private const string IsUp = "IsUp";
    private const string IsLeft = "IsLeft";
    private const string IsExitFrightened = "IsExitFrightened";
    private const string IsFrightened = "IsFrightened";


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        SettingAnimations();
    }

    private void SettingAnimations()
    {
        _curDirection = ghost._currentDirection;
        _isFrightened = !ghost._CanEatPacman;
        _isExitFrightenedVis = ghost._ExitFrightened;
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
        if (_isFrightened)
        {
            if (_isExitFrightenedVis)
            {
                animator.SetBool(ExitFrightened, true);
                animator.SetBool(Frightened, false);
            }
            else
            {
                animator.SetBool(Frightened, true);
                animator.SetBool(ExitFrightened, false);
            }
        }
        else
        {
            animator.SetBool(ExitFrightened, false);
            animator.SetBool(Frightened, false);
        }
    }
}
