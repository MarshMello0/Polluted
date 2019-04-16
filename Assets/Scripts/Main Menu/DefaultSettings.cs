using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class DefaultSettings : ScriptableObject
{
    public List<Setting> defaultSettings = new List<Setting>();
}
