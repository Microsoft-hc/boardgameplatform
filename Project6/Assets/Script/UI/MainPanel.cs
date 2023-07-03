using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour
{
    public static MainPanel Instance;

    public Button AddBtn;
    public Button BackBtn;
    public Button BackToMain;
    public Button AckUpdate;
    public Transform Group;
    public GameObject UpdateGIntro_Panel;
    public Dropdown GameTypeDropdown;//游戏类型
    public Text GameTypetext_New;
    public Text GameTypetext_Old;
    public Text GameIntro_Old;//原游戏介绍    
    public InputField InputField3;//新游戏介绍
    public InputField InputField;//修改后的评论点赞数
    public bool updateflag_for_comment= true;
    public Transform Group1;//评论的滑动框
    private string Level;//用户等级
    public   static bool updateflag_for_ShowGame=true;

    private void Awake()
    {
        Instance = this;
        AddBtn.onClick.AddListener(() => { UIMgr.Instance.ShowUI(nameof(UploadPanel), 1); });
        BackBtn.onClick.AddListener(() => { UIMgr.Instance.HideUI(nameof(MainPanel)); });
        BackToMain.onClick.AddListener(() => { UpdateGIntro_Panel.SetActive(false); });
        
    }

    [Obsolete]
    private void OnEnable()
    {
        UpdateGIntro_Panel.SetActive(false);
        StartCoroutine(ShowAllGame());
    }

    // private void Update()
    // {
    //     if(updateflag_for_ShowGame)
    //     {
    //         ShowAllGame();
    //     }
    // }//

    [Obsolete]
    public IEnumerator ShowAllGame()
    {
        updateflag_for_ShowGame =false;
        foreach (Transform item in Group)
        {
            GameObject.Destroy(item.gameObject);
        }
        yield return null;
        var mySqlTools = new SqlHelper();
        mySqlTools.Open();
        var ds = mySqlTools.Select("imagelist");
        
        var ds1 = mySqlTools.SelectWhere("usertable",new[]{ "Account" },new[]{"="},new[]{LoginPanel.UserID.ToString()});


        var c= MysqlTools.TableData(ds);
        var c1=MysqlTools.GetValue(ds1,"Level");
        Level = c1.ToString();
        
        foreach (var item in c)
        {
            if(LoginPanel.UserID.ToString() == item["ByUser"].ToString() || Level == "1" )
            {
                GameObject go = Instantiate(Resources.Load<GameObject>("GameItem"));

                go.transform.Find("Image/Text").GetComponent<Text>().text = item["GameName"].ToString();

                StartCoroutine(Download($"{item["GUID"]}.png",go.transform.Find("Icon").GetComponent<Image>()));

                Button Delete = go.transform.Find("Delete").GetComponent<Button>();
                Button Icon = go.transform.Find("Icon").GetComponent<Button>();

                go.transform.SetParent(Group);
                go.transform.localScale = new Vector3(1, 1, 1);

                Delete.onClick.AddListener(() =>
                {
                    mySqlTools.Open();
                    mySqlTools.Delete("imagelist", new[]{"GUID"}, new[]{"="}, new[]{item["GUID"].ToString()});
                    mySqlTools.Close();
                    updateflag_for_ShowGame = true;
                });

                Icon.onClick.AddListener(() =>
                {
                    UpdateGIntro_Panel.SetActive(true);
                    ShowUpdateGIntro(item["GUID"].ToString());
                });

            }
        }
        mySqlTools.Close();
        //return null;
    }


    private void ShowUpdateGIntro(string GUID)
    {
        var mySqlTools = new SqlHelper();
        mySqlTools.Open();
        var ds = mySqlTools.Select("imagelist");

        var c= MysqlTools.TableData(ds);
        foreach(var item in c)
        {
            if(GUID == item["GUID"].ToString())
            {
                GameTypetext_Old.text = item["GameType"].ToString();
                GameIntro_Old.text = item["Introduction"].ToString();

                GameTypetext_New.text = GameTypeDropdown.options[GameTypeDropdown.value].text;
                
                AckUpdate.onClick.AddListener(()=> 
                {
                    mySqlTools.Open();
                    mySqlTools.UpdateIntoSpecific("imagelist",new[]{"GUID"}, new[]{"="}, new[]{ GUID },new[] {"GameType", "Introduction"}, new[]{GameTypetext_New.text, InputField3.text });
                    mySqlTools.Close();
                    UpdateGIntro_Panel.SetActive(false);
                });

                ShowAllComment(GUID);

            }
            
        }
        
        mySqlTools.Close();
    }

    private void ShowAllComment(string CurrentGUID)
    {
        if(Level == "1")
        {
            var mySqlTools = new SqlHelper();
            mySqlTools.Open();
            var ds1 = mySqlTools.Select("GameComment");
            var c1 = MysqlTools.TableData(ds1);

            foreach (Transform item in Group1)
            {
                GameObject.Destroy(item.gameObject);
            }


            foreach(var Iteam in c1)
            {

                GameObject go1 = Instantiate(Resources.Load<GameObject>("GameComment"));
                string GameForCommentGUID = Iteam["GameGUID"].ToString();//GameCommentGUID仅用于判定当前评论是否属于当前游戏中，为游戏的GUID，并非评论的GUID
                
                if(GameForCommentGUID == CurrentGUID )
                {
                    
                    go1.transform.Find("Image/Text (Comment)").GetComponent<Text>().text = Iteam["Comment"].ToString();
                    go1.transform.Find("Image/Text (UserID)").GetComponent<Text>().text = Iteam["Account"].ToString();
                    go1.transform.Find("HotCount/InputField/Placeholder").GetComponent<Text>().text = Iteam["HotCount"].ToString();

                    Button Delete_Button = go1.transform.Find("Delete").GetComponent<Button>();
                    Button Change_Button = go1.transform.Find("HotCount/Change").GetComponent<Button>();
                    
                    Delete_Button.onClick.AddListener(() => 
                    {
                        mySqlTools.Open();
                        mySqlTools.Delete("GameComment", new[]{"CommentGUID"}, new[]{"="}, new[]{Iteam["CommentGUID"].ToString()});
                        mySqlTools.Close();
                    });

                    Change_Button.onClick.AddListener(() =>
                    {
                        mySqlTools.Open();
                        mySqlTools.UpdateIntoSpecific("GameComment", new[]{"CommentGUID"}, new[]{"="}, new[]{Iteam["CommentGUID"].ToString()}, new[]{"HotCount"}, new[]{InputField.text});
                        mySqlTools.Close();
                    });

                    go1.transform.SetParent(Group1);
                    go1.transform.localScale = new Vector3(1, 1, 1);
                }       
                
            }
            updateflag_for_comment = false;
            mySqlTools.Close();
        }
        
    }

    [Obsolete]
    /// <summary>
    /// 下载
    /// </summary>
    /// <param name="url">下载的地址</param>
    /// <returns></returns>
    IEnumerator Download(string ImageName,Image image)
    {
        Debug.Log(ImageName);
        string url = "http://1.13.174.102:9090/"+ ImageName;
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        if (request.isHttpError || request.isNetworkError)
        {
            print("当前的下载发生错误" + request.error);
            yield break;
        }
        if (request.isDone)
        {
            byte[] rawData = request.downloadHandler.data;
            Texture2D texture = new Texture2D(10, 10);
            texture.LoadImage(rawData);
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), UnityEngine.Vector2.zero);
        }
    }
}
