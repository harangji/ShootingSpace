using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 플레이어의 물리적인 이동과 회전 로직을 전담하는 클래스입니다.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Title("이동 및 회전 설정")]
    [LabelText("정지 거리"), SuffixLabel("단위")]
    [SerializeField] private float stopDistance = 0.1f;

    [LabelText("감속 시작 거리"), SuffixLabel("단위")]
    [SerializeField] private float slowDistance = 2.0f;

    [LabelText("회전 속도"), SuffixLabel("도/초")]
    [SerializeField] private float rotationSpeed = 720f;

    [Title("실시간 상태")]
    [ReadOnly, LabelText("목표 지점")]
    [SerializeField] private Vector2 _targetPosition;

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
        if (UnityEngine.InputSystem.Mouse.current != null)
        {
            Vector2 mouseScreenPos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
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
            float speed = (_stats != null) ? _stats.MoveSpeed : 5f;
            if (distance < slowDistance) speed *= (distance / slowDistance);

            Vector2 direction = (_targetPosition - currentPos).normalized;
            transform.position += (Vector3)direction * (speed * Time.fixedDeltaTime);
        }
    }
}
