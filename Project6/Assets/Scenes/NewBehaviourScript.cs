using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.IO;
using UnityEngine;
using MySql.Data.MySqlClient;
using System;
using System.Net;
using System.Text;
using LitJson;

public class NewBehaviourScript : MonoBehaviour
{

    public void Start()
    {
        //Button_Click();
        //Button_Click_1();
        Login();
    }

    private void Login()
    {
        var OK = SenRT("Login", "1", "2");
        Debug.Log(OK);
        if (OK)
        {
            //UIMgr.Instance.ShowUI(nameof(MainPanel), 1);
        }
    }


    public bool SenRT(string SNNumber, string DownFileName, string UploadFileName)
    {
        HttpClsMessage MyHttpClsMessage = new HttpClsMessage();
        MyHttpClsMessage.SNNumber = SNNumber;
        MyHttpClsMessage.DownFileName = DownFileName;
        MyHttpClsMessage.UploadFileName = UploadFileName;
        MyHttpClsMessage.ComType = HttpCommandType.Login;
        try
        {
            WebClient myClient = new WebClient();
            string AllHttpUrl = HttpClient.Instance.GetUrl(MyHttpClsMessage);

            var a = myClient.DownloadData(AllHttpUrl);
            var str = Encoding.UTF8.GetString(a, 0, a.Length);
            Debug.Log(str);
            //Msg msg = JsonMapper.ToObject<Msg>(str);

            return true;
        }
        catch (Exception ex)
        {
            print(ex.Message);
            return false;
        }

    }


    private void Button_Click()
    {
        MySqlConnection conn;
        MySqlCommand cmd;

        conn = new MySqlConnection();
        cmd = new MySqlCommand();

        string SQL;
        long FileSize;
        byte[] rawData;
        FileStream fs;

        conn.ConnectionString = "SERVER=1.13.174.102;DATABASE=user;UID=root;PASSWORD=root;charset=utf8;";

        try
        {
            fs = new FileStream(@"H:\111.apk", FileMode.Open, FileAccess.Read);
            FileSize = fs.Length;

            rawData = new byte[FileSize];
            fs.Read(rawData, 0, (int)FileSize);
            fs.Close();

            conn.Open();

            SQL = "INSERT INTO applist VALUES(@GUID, @GameName, @Image, @ImageSize)";

            cmd.Connection = conn;
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@GUID", 123);
            cmd.Parameters.AddWithValue("@GameName", "123");
            cmd.Parameters.AddWithValue("@Image", rawData);
            cmd.Parameters.AddWithValue("@ImageSize", rawData.Length);

            cmd.ExecuteNonQuery();

            Debug.Log("Success!");

            conn.Close();
        }
        catch (MySql.Data.MySqlClient.MySqlException ex)
        {
            Debug.LogError(ex);
        }
    }
    private void Button_Click_1()
    {
        MySqlConnection conn;
        MySqlCommand cmd;
        MySqlDataReader myData;

        conn = new MySqlConnection();
        cmd = new MySqlCommand();

        string SQL;
        UInt32 FileSize;
        byte[] rawData;
        FileStream fs;

        conn.ConnectionString = "SERVER=1.13.174.102;DATABASE=user;UID=root;PASSWORD=root;charset=utf8;";

        SQL = "SELECT GUID, App, AppSize FROM applist";

        try
        {
            conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = SQL;

            myData = cmd.ExecuteReader();

            if (!myData.HasRows)
                throw new Exception("There are no BLOBs to save");

            myData.Read();

            FileSize = myData.GetUInt32(myData.GetOrdinal("AppSize"));
            rawData = new byte[FileSize];

            myData.GetBytes(myData.GetOrdinal("App"), 0, rawData, 0, (int)FileSize);

            fs = new FileStream(@"H:\2.apk", FileMode.OpenOrCreate, FileAccess.Write);
            fs.Write(rawData, 0, (int)FileSize);
            fs.Close();

            Debug.Log("Success!");

            myData.Close();
            conn.Close();
        }
        catch (MySql.Data.MySqlClient.MySqlException ex)
        {
            Debug.Log(ex);
        }
    }



}
