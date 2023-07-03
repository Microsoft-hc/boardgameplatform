using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    public Button LoginBtn;
    public Button RegisterBtn;
    public Button ConfirmRegisterBtn;
    public Button BackRegisterBtn;
    public InputField InputField1;
    public InputField InputField2;
    public InputField InputField3;//注册时填写的账号
    public InputField InputField4;//注册时填写的密码
    public InputField InputField5;//注册时填写的昵称
    public InputField InputField6;//注册时填写的电子邮箱
    public GameObject RegisterPanel;//注册面板
    public static object UserID;

    private void Awake()
    {
        Application.runInBackground= true;
        LoginBtn.onClick.AddListener(() => { Login(); });
        RegisterBtn.onClick.AddListener(() => { RegisterPanel.SetActive(true); });
        ConfirmRegisterBtn.onClick.AddListener(() => { Register(); });
        BackRegisterBtn.onClick.AddListener(() => { RegisterPanel.SetActive(false); });
    }

    private void Start()
    {
        RegisterPanel.SetActive(false);
    }

    private void Login()
    {
        if (InputField1.text == "" || InputField2.text == "")
        {
            TipPanel.Instance.ShowMsg("账号密码不能为空");
            return;
        }

        //创建数据库类                 IP地址       端口    用户名   密码     数据库项目名称
        var mySqlTools = new SqlHelper();
        //打开数据库
        mySqlTools.Open();
        var ds = mySqlTools.SelectWhere("usertable", new[] { "Account" },new[] { "=" }, new[] { InputField1.text });
        object values = MysqlTools.GetValue(ds, "Psd");
        print(values); //最后打印15924
        mySqlTools.Close();
        if (values == null)
        {
            TipPanel.Instance.ShowMsg("账号不存在");
            return;
        }
        if (values.ToString() != InputField2.text)
        {
            TipPanel.Instance.ShowMsg("密码不正确");
            return;
        }
        UIMgr.Instance.ShowUI(nameof(MainPanel), 1);

        UserID = MysqlTools.GetValue(ds,"Account");
    }

    private void Register()
    {
        if (InputField3.text == "" || InputField4.text == "")
        {
            TipPanel.Instance.ShowMsg("账号密码不能为空");
            return;
        }
        if (InputField5.text == "" || InputField6.text == "")
        {
            TipPanel.Instance.ShowMsg("昵称或邮箱不能为空");
            return;
        }

        var mySqlTools = new SqlHelper();
        mySqlTools.Open();
        var ds = mySqlTools.SelectWhere("usertable", new[] { "Account" }, new[] { "=" }, new[] { InputField3.text });
        object values = MysqlTools.GetValue(ds, "Account");
        print(values); //最后打印15924
        if (values != null)
        {
            TipPanel.Instance.ShowMsg("账号已存在");
            mySqlTools.Close();
            return;
        }
        mySqlTools.InsertInto("usertable", new[] { "Account", "UserName", "Psd", "Email", "Level", "Type_Abstract", "Type_Customizable", "Type_Kid", "Type_Family", "Type_Party", "Type_Strategy", "Type_Thematic", "Type_WarChess" }, new[] { InputField3.text, InputField5.text, InputField4.text, InputField6.text, "0", "0", "0", "0", "0", "0", "0", "0", "0" });
        mySqlTools.Close();
        RegisterPanel.SetActive(false);
        TipPanel.Instance.ShowMsg("注册成功");
    }




  

}
