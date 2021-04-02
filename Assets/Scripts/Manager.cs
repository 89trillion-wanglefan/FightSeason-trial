using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public GameObject UI;
    public GameObject Button;
    public ShowPlayerInfo PlayerCase; //用户数据显示
    private readonly List<SeasonGood> goodList = new List<SeasonGood>();//赛季奖励数据
    private Users user;//用户数据
    public ScrollRect scrollRect;//滚动框
    public RectTransform rectTransform;//滚动栏RectTransform，获取高度用
    [Header("Item的预制体")] public ShowRewards itemPrefab;
    public long userID = 3716954261;
    private RectTransform content; //滑动框的Content
    [SerializeField] private GridLayoutGroup layout; //布局组件

    private readonly List<ShowRewards> dataList = new List<ShowRewards>(); //实例列表
    private readonly List<ShowRewards> dataListOrigin = new List<ShowRewards>(); //保存原始顺序
    private int totalCount; //总的数据数量
    private int headIndex; //头下标
    private int tailIndex; //尾下标
    private Vector2 firstItemAnchoredPos; //第一个元素的坐标
    private int fixedCount; //显示元素数量

    private JSONNode rewardsInfo;//读取时用的变量
    private JSONNode userInfo;

    // Start is called before the first frame update
    void Start()
    {
        fixedCount = (int) (rectTransform.rect.size.y / (layout.spacing.y + layout.cellSize.y)) + 2;//计算需要的显示元素数量，减少开销
        UI.SetActive(false);//关闭排名UI
        StartCoroutine(Init());
    }

    /// <summary>
    /// 滚动时设置元素位置
    /// </summary>
    private void OnScroll()
    {
        //向下滚动
        while (content.anchoredPosition.y >=
               layout.padding.top + (headIndex + 1) * (layout.cellSize.y + layout.spacing.y)
               && tailIndex != totalCount - 1)
        {
            //将数据列表中的第一个元素移动到最后一个
            ShowRewards item = dataList[0];
            dataList.Remove(item);
            dataList.Add(item);

            //设置位置
            SetRewardPos(item, tailIndex + 1);
            //设置显示
            SetRewardInfo(item, tailIndex + 1);

            headIndex++;
            tailIndex++;
        }

        //向上滑
        while (content.anchoredPosition.y <= layout.padding.top + headIndex * (layout.cellSize.y + layout.spacing.y)
               && headIndex != 0)
        {
            //将数据列表中的最后一个元素移动到第一个
            ShowRewards item = dataList.Last();
            dataList.Remove(item);
            dataList.Insert(0, item);

            //设置位置
            SetRewardPos(item, headIndex - 1);
            //设置显示
            SetRewardInfo(item, headIndex - 1);

            headIndex--;
            tailIndex--;
        }
    }

    /// <summary>
    /// 初始化奖励列表
    /// </summary>
    private void InitRewardList()
    {
        for (int i = 0; i < fixedCount; i++)
        {
            ShowRewards tempItem = Instantiate(itemPrefab, content);
            dataList.Add(tempItem);
            dataListOrigin.Add(tempItem);
            SetRewardInfo(tempItem, i);
        }
    }
    
    /// <summary>
    /// 重置奖励列表内容，复用元素
    /// </summary>
    private void ReInitRewardList()
    {
        dataList.Clear();
        for (int i = 0; i < fixedCount; i++)
        {
            dataList.Add(dataListOrigin[i]);
            SetRewardPos(dataListOrigin[i], i);
            SetRewardInfo(dataListOrigin[i], i);
        }

        headIndex = 0;
        tailIndex = fixedCount - 1;
    }

    /// <summary>
    /// 获得首个元素的位置
    /// </summary>
    private void GetFirstItemAnchoredPos()
    {
        firstItemAnchoredPos = new Vector2
        (
            layout.padding.left + layout.cellSize.x / 2,
            -layout.padding.top - layout.cellSize.y / 2
        );
    }

    /// <summary>
    /// 设置奖励元素的位置
    /// </summary>
    /// 奖励显示框
    /// <param name="trans"></param>
    /// 奖励在奖励列表中的序列
    /// <param name="index"></param>
    private void SetRewardPos(ShowRewards trans, int index)
    {
        trans.rectTransform.anchoredPosition = new Vector2
        (
            firstItemAnchoredPos.x,
            index == 0
                ? firstItemAnchoredPos.y
                : firstItemAnchoredPos.y - index * (layout.cellSize.y + layout.spacing.y)
        );
    }

    /// <summary>
    /// 设置奖励元素的内容，并设定按钮
    /// </summary>
    /// 奖励显示框
    /// <param name="trans"></param>
    /// 奖励在奖励列表中的序列
    /// <param name="index"></param>
    private void SetRewardInfo(ShowRewards trans, int index)
    {
        trans.ShowRewardInfo(goodList[index], index, user.RewardsGot);
        trans.button.onClick.AddListener(delegate { GetReward(index, trans); });
    }

    /// <summary>
    /// 展示玩家信息
    /// </summary>
    /// 玩家信息显示框
    /// <param name="player"></param>
    private void SetUserInfo(ShowPlayerInfo player)
    {
        player.ShowUserInfo(this.user);
    }

    /// <summary>
    /// 开始按键
    /// </summary>
    public void ToUI() 
    {
        UI.SetActive(true);
        Button.SetActive(false);
    }

    /// <summary>
    /// 关闭按键
    /// </summary>
    public void Close()
    {
        UI.SetActive(false);
        Button.SetActive(true);
        content.localPosition = new Vector3(0f, 0f, 0f);
        ReInitRewardList();
    }

    /// <summary>
    /// 加分按键
    /// </summary>
    public void AddScore()
    { 
        user.Score += 100;
        if (user.Score > 6000) user.Score = 6000;
        PlayerCase.ShowUserInfo(this.user);
    }

    /// <summary>
    /// 注册给奖励按钮的方法，控制加分并设置已领取标签
    /// </summary>
    /// 领取的奖励在列表中的位置
    /// <param name="index"></param>
    /// 奖励显示框
    /// <param name="reward"></param>
    public void GetReward(int index, ShowRewards reward)
    {
        if ((1 & (user.RewardsGot >> index)) == 0)
        {
            if (user.Score >= goodList[index].ScoreRequired)
            {
                user.Coins += goodList[index].Reward;
                user.RewardsGot |= 1 << index;
                reward.score.text = "已领取";
            }
        }

        PlayerCase.ShowUserInfo(this.user);
    }

    /// <summary>
    /// 从json读取用户数据
    /// </summary>
    private void ReadPlayerInfo()
    {
        var streamers = new StreamReader(Application.dataPath + "/StreamingAssets/UserInfo.json"); //读取数据，转换成数据流
        var str = streamers.ReadToEnd();
        userInfo = JSON.Parse(str);
        var usernode = userInfo["User"];
        user = new Users(usernode["uid"].AsLong, usernode["nickName"], usernode["trophy"].AsInt,
            usernode["coins"].AsInt, 0);
    }

    /// <summary>
    /// 从json读取奖励数据
    /// </summary>
    private void ReadSeasonInfo()
    {
        var streamers = new StreamReader(Application.dataPath + "/StreamingAssets/SeasonRewards.json"); //读取数据，转换成数据流
        var str = streamers.ReadToEnd();
        rewardsInfo = JSON.Parse(str);
        var rewardsList = rewardsInfo["rewards"];
        for (int i = 0; i < rewardsList.Count; i++)
        {
            var node = rewardsList[i];
            SeasonGood good = new SeasonGood(i * 200 + 4000, node.AsInt);
            goodList.Add(good);
        }
    }

    /// <summary>
    /// 整体初始化，协程实现
    /// </summary>
    /// <returns></returns>
    IEnumerator Init()
    {
        ReadPlayerInfo();
        ReadSeasonInfo();
        totalCount = goodList.Count;
        content = scrollRect.content;
        scrollRect.onValueChanged.AddListener(v => OnScroll());
        //设置头下标和尾下标
        headIndex = 0;
        tailIndex = fixedCount - 1;

        //设置content大小
        content.sizeDelta = new Vector2(0, layout.padding.top + totalCount * (layout.cellSize.y + layout.spacing.y));

        //设置用户信息
        SetUserInfo(PlayerCase);
        //实例化Item
        InitRewardList();

        //得到第一个Item的锚点位置
        GetFirstItemAnchoredPos();
        yield break;
    }

    /// <summary>
    /// 新赛季按钮事件，重新读取奖励信息，调整分数和已领取奖励列表
    /// </summary>
    public void SetNewSeason()//新赛季
    {
        ReadSeasonInfo();
        if (user.Score > 4000) user.Score = (user.Score - 4000) / 2 + 4000;
        user.RewardsGot = 0;
        SetUserInfo(PlayerCase);
        ReInitRewardList();
    }
}