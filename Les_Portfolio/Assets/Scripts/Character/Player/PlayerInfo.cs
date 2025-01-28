using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    public Camera _mainCamera { get { return mainCamera; } }

    [Header("기본 Joystick 이동")]
    [SerializeField] PlayerMoveControl playerMoveControl;
    public PlayerMoveControl _playerMoveControl { get { return playerMoveControl; } }

    [Header("Touch 이동")]
    [SerializeField] PlayerTouchMoveControl playerTouchMoveControl;
    public PlayerTouchMoveControl _playerTouchMoveControl { get { return playerTouchMoveControl; } }

    [SerializeField] PlayerAniControl playerAniControl;
    public PlayerAniControl _playerAniControl { get { return playerAniControl; } }


    [SerializeField] // 테스트
    private GameObject player;
    public GameObject _player { get { return player; } }
    public PlayerData playerData { get; private set; }
    public CinemachineControl cinemachineControl { get; private set; }
    [HideInInspector] public NpcControl npcControl;

    private void Awake()
    {
        cinemachineControl = GameObject.FindGameObjectWithTag("Cinemachine").GetComponent<CinemachineControl>();
        CreatePlayer();
    }

    // Player 생성 및 초기 상태
    private void CreatePlayer()
    {
        playerMoveControl.SetPlayerSpeed(3f);//GameDataManager.Instance.player_Data[0].speed);

        playerAniControl.SetAnimator(player.GetComponent<Animator>());
        playerAniControl.AnimationChanger(PlayerAniState.Default);
        playerAniControl.SetMoveValue(0f);
    }

    #region Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            npcControl = other.GetComponent<NpcControl>();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            npcControl = null;
        }
    }
    #endregion
}
