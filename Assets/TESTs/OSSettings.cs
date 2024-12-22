using System.Collections;
using System.IO;
using UnityEngine;
using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace StockGame
{
    public class OSSettings
    {
        public static string GetSettingPath()
        {
            string path;
            try
            {
#if UNITY_EDITOR
                path = Path.Combine(UnityEngine.Application.dataPath + "/EditorTEST/setting.txt");
#elif UNITY_STANDALONE_WIN
            path = Path.Combine(Application.dataPath + "../Settings/setting.txt");
#elif UNITY_STANDALONE_OSX
            path = Path.Combine(Application.dataPath + "../../Settings/setting.txt");
#elif UNITY_WEBGL//WEBGLでは設定固定？(サーバーから読み出してターゲットのtxtを自動作成？)
            path = Path.Combine(Application.streamingAssetsPath + "/setting.txt");
#endif
                return path;
            }
            catch
            {
                DialogShow("パスの作成でエラーが発生しました。", "回復可能なエラー",MessageBoxIcon.Error);
                return Path.Combine(UnityEngine.Application.streamingAssetsPath, "/setting.txt");
                throw new PathNotException();
            }
        }
        public static void DialogShow(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
            /*
#if UNITY_STANDALONE_WIN && UNITY_EDITOR
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
#elif UNITY_STANDALONE_OSX
            ShowMacWarning(title, message);
#endif
        }
#if UNITY_STANDALONE_OSX
    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    private static extern IntPtr NSApplicationLoad();

    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    private static extern IntPtr NSAlertCreate();

    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    private static extern void NSAlertSetMessageText(IntPtr alert, string message);

    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    private static extern void NSAlertSetInformativeText(IntPtr alert, string infoText);

    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    private static extern void NSAlertAddButton(IntPtr alert, string title);

    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    private static extern int NSAlertRunModal(IntPtr alert);

    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    private static extern void NSAlertRelease(IntPtr alert);
    public static void ShowMacWarning(string title, string message)
    {
        // Cocoa環境を初期化
        NSApplicationLoad();

        // NSAlertオブジェクトを作成
        IntPtr alert = NSAlertCreate();

        // メッセージを設定
        NSAlertSetMessageText(alert, title);

        // 詳細な情報を設定
        NSAlertSetInformativeText(alert, message);

        // ボタンを追加
        NSAlertAddButton(alert, "OK");
        NSAlertAddButton(alert, "Cancel");

        // ダイアログを表示してユーザーの選択を取得
        int response = NSAlertRunModal(alert);

        // リソースを解放
        NSAlertRelease(alert);

        // 結果をログに表示
        switch (response)
        {
            case 1000:
                Debug.Log("OKが選択されました。");
                break;
            case 1001:
                Debug.Log("Cancelが選択されました。");
                break;
            default:
                Debug.Log("予期しない結果: " + response);
                break;
        }*/
        }
            
/*#else
        public static void ShowMacWarning(string title, string message)
        {

        }*/
    }
    class PathNotException : Exception
    {
    }
}