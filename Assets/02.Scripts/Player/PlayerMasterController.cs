using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMasterController : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> playerList;

    [Header("Current Player")]
    public GameObject currentPlayer;

    [Header("Player Input")]
    public InputActionAsset inputActionAsset;

    [Header("particl")]
    public GameObject particl;

    private void Awake()
    {
        currentPlayer = playerList[1];
        SwitchPlayerComponent();
    }

    public void SwitchPlayerComponent()
    {
        currentPlayer.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        PlayerInput oldPlayerInput = null;
        if (currentPlayer.GetComponent<PlayerInput>() != null)
            oldPlayerInput = currentPlayer.GetComponent<PlayerInput>();

        if (oldPlayerInput != null)
        {
            Destroy(oldPlayerInput);
        }

        // 현재 플레이어의 인덱스를 가져온다.
        int currentIndex = playerList.IndexOf(currentPlayer);

        // 다음 플레이어의 인덱스를 계산한다.
        int nextIndex = (currentIndex + 1) % playerList.Count;



        if (particl != null)
        {
            GameObject particleInstance = Instantiate(particl, currentPlayer.transform.position, Quaternion.identity);
            particleInstance.transform.parent = gameObject.transform; // parentTransform은 넣고 싶은 부모 GameObject의 Transform입니다.
            SwitchParticle switchParticle = particleInstance.GetComponent<SwitchParticle>();
            switchParticle.SwitchPlayer(currentPlayer.transform, playerList[nextIndex].transform);
        }
            // 다음 플레이어를 currentPlayer로 설정한다.
            currentPlayer = playerList[nextIndex];

        PlayerInput playerInput = currentPlayer.AddComponent<PlayerInput>();

        // InputActionAsset 할당
        playerInput.actions = inputActionAsset;

        // 기본 맵 설정
        playerInput.defaultActionMap = "Player";

        // 모바일 맵 설정
        playerInput.defaultActionMap = "Mobile";

        // Behavior 설정 (예: Send Messages)
        playerInput.notificationBehavior = PlayerNotifications.SendMessages;

        // PlayerInput 활성화
        playerInput.ActivateInput();

        currentPlayer.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
    }
}
