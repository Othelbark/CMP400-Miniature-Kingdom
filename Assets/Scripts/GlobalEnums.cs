using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType { NONE, WOOD, STONE, PLANKS, STONE_BLOCKS, FOOD }
public enum AgentState { WAITING, MOVING, COLLECTING, STORING, WORKING, CLEAR_INVENTORY, DUMP_INVENTORY }
public enum GuildState { INACTIVE, ACTIVE }

public enum GatherableState { NON_GATHERABLE, GATHERABLE_NOT_READY, GATHERABLE_READY}
