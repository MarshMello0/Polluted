using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class DefaultBindings : ScriptableObject
{
    public List<Binding> defaultBindings = new List<Binding>();
}
