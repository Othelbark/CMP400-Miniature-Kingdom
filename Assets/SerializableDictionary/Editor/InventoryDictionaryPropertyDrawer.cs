using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(InventoryDictionary))]
public class InventoryDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }


[CustomPropertyDrawer(typeof(PrefabsByString))]
public class PrefabsByStringPropertyDrawer : SerializableDictionaryPropertyDrawer { }