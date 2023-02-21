using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour
{

    public Text textObject;

    protected InteractionSystemController _interactionSystemController;
    protected KingdomManager _kingdomManager;
    protected NaturalWorldManager _naturalWorldManager;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            _interactionSystemController = GameObject.FindGameObjectWithTag("InteractionSystemController").GetComponent<InteractionSystemController>();
        }
        catch
        {
            Debug.LogError("Can't find interaction system controller.");
        }

        try
        {
            _kingdomManager = GameObject.FindGameObjectWithTag("KingdomManager").GetComponent<KingdomManager>();
        }
        catch
        {
            Debug.LogError("Can't find kingdom manager.");
        }


        try
        {
            _naturalWorldManager = GameObject.FindGameObjectWithTag("NaturalWorldManager").GetComponent<NaturalWorldManager>();
        }
        catch
        {
            Debug.LogError("Can't find natural world manager.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        string textOutput = "";

        textOutput += "Total Population: " + _kingdomManager.GetAgentCount();
        textOutput += "\n\n";

        textOutput += "Guilds: \n";
        foreach (Guild guild in _kingdomManager.GetGuilds())
        {
            textOutput += guild.name + ": " + guild.GetAgentCount();
            textOutput += "\n";
        }
        textOutput += "\n";

        textOutput += "Total stored resources: \n";
        foreach (int i in System.Enum.GetValues(typeof(ResourceType)))
        {
            if (i != 0)
            {
                textOutput += (ResourceType)i;
                textOutput += ": " + _kingdomManager.GetTotalResources((ResourceType)i);
                textOutput += "\n";
            }
        }

        textOutput += "\n\n\n\n";

        textOutput += "Interaction System: " + _interactionSystemController.GetControlType() + "\n";

        textOutput += "(Space to toggle)";

        textObject.text = textOutput;
    }
}
