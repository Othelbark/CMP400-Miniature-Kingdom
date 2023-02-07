using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType { NONE, WOOD, STONE, FOOD }
public enum AgentState { WAITING, MOVING, WOODCUTTING, STORING }
public enum GuildState { INACTIVE, ACTIVE }

public enum GatherableState { NON_GATHERABLE, GATHERABLE_NOT_READY, GATHERABLE_READY}
