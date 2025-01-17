using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using static Ingredient;

public class CuttingBoard_Net : MonoBehaviour
{
    private ObjectHighlight parentObject;
    public Slider cookingBar;
    [SerializeField] Vector3 pos;
    public Coroutine _CoTimer;
    public bool Pause = false;
    public float CuttingTime;

    [SerializeField] private GameObject Canvas;
    [SerializeField] private GameObject IngredientUI;
    [SerializeField] private Sprite[] Icons;

    private void Start()
    {
        parentObject = transform.parent.GetComponent<ObjectHighlight>();
    }

    private void Update()
    {
        UpdateCookingBarPosition();
        UpdateCookingBarValue();
        ToggleKnife();
    }

    private void UpdateCookingBarPosition()
    {
        cookingBar.transform.position = Camera.main.WorldToScreenPoint(parentObject.transform.position + pos);
    }

    private void UpdateCookingBarValue()
    {
        cookingBar.value = CuttingTime;
    }

    private void ToggleKnife()
    {
        transform.GetChild(0).GetChild(0).gameObject.SetActive(!parentObject.onSomething);
    }

    public void StartCutting1(GameObject player, UnityAction EndCallBack = null)
    {
        StartCutting(player.transform.GetComponent<PlayerInteractController_Net>(), EndCallBack);
    }

    private void StartCutting(PlayerInteractController_Net playerController, UnityAction EndCallBack = null)
    {
        if (parentObject.onSomething)
        {
            SoundManager.Instance.PlayEffect("cut");
            cookingBar.gameObject.SetActive(true);
            ClearTime();
            _CoTimer = StartCoroutine(CoStartCutting(playerController.transform.gameObject, EndCallBack));
            playerController.anim.SetTrigger("startCut");
            playerController.anim.SetBool("canCut", true);
        }
    }

    public void PauseSlider(bool pause)
    {
        Pause = pause;
    }

    private IEnumerator CoStartCutting(GameObject player, UnityAction EndCallBack = null)
    {
        while (CuttingTime <= 1)
        {
            while (Pause)
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.45f);
            CuttingTime += 0.25f;
            //SoundManager.instance.PlayEffect("cut");
        }
        EndCallBack?.Invoke();
        OffSlider(player);
        _CoTimer = null;
        Pause = false;
        CuttingTime = 0;
    }

    private void ClearTime()
    {
        if (_CoTimer != null)
        {
            StopCoroutine(_CoTimer);
            _CoTimer = null;
        }
        Pause = false;
    }

    public void OffSlider(GameObject player)
    {
        cookingBar.value = 0f;
        cookingBar.gameObject.SetActive(false);
        UpdateCanCutState(player.transform.GetComponent<PlayerInteractController_Net>());
        UpdateIngredientState();
        InstantiateUI();
    }

    private void UpdateCanCutState(PlayerInteractController_Net playerController)
    {
        if (playerController.interactObject == transform.parent.gameObject)
        {
            playerController.anim.SetBool("canCut", false);
        }
    }

    private void UpdateIngredientState()
    {
        Ingredient_Net Ingredient = transform.parent.parent.GetChild(2).GetChild(0).GetChild(0).GetComponent<Ingredient_Net>();
        Ingredient.isCooked = true;

        if (Ingredient.type == Ingredient_Net.IngredientType.Meat || Ingredient.type == Ingredient_Net.IngredientType.Chicken)
            Ingredient.isCooked = false;

        Ingredient.ChangeMesh(Ingredient.type);
    }

    public void OffAnim1()
    {

    }

    public void InstantiateUI()
    {
        Ingredient_Net Ingredient = transform.parent.parent.GetChild(2).GetChild(0).GetChild(0).GetComponent<Ingredient_Net>();
        GameObject madeUI = Instantiate(IngredientUI, Vector3.zero, Quaternion.identity, Canvas.transform);

        madeUI.transform.GetChild(0).gameObject.SetActive(true);
        Image image = madeUI.transform.GetChild(0).GetComponent<Image>();
        image.sprite = GetIcon(Ingredient.type);
        madeUI.GetComponent<IngredientUI>().target = Ingredient.transform;
    }

    private Sprite GetIcon(Ingredient_Net.IngredientType ingredientType)
    {
        switch (ingredientType)
        {
            case Ingredient_Net.IngredientType.Fish:
                return Icons[0];

            case Ingredient_Net.IngredientType.Shrimp:
                return Icons[1];

            case Ingredient_Net.IngredientType.Tomato:
                return Icons[2];

            case Ingredient_Net.IngredientType.Lettuce:
                return Icons[3];

            case Ingredient_Net.IngredientType.Cucumber:
                return Icons[4];

            case Ingredient_Net.IngredientType.Potato:
                return Icons[5];

            case Ingredient_Net.IngredientType.Chicken:
                return Icons[6];

            case Ingredient_Net.IngredientType.SeaWeed:
                return Icons[7];

            case Ingredient_Net.IngredientType.Tortilla:
                return Icons[8];

            case Ingredient_Net.IngredientType.Rice:
                return Icons[9];

            case Ingredient_Net.IngredientType.Pepperoni:
                return Icons[10];

            case Ingredient_Net.IngredientType.Meat:
                return Icons[11];

            case Ingredient_Net.IngredientType.Dough:
                return Icons[12];

            case Ingredient_Net.IngredientType.Cheese:
                return Icons[13];

            case Ingredient_Net.IngredientType.SushiRice:
                return Icons[9];

            case Ingredient_Net.IngredientType.SushiFish:
                return Icons[0];

            case Ingredient_Net.IngredientType.SushiCucumber:
                return Icons[4];

            case Ingredient_Net.IngredientType.PizzaTomato:
                return Icons[2];

            default:
                // 기본적으로 아무것도 하지 않음
                throw new ArgumentOutOfRangeException();
        }
    }
}
