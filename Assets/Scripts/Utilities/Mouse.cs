using System.Security.Policy;
using UnityEngine;

public static class Mouse
{
        public static void Lock(bool isLocked)
        {
                Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = !isLocked;
        }
}