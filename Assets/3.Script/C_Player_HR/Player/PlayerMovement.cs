using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어의 물리적인 이동과 회전 로직을 전담하는 클래스입니다.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float stopDistance = 0.1f;
    [SerializeField] private float slowDistance = 2.0f;
    [SerializeField] private float rotationSpeed = 720f;

    private Vector2 _targetPosition;
    private Camera _mainCamera;
    private PlayerStatsManager _stats;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _stats = GetComponent<PlayerStatsManager>();
    }

    private void Update()
    {
        UpdateTargetPosition();
    }

    private void UpdateTargetPosition()
    {
        if (Mouse.current != null)
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 worldPos = _mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, -_mainCamera.transform.position.z));
            _targetPosition = (Vector2)worldPos;
        }
    }

    private void FixedUpdate()
    {
        HandleRotation();
        HandleMovement();
    }

    private void HandleRotation()
    {
        Vector2 lookDir = _targetPosition - (Vector2)transform.position;
        if (lookDir.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void HandleMovement()
    {
        Vector2 currentPos = transform.position;
        float distance = Vector2.Distance(currentPos, _targetPosition);

        if (distance > stopDistance)
        {
            float speed = _stats.MoveSpeed;
            if (distance < slowDistance) speed *= (distance / slowDistance);

            Vector2 direction = (_targetPosition - currentPos).normalized;
            transform.position += (Vector3)direction * (speed * Time.fixedDeltaTime);
        }
    }
}
