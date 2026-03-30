using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어의 이동 및 회전을 제어합니다.
/// 입력 수집은 Update에서, 실제 물리적 이동 및 회전 처리는 FixedUpdate에서 수행합니다.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float stopDistance = 0.1f;
    [SerializeField] private float slowDistance = 2.0f;
    [SerializeField] private float rotationSpeed = 720f;

    private Vector2 _targetPosition;
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        // 마우스 월드 좌표 업데이트 (입력 수집은 매 프레임 수행)
        if (Mouse.current != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, _mainCamera.nearClipPlane));
            _targetPosition = (Vector2)mouseWorldPos;
        }
    }

    private void FixedUpdate()
    {
        HandleRotation();
        HandleMovement();
    }

    /// <summary>
    /// 마우스 위치를 향해 부드러운 회전을 처리합니다.
    /// </summary>
    private void HandleRotation()
    {
        Vector2 lookDir = _targetPosition - (Vector2)transform.position;
        if (lookDir.sqrMagnitude > 0.001f)
        {
            // Y축 정면 기준 회전 보정 (-90도)
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            
            // FixedUpdate 주기에 맞게 deltaTime을 사용 (자동으로 fixedDeltaTime으로 동작)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// 마우스 위치를 향해 점진적으로 이동시키며 도착 시 감속을 처리합니다.
    /// </summary>
    private void HandleMovement()
    {
        Vector2 currentPos = transform.position;
        float distance = Vector2.Distance(currentPos, _targetPosition);

        if (distance > stopDistance)
        {
            float speed = moveSpeed;

            // 목표 거리에 근접 시 감속 처리
            if (distance < slowDistance)
            {
                speed = moveSpeed * (distance / slowDistance);
            }

            Vector2 direction = (_targetPosition - currentPos).normalized;
            
            // FixedUpdate 주기에 맞게 위치 갱신
            transform.position += (Vector3)direction * (speed * Time.fixedDeltaTime);
        }
    }
}
