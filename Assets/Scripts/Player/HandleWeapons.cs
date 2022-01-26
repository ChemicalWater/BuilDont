// Code by Eele Roet
using UnityEngine;

/// <summary>
/// <para>script that handles holding and using weapons</para>
/// <para>shows correct weaponmodel in the hands of the player, handles input for using weapons, instantiates usables in world.
/// </summary>

public class HandleWeapons : MonoBehaviour
{
    [SerializeField] GameObject hammer;//GameObject that contains all hammer models
    [SerializeField] GameObject usable;//GameObject that contains all usable models
    [SerializeField] GameObject bomb;//exploding bomb prefab
    [SerializeField] GameObject moneybag; // money from the bank
    [SerializeField] GameObject ladder;//climable ladder

    [SerializeField] AudioSource weaponSoundsSource;//audio source for switching and using weapons
    [SerializeField] AudioClip switchSound;//sound for switching weapons
    [SerializeField] AudioClip[] useSounds;//sounds for using different usables

    // Update is called once per frame
    void Update()
    {
        //if player is holding a hammer
        if (InventoryScript.IsHammerEquipped()) 
        {
            //if leftmousebutton is pressed or released this frame
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
            {
                SpinHammer.CheckForSwing(Input.GetMouseButtonDown(0), Input.GetMouseButtonUp(0));//handles swinging hammer animation in spinHammer script
            }
        }
        //if player is holding a usable
        else
        {
            //if leftmousebutton is pressed this frame
            if (Input.GetMouseButtonDown(0))
            {
                switch (InventoryScript.UseUsable()) //checks the type of usable to spawn
                {
                    case ItemType.bomb:
                        Instantiate(bomb, gameObject.transform.position, Quaternion.identity);//spawn bomb
                        weaponSoundsSource.clip = useSounds[0];
                        weaponSoundsSource.Play(); 
                        break;
                    case ItemType.ladder:
                        Instantiate(ladder, gameObject.transform.position, gameObject.transform.rotation);//spawn ladder
                        weaponSoundsSource.clip = useSounds[1];
                        weaponSoundsSource.Play();
                        break;
                    case ItemType.moneybag:
                        Instantiate(moneybag, gameObject.transform.position, gameObject.transform.rotation);//spawn moneybag
                        weaponSoundsSource.clip = useSounds[2];
                        weaponSoundsSource.Play();
                        break;

                    case ItemType.empty:
                        //do nothing
                        break;
                }
            }
        }
    }

    //shows the model for 'weaponType' in the hands of the player
    public void SwitchToWeapon(ItemType weaponType)
    {
        //disable all the models
        DisableUsableChildren();
        DisableHammerChildren();

        //checks the type of model to enable, then enables that model.
        switch (weaponType)
        {
            case ItemType.stoneHammer:
                hammer.transform.Find("default").gameObject.SetActive(true);
                break;
            case ItemType.goldHammer:
                hammer.transform.Find("gold").gameObject.SetActive(true);
                break;
            case ItemType.rubberHammer:
                hammer.transform.Find("rubber").gameObject.SetActive(true);
                break;
            case ItemType.elekHammer:
                hammer.transform.Find("elek").gameObject.SetActive(true);
                break;
            case ItemType.bomb:
                usable.transform.Find("bomb").gameObject.SetActive(true);
                break;
            case ItemType.ladder:
                usable.transform.Find("ladder").gameObject.SetActive(true);
                break;
            case ItemType.moneybag:
                usable.transform.Find("MoneyBag").gameObject.SetActive(true);
                break;
            case ItemType.empty:
                break;
        }
        weaponSoundsSource.clip = switchSound;
        weaponSoundsSource.Play();
    }

    //method for disabling all models of the hammer.
    private void DisableHammerChildren()
    {
        for (int i = 0; i < hammer.transform.childCount; i++)
        {
            var child = hammer.transform.GetChild(i).gameObject;
            if (child != null)
                child.SetActive(false);
        }
    }

    //method for disabling all models of the usable.
    private void DisableUsableChildren()
    {
        for (int i = 0; i < usable.transform.childCount; i++)
        {
            var child = usable.transform.GetChild(i).gameObject;
            if (child != null)
                child.SetActive(false);
        }

    }
}
