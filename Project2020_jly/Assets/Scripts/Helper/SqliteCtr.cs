using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;

/// <summary>
/// sqlite的控制类
/// </summary>
public class SqliteCtr
{
    //sqlite的数据库名称(数据库放在streamingAssets的路径下)
    public static string DataBaseName = "/TestDB.db";
    private static readonly SqliteCtr instance = new SqliteCtr();
    private SqliteDataReader reader;
    private static SqliteDbHelper dbHelper;
    public SqliteDataReader Reader
    {
        get { return reader; }
        set { reader = value; }
    }
    public static SqliteDbHelper DbHelper
    {
        get {
            if (dbHelper == null)
                dbHelper = new SqliteDbHelper(Application.streamingAssetsPath + DataBaseName);
            return dbHelper; 
        }
    }
    public static SqliteCtr Instance
    {
        get { return instance; }
    }
}
