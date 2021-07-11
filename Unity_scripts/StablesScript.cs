using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StablesScript : MonoBehaviour
{
    public GameObject menuManager;
    MenuManagerScript menuManagerScript;
    public GameObject fieldManager;
    FieldManager fieldManagerScript;
    public GameObject barnDoor;
    BarnDoorScript barnDoorScript;
    public GameObject inventoryManager;
    InventoryManagerScript inventoryManagerScript;
    // Start is called before the first frame update
    void Start()
    {
        menuManagerScript = menuManager.GetComponent<MenuManagerScript>();
        fieldManagerScript = fieldManager.GetComponent<FieldManager>();
        barnDoorScript = barnDoor.GetComponent<BarnDoorScript>();
        inventoryManagerScript = inventoryManager.GetComponent<InventoryManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnMouseDown()
    {
        if (!menuManagerScript.messageUp && menuManagerScript.royIsRoosted) {
            if (!menuManagerScript.waitForAnimalToEat && !menuManagerScript.waitForAnimalToBeHappy) {
                if (menuManagerScript.atTheMainMenu) {
                    menuManagerScript.goToStables = true;
                } else if (menuManagerScript.atTheField) {
                    menuManagerScript.goFromFieldToStables = true;
                    inventoryManagerScript.TellAllAnimalsToGoHome();
                    fieldManagerScript.ClearAllFood();
                    fieldManagerScript.selectedAnimal = null;
                    fieldManagerScript.canTestField = true;
                } else if (menuManagerScript.atTheBarnDoors) {
                    menuManagerScript.goToStart = true;
                    barnDoorScript.CloseDoors();
                } else if (menuManagerScript.atTheStables) {
                    menuManagerScript.goToStart = true;
                }
            }
        } else {
            if (menuManagerScript.waitForAnimalToEat || menuManagerScript.waitForAnimalToBeHappy) {
                menuManagerScript.BackButtonTappedDuringTutorialSoTellPlayerToFeedAnimal();
            }
        }
    }
}
