using UnityEngine;
using Sirenix.OdinInspector;

public enum GameState
{
    Init,       // 초기 모드 선택 중
    Playing,    // 게임 진행 중
    Selection,  // 증강 선택 중 (일시정지)
    GameOver    // 게임 종료
}

/// <summary>
/// 게임의 전체 흐름과 모드를 관리하는 싱글톤 매니저입니다.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Title("Game State")]
    [ReadOnly] public GameState currentState = GameState.Init;
    
    [Title("Settings")]
    [SerializeField] private int killsToNextStage = 10; // 첫 스테이지 목표 처치 수
    [SerializeField] private float killMultiplier = 1.5f; // 다음 스테이지 난이도 상승폭

    [Title("Current Stats")]
    [ReadOnly] public int currentKills = 0;
    [ReadOnly] public int currentStage = 1;
    [ReadOnly] public int targetKills = 10;

    [Title("UI References")]
    [SerializeField] private AugmentSelectionUI selectionUI;
    [SerializeField] private GameObject modeSelectionUI; // 모드 선택 패널
    [SerializeField] private EnemySpawner spawner; // 스포너 참조 추가

    [Title("Player Reference")]
    [SerializeField] public Transform playerTransform;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.OnEnemyKilled += HandleEnemyKilled;
        }
        StartStageMode();
    }

    private void OnDisable()
    {
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.OnEnemyKilled -= HandleEnemyKilled;
        }
    }

    /// <summary>
    /// 모드 1 (스테이지 모드) 시작!
    /// </summary>
    [Button("Start Mode 1 (Stage Mode)")]
    public void StartStageMode()
    {
        currentState = GameState.Playing;
        currentStage = 1;
        currentKills = 0;
        targetKills = killsToNextStage;
        
        if (modeSelectionUI != null) modeSelectionUI.SetActive(false);
        
        // 스포너 시작
        if (spawner != null) spawner.StartStage(currentStage);

        Debug.Log("[GameManager] 모드 1: 스테이지 모드 시작!");
    }

    private void HandleEnemyKilled(EnemyBase enemy)
    {
        if (currentState != GameState.Playing) return;

        currentKills++;

        // 목표 달성 체크 (주석 처리: 스테이지 전환 중단)
        /*
        if (currentKills >= targetKills)
        {
            EnterSelectionPhase();
        }
        */
    }

    private void EnterSelectionPhase()
    {
        currentState = GameState.Selection;

        // 스폰 일시 중단
        if (spawner != null) spawner.StopSpawning();

        Debug.Log($"[GameManager] 스테이지 {currentStage} 클리어! 증강 선택 시작.");

        if (selectionUI != null)
        {
            selectionUI.ShowSelection(); // 증강 UI 띄우기
        }
    }

    /// <summary>
    /// 증강 선택 완료 후 호출될 메서드
    /// </summary>
    public void ResumeAfterSelection()
    {
        currentStage++;
        currentKills = 0;
        // 다음 스테이지 목표 설정
        targetKills = Mathf.RoundToInt(targetKills * killMultiplier);

        currentState = GameState.Playing;

        // 다음 스테이지 스폰 시작
        if (spawner != null) spawner.StartStage(currentStage);
        
        Debug.Log($"[GameManager] 스테이지 {currentStage} 시작! 목표 처치 수: {targetKills}");
    }
}
