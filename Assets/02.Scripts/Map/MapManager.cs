using TMPro;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    [SerializeField]
    public int[] stages = {3,0,1,0};
    public bool[] unlock;
    public int starSum = 0;
    public TextMeshProUGUI stars;
    public GameObject[] levelFlagUi;
    public GameObject[] level;

    private void Start()
    {
        unlock = new bool[MapManager.Instance.stages.Length + 1];
        UnlockNode();
        starSum = 0;
        Sum();
        Flip();
    }

    private void Flip()
    {
        for (int i = 1; i < stages.Length; i++)
        {
            if (stages[i - 1] > 0)
            {
                AniTEST Ani = level[i].GetComponent<AniTEST>();
                Ani.PlayChildAnimations();
                levelFlagUi[i].SetActive(true);
            }
        }
    }

    private void UnlockNode()
    {
        for (int i = 0; i < stages.Length; i++)
        {
            if (stages[i] > 0)
            {
                unlock[i + 1] = true;
            }
        }
    }

    private void Sum()
    {
        foreach (int Star in stages)
        {
            starSum += Star;
        }
        stars.text = "" + starSum;
    }
}
