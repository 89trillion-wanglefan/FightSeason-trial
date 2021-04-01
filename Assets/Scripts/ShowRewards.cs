using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class ShowRewards : MonoBehaviour
{
    [SerializeField] private Text playername;
    [SerializeField] public Text score;

    public RectTransform rectTransform;
    public Button button;

    /// <summary>
    /// 控制奖励显示框的显示信息
    /// </summary>
    /// 对应奖励信息
    /// <param name="good"></param>
    /// 奖励顺序
    /// <param name="index"></param>
    /// 已领取信息
    /// <param name="mark"></param>
    public void ShowRewardInfo(SeasonGood good, int index, int mark)
    {
        playername.text = good.ScoreRequired.ToString();
        transform.name = index.ToString();
        score.text = (1 & (mark >> index)) == 0 ? good.Reward.ToString() : "已领取";
    }
}