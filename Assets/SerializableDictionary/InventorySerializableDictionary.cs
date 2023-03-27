using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class InventoryDictionary : SerializableDictionary<ResourceType, int> { }
[Serializable]
public class PrefabsByString : SerializableDictionary<string, GameObject> { }

