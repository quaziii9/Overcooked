using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Ingredient;

public class CuttingBoard : MonoBehaviour
{
    private ObjectHighlight _parentObject;
    
    [Header("UI")]
    [SerializeField] private GameObject canvas;
    public Slider cookingBar; [SerializeField] private GameObject ingredientUI;

    [Space(10)]
    [SerializeField] Vector3 pos;
    public Coroutine CoTimer; 
    public bool Pause { get; set; }
    public float CuttingTime { get; set; }

    private PlayerInteractController _player1Controller;
    private Player2InteractController _player2Controller;
    private static readonly int StartCut = Animator.StringToHash("startCut");
    private static readonly int CanCut = Animator.StringToHash("canCut");
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
        _parentObject = transform.parent.GetComponent<ObjectHighlight>();
        _player1Controller = FindObjectOfType<PlayerInteractController>();
        _player2Controller = FindObjectOfType<Player2InteractController>();
    }

    private void Update()
    {
        UpdateCookingBarValue();
        ToggleKnife();
    }

    private void UpdateCookingBarPosition()
    {
        // 칼을 사용하는 UI 바의 위치를 업데이트
        if (_camera)
            cookingBar.transform.position = _camera.WorldToScreenPoint(_parentObject.transform.position + pos);
    }

    private void UpdateCookingBarValue()
    {
        // 칼질 진행 상태를 나타내는 UI 바의 값을 업데이트
        cookingBar.value = CuttingTime;
    }

    private void ToggleKnife()
    {
        // 객체가 활성화된 상태에 따라 칼 UI를 토글
        transform.GetChild(0).GetChild(0).gameObject.SetActive(!_parentObject.onSomething);
    }

    public void StartCutting1(UnityAction endCallBack = null)
    {
        // 플레이어 1의 칼질을 시작
        StartCutting(_player1Controller, endCallBack);
    }

    public void StartCutting2(UnityAction endCallBack = null)
    {
        // 플레이어 2의 칼질을 시작
        StartCutting(_player2Controller, endCallBack);
    }

    private void StartCutting(object playerController, UnityAction endCallBack = null)
    {
        Ingredient ingredient = transform.parent.parent.GetChild(2).GetChild(0).GetChild(0).GetComponent<Ingredient>();
        if (ingredient.currentState == IngredientState.Chopped) return;
        if (!_parentObject.onSomething) return;
        
        UpdateCookingBarPosition();
        
        cookingBar.gameObject.SetActive(true);
        ClearTime();
        CoTimer = StartCoroutine(CoStartCutting(endCallBack));
        
        switch (playerController)
        {
            case PlayerInteractController player1:
                player1.Animator.SetTrigger(StartCut);
                player1.Animator.SetBool(CanCut, true);
                break;
            case Player2InteractController player2:
                player2.Animator.SetTrigger(StartCut);
                player2.Animator.SetBool(CanCut, true);
                break;
        }
    }

    public void PauseSlider(bool pause)
    {
        // 칼질 진행을 일시 중지하거나 재개
        this.Pause = pause;
    }

    private IEnumerator CoStartCutting(UnityAction endCallBack = null)
    {
        // 칼질을 일정 시간 동안 진행
        while (CuttingTime <= 1)
        {
            while (Pause)
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.45f);
            SoundManager.Instance.PlayEffect("cut");
            CuttingTime += 0.25f;
        }
        endCallBack?.Invoke();
        OffSlider();
        CoTimer = null;
        Pause = false;
        CuttingTime = 0;
    }

    private void ClearTime()
    {
        // 칼질 타이머를 초기화
        if (CoTimer != null)
        {
            StopCoroutine(CoTimer);
            CoTimer = null;
        }
        Pause = false;
    }

    private void OffSlider()
    {
        // 칼질 진행 UI를 비활성화
        cookingBar.value = 0f;
        cookingBar.gameObject.SetActive(false);
        UpdateCanCutState(_player1Controller);
        UpdateCanCutState(_player2Controller);
        UpdateIngredientState();
        InstantiateUI();
    }

    private void UpdateCanCutState(object playerController)
    {
        // 플레이어의 칼질 가능 상태를 업데이트
        if (playerController is PlayerInteractController player1 && player1.interactObject == transform.parent.gameObject)
        {
            player1.Animator.SetBool("canCut", false);
        }
        else if (playerController is Player2InteractController player2 && player2.interactObject == transform.parent.gameObject)
        {
            player2.Animator.SetBool("canCut", false);
        }
    }

    private void UpdateIngredientState()
    {
        // 재료의 상태를 업데이트
        Ingredient ingredient = transform.parent.parent.GetChild(2).GetChild(0).GetChild(0).GetComponent<Ingredient>();
        ingredient.isCooked = true;

        if (ingredient.type is IngredientType.Meat or IngredientType.Chicken)
        {
            ingredient.isCooked = false;
            ingredient.currentState = IngredientState.Chopped;
        }

        ingredient.ChangeMesh(ingredient.type);
    }

    private void InstantiateUI()
    {
        // 재료 UI를 생성
        Ingredient ingredient = transform.parent.parent.GetChild(2).GetChild(0).GetChild(0).GetComponent<Ingredient>();
        GameObject madeUI = Instantiate(ingredientUI, Vector3.zero, Quaternion.identity, canvas.transform);

        madeUI.transform.GetChild(0).gameObject.SetActive(true);
        Image image = madeUI.transform.GetChild(0).GetComponent<Image>();
        image.sprite = IconManager.Instance.GetIcon(ingredient.type);
        madeUI.GetComponent<IngredientUI>().target = ingredient.transform;
    }
}
