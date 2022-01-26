//Code by Eele Roet
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// <para>InventoryScript script that:</para>
/// <para>keeps track of collected items, allows player to switch between items,</para>
/// <para>handles adding items, keeps track of item amount and using items,</para>
/// <para>updates the HUD inventory when player selects a different item.</para>
/// </summary>
public class InventoryScript : MonoBehaviour
{
    [SerializeField] HandleWeapons playerWeaponScript; //reference to script that shows weapon models and spawns usables

    //hammer related variables
    [SerializeField] private RawImage hammerSlotImage;
    [SerializeField] private RawImage hammerSlotHighlight;
    private static ArrayList hammers;
    private static InventoryItem emptyHammer = new InventoryItem(ItemType.empty, true);
    private static InventoryItem currentHammer;
    [SerializeField] private Texture2D[] hammerTextures = new Texture2D[5];
    private static ItemType lastHammerPickedUp = ItemType.empty;

    //usable related variables
    [SerializeField] private RawImage usableSlotImage;
    [SerializeField] private RawImage usableSlotHighlight;
    [SerializeField] private TMP_Text usableCounter;
    private static ArrayList usables;
    private static InventoryItem emptyUsable = new InventoryItem(ItemType.empty, false);
    public static InventoryItem currentUsable;
    [SerializeField] private Texture2D[] usableTextures = new Texture2D[5];

    private static bool updateUIImagesNextFrame = false;//used to update UI from static methods
    private static bool updateEquippedNextFrame = false;//used to update weapon model in player hands from static methods
    public static InventoryItem equippedItem;
    

    // Start is called before the first frame update
    void Start()
    {
        hammers = new ArrayList();
        usables = new ArrayList();
        currentHammer = emptyHammer;
        currentUsable = emptyUsable;
        
        AddHammer(ItemType.stoneHammer);
        UpdateEquipped(currentHammer);

        updateUIImagesNextFrame = true;
    }

    // Update is called once per frame
    void Update()
    {
        //update hotbar images when needed
        if(updateUIImagesNextFrame)
        {
            updateUIImagesNextFrame = false;
            UpdateImagesVoid();
        }
        if(updateEquippedNextFrame)
        {
            updateEquippedNextFrame = false;
            UpdateEquipped(equippedItem);
        }
        HandleHotbarInput();//handles keyboard input for switching items
    }

    public static bool IsHammerEquipped()
    {
        return equippedItem.IsHammer();
    }

    public static ItemType GetLastHammerPickedUp()
    {
        return lastHammerPickedUp;
    }

    public static InventoryItem GetInventoryItemByType(ItemType type) //returns an item from the inventory by type
    {
        if(hammers.Count > 0)
        {
            foreach(InventoryItem item in hammers)
            {
                if(item.GetItemType() == type)
                {
                    return item;
                }
            }
        }
        if(usables.Count > 0)
        {
            foreach (InventoryItem item in usables)
            {
                if (item.GetItemType() == type)
                {
                    return item;
                }
            }
        }
        return null;
    }

    public static void AddHammer(ItemType hammerType)
    {
        //switch hammerType to check if hammer exists
        switch (hammerType)
        {
            case ItemType.stoneHammer:
            case ItemType.goldHammer:
            case ItemType.rubberHammer:
            case ItemType.elekHammer:
                
                if (GetInventoryItemByType(hammerType) == null) //if hammer isn't already in inventory
                {
                    InventoryItem hammerToAdd = new InventoryItem(hammerType, true);
                    hammers.Add(hammerToAdd); //add new hammer 
                    lastHammerPickedUp = hammerType;
                    if (currentHammer.GetItemType() == ItemType.empty)//if there was no hammer set new hammer to current hammer
                    {
                        currentHammer = hammerToAdd;
                        updateUIImagesNextFrame = true;
                    }
                    GameManager.ShowFadingText("Use E to switch between hammers");
                }
                else
                {
                    Debug.Log("hammer already in inventory");
                }
                break;
            default:
                Debug.Log("hammer with this name doesn't exist");
                break;
        }
    }

    public static void AddUsable(ItemType usableType)
    {
        //switch hammerName to check if usable exists 
        switch (usableType)
        {
            case ItemType.bomb:
            case ItemType.ladder:
            case ItemType.moneybag:
                InventoryItem possibleItem = GetInventoryItemByType(usableType);
                if (possibleItem == null)//if usable isn't already in inventory
                {
                    InventoryItem usableToAdd = new InventoryItem(usableType, false);
                    usables.Add(usableToAdd);//add new usable
                    if (currentUsable.GetItemType() == ItemType.empty)  
                    {
                        currentUsable = usableToAdd;
                        updateUIImagesNextFrame = true;
                    }
                    GameManager.ShowFadingText("Use Q to switch to bombs and other usables");
                }
                else//if usable is already in inventory
                {
                    possibleItem.AddAmount(1);//add 1 to amount
                    updateUIImagesNextFrame = true;
                }
                break;
            default:
                Debug.Log("usable with this name doesn't exist");
                break;
        }
    }

    public static ItemType UseUsable()//public method used by HandleWeapons.cs to see what usable to spawn
    {
        if (currentUsable != null && currentUsable.GetItemType() != ItemType.moneybag)
        {
            if (currentUsable.GetAmount() > 1)//if there is 2 or more of a usable 
            {
                currentUsable.AddAmount(-1);//remove 1 from amount
                updateUIImagesNextFrame = true;
                return currentUsable.GetItemType();//pass type to HandleWeapons
            }
            else if(currentUsable.GetAmount() == 1)//if there is only 1 of a usable left
            {
                ItemType tempType = currentUsable.GetItemType();
                usables.Remove(currentUsable);//remove the usable
                currentUsable = usables.Count > 0 ? usables[0] as InventoryItem : emptyUsable; //if there is a usable left in inventory set a new usable, else set empty.
                equippedItem = currentUsable;
                updateEquippedNextFrame = true;
                updateUIImagesNextFrame = true;
                return tempType;
            }
            else//if usable amount is 0, remove usable
            {
                usables.Remove(currentUsable);
                currentUsable = usables.Count > 0 ? usables[0] as InventoryItem : emptyUsable; //if there is a usable left in inventory set a new usable, else set empty.
                return ItemType.empty;
            }

        }
        else
        {
            return ItemType.empty;
        }
    }

    private void ScrollEquipped(int scrollDirection) //scroll through items in selected slot, hammers or usables
    {
        if(equippedItem.IsHammer())//scroll hammer
        {
            if (hammers.Count < 2)
            {
                return;
            }
            else //go to next hammer in the arraylist
            {
                int newHammerIndex = Math.Abs(hammers.IndexOf(currentHammer) + scrollDirection) % (hammers.Count);
                currentHammer = hammers[newHammerIndex] as InventoryItem;
                UpdateEquipped(currentHammer);
                updateUIImagesNextFrame = true;
            }
        }
        else//scroll usable
        {
            if (usables.Count < 2)
            {
                return;
            }
            else //go to next usable in the arraylist
            {
                int newUsableIndex = Math.Abs(usables.IndexOf(currentUsable) + scrollDirection) % (usables.Count);
                currentUsable = usables[newUsableIndex] as InventoryItem;
                UpdateEquipped(currentUsable);
                updateUIImagesNextFrame = true;
            }
        }
    }

    private void SwitchSlots()
    {
        if (currentHammer == equippedItem)
        {
            UpdateEquipped(currentUsable);
        }
        else
        {
            UpdateEquipped(currentHammer);
        }
        updateUIImagesNextFrame = true;
    }

    private void UpdateEquipped(InventoryItem newEquipped) //update the model the player is holding
    {
        equippedItem = newEquipped;
        playerWeaponScript.SwitchToWeapon(newEquipped.GetItemType());
    }

    private void UpdateImagesVoid() //updates the HUD inventory images
    {
        switch (currentHammer.GetItemType()) //update hammer image
        {
            case ItemType.stoneHammer:
                hammerSlotImage.texture = hammerTextures[0];
                hammerSlotImage.enabled = true;
                break;
            case ItemType.goldHammer:
                hammerSlotImage.texture = hammerTextures[1];
                hammerSlotImage.enabled = true;
                break;
            case ItemType.rubberHammer:
                hammerSlotImage.texture = hammerTextures[2];
                hammerSlotImage.enabled = true;
                break;
            case ItemType.elekHammer:
                hammerSlotImage.texture = hammerTextures[3];
                hammerSlotImage.enabled = true;
                break;
            case ItemType.empty:
                hammerSlotImage.enabled = false;
                break;
        }
        if (currentUsable != null)
        {
            switch (currentUsable.GetItemType()) //update usable image
            {
                case ItemType.bomb:
                    usableSlotImage.texture = usableTextures[0];
                    usableSlotImage.enabled = true;
                    usableCounter.enabled = true;
                    break;
                case ItemType.ladder:
                    usableSlotImage.texture = usableTextures[1];
                    usableSlotImage.enabled = true;
                    usableCounter.enabled = true;
                    break;
                case ItemType.moneybag:
                    usableSlotImage.texture = usableTextures[2];
                    usableSlotImage.enabled = true;
                    usableCounter.enabled = true;
                    break;
                case ItemType.empty:
                    usableSlotImage.enabled = false;
                    usableCounter.enabled = false;
                    break;
            }
            usableCounter.text = currentUsable.GetAmount().ToString(); //update usable counter text
        }

        //update highlight
        if (equippedItem.IsHammer())
        {
            hammerSlotHighlight.enabled = true;
            usableSlotHighlight.enabled = false;
        }
        else
        {
            hammerSlotHighlight.enabled = false;
            usableSlotHighlight.enabled = true;
        }
    }

    private void HandleHotbarInput()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            ScrollEquipped(1);
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            SwitchSlots();
        }
    }
}

/// <summary>
/// <para>InventoryItem class that:</para>
/// <para>is used to store information about the item.</para>
/// <para>stores ItemType, amount and if the item is a hammer or not.</para>
/// </summary>
public class InventoryItem //class that stores information about inventory items
{
    private ItemType type;
    private int amount;
    private bool isHammer;

    public InventoryItem(ItemType itemType, bool hammer)
    {
        type = itemType;
        amount = 1;
        isHammer = hammer;
    }

    public ItemType GetItemType()
    {
        return type;
    }

    public int GetAmount()
    {
        return amount;
    }

    public void AddAmount(int num)
    {
        amount += num;
    }

    public bool IsHammer()
    {
        return isHammer;
    }
}

/// <summary>
/// ItemType enum that is used to give items a unique type.
/// </summary>
public enum ItemType
{
    stoneHammer,
    goldHammer,
    rubberHammer,
    elekHammer,
    bomb,
    ladder,
    moneybag,
    empty
}