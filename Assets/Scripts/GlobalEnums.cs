using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType { NONE = 0, WOOD, STONE, PLANKS, STONE_BLOCKS, FOOD }
public enum AgentState { WAITING, MOVING, COLLECTING, STORING, WORKING, UNWORKING, PICK_UP, DROP_OFF, CLEAR_INVENTORY, DUMP_INVENTORY }
public enum GuildState { INACTIVE, ACTIVE }

public enum GatherableState { NON_GATHERABLE, GATHERABLE_NOT_READY, GATHERABLE_READY }

public enum ControlType { NONE = 0, DIRECT = 1, ABSTRACTED = 2 }

public enum ConstructionState { WAITING_FOR_RESOURCES, BUILDING, DECONSTRUCTING, WAITING_FOR_EMPTY }

static class Constants
{
    public const int AgentInventorySpace = 100;
}

