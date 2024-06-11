using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;

public class PotOnStove : MonoBehaviour
{
    [Header("UI")]
    public GameObject canvas; // UI 캔버스
    public Slider cookingBar; // 요리 진행 바
    public GameObject ingredientUI; // 재료 UI

    [Space(10)]
    public GameObject pfxFire; // 불 이펙트

    [Header("State")]
    public bool isOnStove = false; // 냄비가 스토브 위에 있는지 여부
    public bool inSomething = false; // 냄비에 재료가 있는지 여부
    public float cookingTime; // 요리 시간

    private CancellationTokenSource _cts; // 비동기 작업 취소 토큰
    private bool pause = false; // 요리 일시 정지 여부
    private bool stateIsCooked = false; // 재료가 요리되었는지 여부

    public Sprite[] icons; // 재료 아이콘 배열

    private void Update()
    {
        // 팬 위에 음식이 있을 때 불을 켜고, 없으면 끕니다.
        pfxFire.SetActive(isOnStove && inSomething);

        // 요리 중인 상태 업데이트
        if (isOnStove && inSomething && !stateIsCooked)
        {
            UpdateCookingBarPosition();
            UpdateCookingBarValue();
            UpdateIsIngredientState();
        }

        // 요리가 완료되면 요리 진행 바를 비활성화합니다.
        if (stateIsCooked)
        {
            cookingBar.gameObject.SetActive(false);
        }
    }

    // 재료의 상태 업데이트
    private void UpdateIsIngredientState()
    {
        if (transform.childCount > 2)
        {
            stateIsCooked = transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Ingredient>().isCooked;
        }
    }

    // 요리 진행 바의 위치 업데이트
    private void UpdateCookingBarPosition()
    {
        cookingBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1, 0)); // 적절한 위치 조정
    }

    // 요리 진행 바의 값 업데이트
    private void UpdateCookingBarValue()
    {
        cookingBar.value = cookingTime;
    }

    // 요리를 시작합니다.
    public async void StartCooking(UnityAction EndCallBack = null)
    {
        if (_cts == null)
        {
            Debug.Log("start cooking");
            cookingBar.gameObject.SetActive(true);
            ClearTime();

            _cts = new CancellationTokenSource();
            StartCookingAsync(EndCallBack, _cts.Token).Forget();
        }
        else if (pause)
        {
            pause = false; // 일시 정지를 해제합니다.
        }
    }

    // 비동기적으로 요리를 시작합니다.
    private async UniTask StartCookingAsync(UnityAction EndCallBack = null, CancellationToken cancellationToken = default)
    {
        if (!inSomething)
        {
            pfxFire.SetActive(false); // 재료가 없으면 불을 끕니다.
            return;
        }

        // 요리 시간을 증가시키며 요리 상태를 업데이트합니다.
        while (cookingTime <= 1)
        {
            if (!inSomething || cancellationToken.IsCancellationRequested)
            {
                pfxFire.SetActive(false);
                return;
            }

            while (pause)
            {
                await UniTask.Yield(cancellationToken); // 일시 정지된 동안 대기합니다.
            }

            await UniTask.Delay(450, cancellationToken: cancellationToken); // 요리 시간이 증가하는 간격
            cookingTime += 0.25f;
        }

        Debug.Log("Cooking End");
        EndCallBack?.Invoke();
        OffSlider();

        pause = false;
        cookingTime = 0;
        _cts.Dispose(); // 취소 토큰 소스를 해제합니다.
        _cts = null;
    }

    // 요리 시간을 초기화합니다.
    private void ClearTime()
    {
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
        pause = false;
    }

    // 요리 진행 바를 비활성화합니다.
    public void OffSlider()
    {
        cookingBar.value = 0f;
        cookingBar.gameObject.SetActive(false);
        UpdateIngredientState();
        InstantiateUI();
    }

    // 재료의 상태를 업데이트합니다.
    private void UpdateIngredientState()
    {
        if (transform.childCount < 3) return;

        Ingredient ingredient = transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Ingredient>();
        ingredient.isCooked = true;
        ingredient.ChangeMesh(ingredient.type);
    }

    // 재료 UI를 생성합니다.
    private void InstantiateUI()
    {
        if (transform.childCount < 3) return;

        Ingredient ingredient = transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Ingredient>();
        GameObject madeUI = Instantiate(ingredientUI, Vector3.zero, Quaternion.identity, canvas.transform);
        madeUI.transform.GetChild(0).gameObject.SetActive(true);
        Image image = madeUI.transform.GetChild(0).GetComponent<Image>();
        image.sprite = GetIcon(ingredient.type);
        madeUI.GetComponent<IngredientUI>().target = ingredient.transform;
    }

    // 재료 타입에 따른 아이콘을 반환합니다.
    private Sprite GetIcon(Ingredient.IngredientType ingredientType)
    {
        switch (ingredientType)
        {
            case Ingredient.IngredientType.Fish:
                return icons[0];
            case Ingredient.IngredientType.Shrimp:
                return icons[1];
            case Ingredient.IngredientType.Tomato:
                return icons[2];
            case Ingredient.IngredientType.Lettuce:
                return icons[3];
            case Ingredient.IngredientType.Cucumber:
                return icons[4];
            case Ingredient.IngredientType.Potato:
                return icons[5];
            case Ingredient.IngredientType.Chicken:
                return icons[6];
            case Ingredient.IngredientType.SeaWeed:
                return icons[7];
            case Ingredient.IngredientType.Tortilla:
                return icons[8];
            case Ingredient.IngredientType.Rice:
                return icons[9];
            case Ingredient.IngredientType.Pepperoni:
                return icons[10];
            case Ingredient.IngredientType.Meat:
                return icons[11];
            case Ingredient.IngredientType.Dough:
                return icons[12];
            case Ingredient.IngredientType.Cheese:
                return icons[13];
            case Ingredient.IngredientType.SushiRice:
                return icons[9];
            case Ingredient.IngredientType.SushiFish:
                return icons[0];
            case Ingredient.IngredientType.SushiCucumber:
                return icons[4];
            case Ingredient.IngredientType.PizzaTomato:
                return icons[2];
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
