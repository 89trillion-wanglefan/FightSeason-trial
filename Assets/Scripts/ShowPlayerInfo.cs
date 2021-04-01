using UnityEngine;
using UnityEngine.UI;

public class ShowPlayerInfo : MonoBehaviour
{
    [SerializeField] private Text playername;
    [SerializeField] private Text coins;
    [SerializeField] private Text score;
    [SerializeField] private Image badge;
    
    /// <summary>
    /// 用户信息显示框显示信息调整
    /// </summary>
    /// 显示的用户信息
    /// <param name="user"></param>
    public void ShowUserInfo(Users user)
    {
        score.text = user.Score.ToString();
        playername.text = user.Nickname;
        coins.text = user.Coins.ToString();
        if (user.Score >= 4000)
        {
            badge.sprite =
                Resources.Load($"Images/rank/arenaBadge_{user.Score / 1000 + 1}", typeof(Sprite)) as Sprite;
            badge.color = new Color(255, 255, 255, 1);
        }
        else badge.color = new Color(255, 255, 255, 0);
    }
}