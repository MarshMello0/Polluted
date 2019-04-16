using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using TMPro;
using UnityEngine.UI;
using System;

public class SteamInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private RawImage pp;
    
    private void Start()
    {
        if (SteamManager.Initialized)
        {
            name.text = SteamFriends.GetPersonaName();
            pp.texture = GetSteamImageAsTexture2D(SteamFriends.GetLargeFriendAvatar(SteamUser.GetSteamID()));

        }
    }

    private IEnumerator A()
    {
        
        DateTime before = DateTime.Now;
        yield return new WaitForSeconds(1);
        DateTime after = DateTime.Now; 
        TimeSpan duration = after.Subtract(before);
        Debug.Log("Duration in milliseconds: " + duration.Milliseconds);
    }
    
    public static Texture2D GetSteamImageAsTexture2D(int iImage) {
        Texture2D ret = null;
        uint ImageWidth;
        uint ImageHeight;
        bool bIsValid = SteamUtils.GetImageSize(iImage, out ImageWidth, out ImageHeight);

        if (bIsValid) {
            byte[] Image = new byte[ImageWidth * ImageHeight * 4];

            bIsValid = SteamUtils.GetImageRGBA(iImage, Image, (int)(ImageWidth * ImageHeight * 4));
            if (bIsValid) {
                ret = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
                ret.LoadRawTextureData(Image);
                ret.Apply();
            }
        }

        return ret;
    }
}
