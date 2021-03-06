﻿using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    // UI Game Objects
    [Header("Trasnform Objects")]
    public RectTransform weaponInfoContainer;
    public RectTransform potionInfoContainer;
    public RectTransform characterStats;
    public RectTransform characterInventory;
    public RectTransform potionTab, weaponsTab, armorTab;
    public RectTransform nextFloorPrompt;
    public RectTransform gameOverScreen;
    public RectTransform enemyStatScreen;
    public RectTransform logEventScreen;
    public RectTransform weaponInfoBox;
    public RectTransform IntroScreen;
    public RectTransform escapePrompt;
    public RectTransform brandingTutorial;
    public RectTransform version1EndTransform;

    [Space(20)]
    [Header("Text Objects")]
    public Text playerHealthText;
    public Text inventoryPotionStat;
    public Text weaponNameText;
    public Text inventoryWeaponStats;
    public Text playerDamageText;
    public Text enemyNameText;
    public Text enemyHealthText;
    public Text enemyDamageText;
    public Text playerMoneyText;
    public Text playerMaxMoneyText;
    public Text currentFloorText;
    public Text newMoneyText;
    public Text playerArmorText;
    public Text weaponInfoName;
    public Text weaponInfo;
    public Text inventoryHealthText;
    public Text inventoryHealthAddedText;
    public Text fullyExploredMap;
    public Text numberOfRedPotions;
    public Text numberOfBluePotions;
    public Text inGameRedPotion, inGameBluePotion;
    public Text inGameRedPotionConfirm, inGameBluePotionConfirm;

    [Space(20)]
    [Header("Slider Objects")]
    public Slider enemyHealthSlider;
    public Slider healthSlider;
    public Slider healthRemovedSlider;
    public Slider inGame_PlayerHealthSlider;
    public Slider inGame_EnemyHealthSlider;

    [Space(20)]
    [Header("Image Objects")]
    public Image fadePanel;
    public Image inventoryWeaponImage;
    public Image inventoryPotionImage;
    public Image healthRemovedSliderImage;
    public Image fadePanelGameOver;

    [Space(25)]
    [Header("Inventory Slots")]
    public Image[] weaponSlots;
    public Image[] armorSlots;
    //public Image[] potionSlots;

    [Space(25)]
    [Header("Inventory/Currently Equiped Weapon")]
    public Image inventoryCurrentWeaponImage;
    public Image inventoryCurrentArmorImage;

    [Space(25)]
    [Header("Inventory/Bottom Right")]
    public Text inventoryPhysicalDamageText;
    public Text inventoryCriticalChanceText;
    public Text inventoryArmorText;

    [Space(25)]
    [Header("Inventory/Armor Info")]
    public RectTransform armorInfoHolder;
    public Text armorInfoStat;
    public Image armorImage;
    public Text inventoryArmorName;

    [Space(25)]
    [Header("Chest/Confirm Weapon")]
    public RectTransform confirmWeapon;
    public Text itemName;
    public Text stat1; // Damage
    public Text stat2; // Crit Chance
    public Image icon1;
    public Image icon2;
    public Button actionButton;

    // Attribute classes
    private PlayerManager playerManager;
    private PlayerInventory playerInventory;
    private FloorManager floorManager;
    private EventBox eventBox;
    private ShopKeeperV2 shopKeeper;
    private BrandStation brandStation;

    // Currently selected inventory weapon
    private Weapon currentlySelectedInventoryWeapon;
    private int currentlySelectedWeaponIndex = -1;

    private int currentlySelectedPotionIndex = -1;

    private Armor currentlySelectedInventoryArmor;
    private int currentlySelectedArmorIndex = -1;

    private const float doubleClickInterval = 0.1f;
    private const float fadeSpeed = 3.5f;

    private Vector2 weaponInfoBoxOffset;

    private SoundManager soundManager;
    private Inventory inventory;

    private float fullyExploredMapTimer = 0;

    private void Awake()
    {
        shopKeeper = FindObjectOfType<ShopKeeperV2>();
        floorManager = FindObjectOfType<FloorManager>();
        playerInventory = FindObjectOfType<PlayerInventory>();
        playerManager = FindObjectOfType<PlayerManager>();
        eventBox = FindObjectOfType<EventBox>();
        inventory = FindObjectOfType<Inventory>();
        brandStation = FindObjectOfType<BrandStation>();
    }

    private void Start()
    {
        StartCoroutine("FadeIn");

        NewPlayerValues();
        UpdatePotionSlots();
        UpdateWeaponSlots();

        float width = weaponInfoBox.rect.width;
        //float height = weaponInfoBox.rect.height;
        weaponInfoBoxOffset = new Vector2(width*0.75f, 60);

        soundManager = FindObjectOfType<SoundManager>();

        // Display intro?
        if (!PlayerPrefs.HasKey("showIntro")) 
        {
            IntroScreen.gameObject.SetActive(true);
            PlayerPrefs.SetInt("showIntro", 0);
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            logEventScreen.gameObject.SetActive(!logEventScreen.gameObject.activeInHierarchy);

        // Flash text things
        if(newMoneyText.color != Color.clear)
        {
            newMoneyText.color = Color.Lerp(newMoneyText.color, Color.clear, 1.5f * Time.deltaTime);
        }
        if(fullyExploredMap.color != Color.clear)
        {   
            if(fullyExploredMapTimer >= 0)
            {
                fullyExploredMapTimer -= Time.deltaTime;
            }
            else
            {
                fullyExploredMap.color = Color.Lerp(fullyExploredMap.color, Color.clear, 0.5f * Time.deltaTime);
            }
        }

        if (healthRemovedSliderImage.color != Color.clear)
            healthRemovedSliderImage.color = Color.Lerp(healthRemovedSliderImage.color, Color.clear, 1f * Time.deltaTime);

        // If the weapon info box is enabled glue it to the mouse
        if (weaponInfoBox.gameObject.activeSelf)
        {
            weaponInfoBox.position = Input.mousePosition + (Vector3)weaponInfoBoxOffset;
        }

        #region Hot Keys
        // Event Log Escape
        if (logEventScreen.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                logEventScreen.gameObject.SetActive(false);
            }
        }

        // Next floor prompt
        if (nextFloorPrompt.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Q))
                NextFloor();
            if (Input.GetKeyDown(KeyCode.E))
                DisableNextFloorPrompt();
        }

        // Player Inventory Hot Keys

        // Toggle Stuff with I
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (shopKeeper.ShopTransform.gameObject.activeInHierarchy)
            {
                shopKeeper.ShopTransform.gameObject.SetActive(false);
            }
            else if (brandStation.BrandStaionTransform.gameObject.activeInHierarchy)
            {
                brandStation.BrandStaionTransform.gameObject.SetActive(false);
            }
            else
            {
                ToggleInventoryScreen();
            }
        }

        // Navigating inside the inventory
        if (characterInventory.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                GoTo_WeaponsTab();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                GoTo_ArmorTab();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                GoTo_PotionTab();
            }
        }

        #endregion
    }

    public void NewPlayerValues()
    {    
        // Player Health Slider
        {
            healthSlider.maxValue = playerManager.getMaxHealth();
            healthSlider.value = playerManager.getHealth();
        }

        // Update the player health text
        if (PlayerPrefs.GetInt("DISPLAY_HEALTH_IN_NUMBERS") == 1)
        {
            playerHealthText.text = playerManager.getHealth() + "/" + playerManager.getMaxHealth();
        }
        else
        {
            float healthPercentage = Convert.ToInt16((playerManager.getHealth() / playerManager.getMaxHealth()) * 100);
            playerHealthText.text = healthPercentage.ToString() + "%";
        }

        // Update player physical damage 
        playerDamageText.text = ""+(playerManager.getAttack() + 
            (playerManager.getEquipedWeapon() != null ? playerManager.getEquipedWeapon().getNormalAttack() : 0));

        // Update player armor text
        if (playerManager.getEquipedArmor() == null)
        {
            playerArmorText.text = ((playerManager.getArmor()) * 100).ToString("0.#"); // Add actual armor piece to this when implemented
        }
        else
        {
            playerArmorText.text = ((playerManager.getArmor() * 100) + playerManager.getEquipedArmor().getArmor() * 100).ToString("0.#");
        }

        // Money text
        playerMoneyText.text = playerManager.getMoney().ToString();
        playerMaxMoneyText.text = "/" + playerManager.getMaxMoney().ToString();

        // Path: Inventory/Currently Equiped Weapon
        // Path: Inventory/Bottom Right
        if (playerManager.getEquipedWeapon() != null)
        {
            inventoryPhysicalDamageText.text = (playerManager.getAttack() + playerManager.getEquipedWeapon().getNormalAttack()).ToString();

            inventoryCurrentWeaponImage.color = Color.white;
            inventoryCurrentWeaponImage.sprite = playerManager.getEquipedWeapon().getWeaponSprite();
            if (playerManager.getEquipedWeapon().getCritChance() == -1)
            {
                inventoryCriticalChanceText.text = "0%";
            }
            else
            {
                inventoryCriticalChanceText.text = playerManager.getEquipedWeapon().getCritChance().ToString() + "%";
            }
        }
        // If the player has no weapon equipeds
        else
        {
            // Currently equiped weapon
            inventoryCurrentWeaponImage.color = Color.clear;
            inventoryCurrentWeaponImage.sprite = null;

            // Inventory bottom right
            inventoryPhysicalDamageText.text = playerManager.getAttack().ToString();
            inventoryCriticalChanceText.text = "0%";
        }

        // Armor
        if (playerManager.getEquipedArmor() == null)
        {
            inventoryCurrentArmorImage.color = Color.clear;
            inventoryArmorText.text = (((playerManager.getArmor()) * 100).ToString("0.#"));
        }else
        {
            inventoryCurrentArmorImage.color = Color.white;
            inventoryCurrentArmorImage.sprite = playerManager.getEquipedArmor().getArmorSprite();
            inventoryArmorText.text = ((playerManager.getArmor() * 100) + playerManager.getEquipedArmor().getArmor() * 100).ToString("0.#");
        }

        // Health
        inventoryHealthText.text = playerManager.getHealth() + "/" + playerManager.getMaxHealth();
        inventoryHealthAddedText.text = string.Empty;

        inventory.Toggled();
    }

    // Hovered over a weapon in the inventory
    public void HoveredoverWeapon(int _index)
    {
        if(playerInventory.GetWeaponsList().Count > _index)
        {
            weaponInfoBox.gameObject.SetActive(true);

            weaponInfoName.text = playerInventory.GetWeaponsList()[_index].getName();
            weaponInfo.text = playerInventory.GetWeaponsList()[_index].getDescription();
        }
    }

    public void HoveredOverArmor(int _index)
    {
        if(playerInventory.GetArmorList().Count > _index)
        {
            weaponInfoBox.gameObject.SetActive(true);

            weaponInfoName.text = playerInventory.GetArmorList()[_index].getName();
            weaponInfo.text = playerInventory.GetArmorList()[_index].getDescription();
        }
    }

    public void MouseLeftWeapon()
    {
        weaponInfoBox.gameObject.SetActive(false);
    }

    // Selects whatever weapon we clicked on if the _index inside players weapons list
    public void ClickedOnWeapon(int _index)
    {
        if (playerInventory.GetWeaponsList().Count > _index)
        {
            armorInfoHolder.gameObject.SetActive(false);
            potionInfoContainer.gameObject.SetActive(false);
            weaponInfoContainer.gameObject.SetActive(true);

            weaponNameText.text = playerInventory.GetWeaponsList()[_index].getName();
            inventoryWeaponImage.sprite = playerInventory.GetWeaponsList()[_index].getWeaponSprite();

            // Calculating new damage
            #region + - On Stats in the inventory
            float excessDamage = 0;
            if (playerManager.getEquipedWeapon() != null)
            {
                excessDamage = (playerInventory.GetWeaponsList()[_index].getNormalAttack()) - (playerManager.getEquipedWeapon().getNormalAttack());
                inventoryPhysicalDamageText.text = (playerManager.getAttack() + playerManager.getEquipedWeapon().attack).ToString();
            }else
            {
                excessDamage = (playerInventory.GetWeaponsList()[_index].getNormalAttack() - 0);
                inventoryPhysicalDamageText.text = playerManager.getAttack().ToString();
            }

            if (excessDamage > 0)
                inventoryPhysicalDamageText.text += " <color=green>   (+" + excessDamage + ")</color>";
            if (excessDamage < 0)
                inventoryPhysicalDamageText.text += " <color=red>   (" + excessDamage + ")</color>";
            
            float excessCritChance = 0;
            // Check if theres a weapon equiped
            if(playerManager.getEquipedWeapon() != null)
            {
                excessCritChance = (playerInventory.GetWeaponsList()[_index].getCritChance() - playerManager.getEquipedWeapon().getCritChance());
                if (playerManager.getEquipedWeapon().getCritChance() != -1)
                    inventoryCriticalChanceText.text = playerManager.getEquipedWeapon().getCritChance().ToString() + "%";
                else
                    inventoryCriticalChanceText.text = "0%";
            }
            else
            {
                if (playerInventory.GetWeaponsList()[_index].getCritChance() != -1)
                {
                    excessCritChance = playerInventory.GetWeaponsList()[_index].getCritChance();
                }
                inventoryCriticalChanceText.text = "0%";
            }

            if (excessCritChance > 0)
                inventoryCriticalChanceText.text += " <color=green>  (+" + (excessCritChance-1) + ")</color>";
            if (excessCritChance < 0)
                inventoryCriticalChanceText.text += " <color=red>  (" + (excessCritChance+1) + ")</color>";
            #endregion

            StopCoroutine("DoubleClick");
            StartCoroutine("DoubleClick");

            if (playerInventory.GetWeaponsList()[_index].getCritChance() == -1)
            {
                inventoryWeaponStats.text = ""+ playerInventory.GetWeaponsList()[_index].getNormalAttack() + "\n" + "0%";
            }
            else
            {
                inventoryWeaponStats.text = "" + playerInventory.GetWeaponsList()[_index].getNormalAttack() + "\n" +
                    playerInventory.GetWeaponsList()[_index].getCritChance() + "%\n" +
                    playerInventory.GetWeaponsList()[_index].getCriticalMultiplier();
            }
            currentlySelectedInventoryWeapon = playerInventory.GetWeaponsList()[_index];

            currentlySelectedWeaponIndex = _index;
        }
    }

    public void ClickedOnPotion_Red()
    {
        // Check if there is any red potions 

        for (int i = 0; i < playerInventory.GetPotionsList().Count; i++)
        {
            if (playerInventory.GetPotionsList()[i].getPotionType() == Potion.potionType.HEALING)
            {
                potionInfoContainer.gameObject.SetActive(true);
                weaponInfoContainer.gameObject.SetActive(false);
                armorInfoHolder.gameObject.SetActive(false);

                inventoryPotionImage.sprite = BaseValues.healthPotionSprite;
                inventoryPotionStat.text = "Type: Healing";
                inventoryHealthAddedText.text = "<color=green>(+" + playerManager.getMaxHealth() * BaseValues.healthPotionFactor + ")</color>";

                StopCoroutine("DoubleClick_Potion");
                StartCoroutine("DoubleClick_Potion");
                currentlySelectedPotionIndex = i;
            }
        }
    }

    public void ClickedOnPotion_Blue()
    {
        // Check if there is any blue potions
        
        for (int i = 0; i < playerInventory.GetPotionsList().Count; i++)
        {
            if (playerInventory.GetPotionsList()[i].getPotionType() == Potion.potionType.STRENTGH)
            {
                potionInfoContainer.gameObject.SetActive(true);
                weaponInfoContainer.gameObject.SetActive(false);
                armorInfoHolder.gameObject.SetActive(false);

                inventoryPotionImage.sprite = BaseValues.strengthPotionSprite;
                inventoryPotionStat.text = "Type: Strength";
                inventoryHealthAddedText.text = string.Empty;

                StopCoroutine("DoubleClick_Potion");
                StartCoroutine("DoubleClick_Potion");
                currentlySelectedPotionIndex = i;
            }
        }
    }

    public void ClickedOnArmor(int _index)
    {
        if(playerInventory.GetArmorList().Count > _index)
        {
            armorInfoHolder.gameObject.SetActive(true);
            potionInfoContainer.gameObject.SetActive(false);
            weaponInfoContainer.gameObject.SetActive(false);

            StopCoroutine("DoubleClick_Armor");
            StartCoroutine("DoubleClick_Armor");

            armorImage.sprite = playerInventory.GetArmorList()[_index].getArmorSprite();
            armorInfoStat.text = (playerInventory.GetArmorList()[_index].getArmor()*100).ToString();
            inventoryArmorName.text = playerInventory.GetArmorList()[_index].getName();

            currentlySelectedArmorIndex = _index;
            currentlySelectedInventoryArmor = playerInventory.GetArmorList()[_index];
        }
    }

    // Equips the currently inventory selected weapon
    public void EquipSelectedWeapon()
    {
        if (currentlySelectedInventoryWeapon != null)
        {
            playerManager.EquipWeapon(currentlySelectedInventoryWeapon);

            currentlySelectedInventoryWeapon = null;
        }
    }

    public void EquipSelectedArmor()
    {
        if (currentlySelectedInventoryArmor != null)
        {
            playerManager.EquipArmor(currentlySelectedInventoryArmor);

            currentlySelectedInventoryArmor = null;
        }
    }

    public void ClickedEquipedWeapon()
    {
        if(playerManager.getEquipedWeapon() != null)
        {
            for(int i = 0; i < playerInventory.GetWeaponsList().Count; i++)
            {
                if(playerInventory.GetWeaponsList()[i].name_ == playerManager.getEquipedWeapon().name_)
                {
                    GoTo_WeaponsTab();
                    ClickedOnWeapon(i);
                }
            }
        }
    }

    public void ClickedOnEquipedArmor()
    {
        if(playerManager.getEquipedArmor() != null)
        {
            for (int i = 0; i < playerInventory.GetArmorList().Count; i++)
            {
                if (playerInventory.GetArmorList()[i].name_ == playerManager.getEquipedArmor().name_)
                {
                    GoTo_ArmorTab();
                    ClickedOnArmor(i);
                }
            }
        }
    }

    public void DrinkPotion()
    {
        potionInfoContainer.gameObject.SetActive(false);
        // Remove the potion from the players potions inventory
        playerManager.ConsumePotion(playerInventory.GetPotionsList()[currentlySelectedPotionIndex].getPotionType());
        playerInventory.RemovePotionAt(currentlySelectedPotionIndex);
        UpdatePotionSlots();
    }

    public void UpdatePotionSlots()
    {
        // Update Text numberOfRedPotions and numberOfBluePotions
        int redPotions = 0;
        int bluePotions = 0;

        for(int i = 0; i < playerInventory.GetPotionsList().Count; i++)
        {
            if(playerInventory.GetPotionsList()[i].getPotionType() == Potion.potionType.HEALING)
            {
                redPotions++;
            }
            else if(playerInventory.GetPotionsList()[i].getPotionType() == Potion.potionType.STRENTGH)
            {
                bluePotions++;
            }
        }

        numberOfRedPotions.text = "x"+redPotions.ToString();
        numberOfBluePotions.text = "x" + bluePotions.ToString();

        inGameRedPotion.text = "x" + redPotions.ToString();
        inGameBluePotion.text = "x" + bluePotions.ToString();
    }

    public void RemoveSelectedWeapon()
    {
        if (playerInventory.GetWeaponsList()[currentlySelectedWeaponIndex] == playerManager.getEquipedWeapon())
        {
            playerManager.EquipWeapon(null);
            NewPlayerValues();
        }
        eventBox.addEvent("Removed " + playerInventory.GetWeaponsList()[currentlySelectedWeaponIndex].getName());
        weaponInfoContainer.gameObject.SetActive(false);
        playerInventory.RemoveWeaponAt(currentlySelectedWeaponIndex);
        UpdateWeaponSlots();
    }

    public void RemoveSelectedArmor()
    {
        if(playerInventory.GetArmorList()[currentlySelectedArmorIndex] == playerManager.getEquipedArmor())
        {
            playerManager.EquipArmor(null);
            NewPlayerValues();
        }
        eventBox.addEvent("Removed" + playerInventory.GetArmorList()[currentlySelectedArmorIndex].getName());
        armorInfoHolder.gameObject.SetActive(false);
        playerInventory.RemoveArmorAt(currentlySelectedArmorIndex);
        UpdateArmorSlots();
    }

    // Assigns the right weaponsSlots to the i index of players weapon list
    public void UpdateWeaponSlots()
    {
        for(int i = 0; i < weaponSlots.Length; i++)
        {
            if (playerInventory.GetWeaponsList().Count > i)
            {
                weaponSlots[i].color = Color.white;
                weaponSlots[i].sprite = playerInventory.GetWeaponsList()[i].getWeaponSprite();
            }
            else
            {
                weaponSlots[i].sprite = null;
                weaponSlots[i].color = Color.clear;
            }
        }
    }

    public void UpdateArmorSlots()
    {
        for(int i = 0; i < armorSlots.Length; i++)
        {
            if(playerInventory.GetArmorList().Count > i)
            {
                armorSlots[i].color = Color.white;
                armorSlots[i].sprite = playerInventory.GetArmorList()[i].getArmorSprite();
            }
            else
            {
                armorSlots[i].color = Color.clear;
                armorSlots[i].sprite = null;
            }
        }
    }

    // Update the player values
    public void UpdateEnemyUI(Enemy enemy)
    {
        if (enemy != null)
        {
            enemyStatScreen.gameObject.SetActive(true);

            // The text after the attack symbol
            //enemyDamageText.text = enemy.getAttack().ToString(); 
            //enemyDamageText.text = enemy.getAverageAttack().ToString();
            enemyDamageText.text = enemy.attack1 + " - " + enemy.attack2;

            // Setting up the slider *health
            enemyHealthSlider.maxValue = enemy.getMaxHP();

            enemyHealthSlider.value = enemy.getHP();

            // The text at the top ie the name and health text
            enemyNameText.text = enemy.getName();
            enemyHealthText.text = (Mathf.RoundToInt(enemy.getHP()) + "/" + Mathf.RoundToInt(enemy.getMaxHP()));
        }
    }

    public void AddedNewMoney(int amount)
    {
        newMoneyText.text = "+" + amount.ToString();
        newMoneyText.color = Color.white;
    }


    public void DisableEnemyUI()
    {
        enemyStatScreen.gameObject.SetActive(false);
    }

    // Enables the window that asks if the player wants to go to the next floor
    public void PromptNextFloor()
    {
        nextFloorPrompt.gameObject.SetActive(true);
    }

    public void DisableNextFloorPrompt() { nextFloorPrompt.gameObject.SetActive(false); }
    public void ToggleExtraStats() { /* Active the extra inventory screen once its implemented */ }

    #region inventory screen
    public void ToggleInventoryScreen()
    {
        //potionTab.gameObject.SetActive(false);
        //weaponsTab.gameObject.SetActive(false);
        if (!characterInventory.gameObject.activeSelf)
        {
            soundManager.OpenedInventory();
        }
        weaponInfoBox.gameObject.SetActive(false);
        inventory.Toggled();
        characterInventory.gameObject.SetActive(!characterInventory.gameObject.activeSelf);
    }
    public void GoTo_WeaponsTab()
    {
        weaponInfoBox.gameObject.SetActive(false);

        weaponsTab.gameObject.SetActive(true);
        potionTab.gameObject.SetActive(false);
        armorTab.gameObject.SetActive(false);
    }
    public void GoTo_PotionTab()
    {
        weaponInfoBox.gameObject.SetActive(false);

        weaponsTab.gameObject.SetActive(false);
        potionTab.gameObject.SetActive(true);
        armorTab.gameObject.SetActive(false);
    }
    public void GoTo_ArmorTab()
    {
        weaponInfoBox.gameObject.SetActive(false);

        armorTab.gameObject.SetActive(true);
        potionTab.gameObject.SetActive(false);
        weaponsTab.gameObject.SetActive(false);
    }
    #endregion

    public void NextFloor()
    {
        DisableNextFloorPrompt();
        playerManager.StartCoroutine(playerManager.AscendNextFloor());
        //floorManager.NewFloor();
    }
    public void OnNewFloor(bool isShop)
    {
        DisableEnemyUI();
        if (isShop)
        {
            currentFloorText.text = "Shop";
        }
        else
        {
            currentFloorText.text = "Floor  " + floorManager.getCurrentFloor();
        }
    }
    public void GameOver()
    {
        StartCoroutine("gameOver");

    }
    public void LoadScene(string _name)
    {
        SceneManager.LoadScene(_name);
    }
    
    public void setHPremovelEffectSlider(float health)
    {
        healthRemovedSlider.maxValue = playerManager.getMaxHealth();
        healthRemovedSlider.value = health;

        healthRemovedSliderImage.color = Color.white;
    }

    public void FullyExploredMap()
    {
        fullyExploredMapTimer = 3;
        fullyExploredMap.color = Color.white;
    }

    public void ConfirmArmor(Armor armor)
    {
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(playerManager.ConfirmArmor_PickUp);

        /* Setting icon and sprites */
        icon1.sprite = BaseValues.armorSymbolSprite;
        icon2.sprite = null;
        icon2.color = Color.clear;
        stat1.text = string.Empty;
        stat2.text = string.Empty;

        confirmWeapon.gameObject.SetActive(true);

        itemName.text = armor.getName();

        /* Defense Points DP(hehe) */
        stat1.text = armor.armorPercentage.ToString();
    }

    public void ConfirmWeapon(Weapon weapon)
    {
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(playerManager.ConfirmWeapon_PickUp);

        /* Setting icon and sprites */
        icon1.sprite = BaseValues.attackSymbolSprite;
        icon2.sprite = BaseValues.criticalSymbolSprite;
        icon2.color = Color.white;
        stat1.text = string.Empty;
        stat2.text = string.Empty;

        confirmWeapon.gameObject.SetActive(true);

        itemName.text = weapon.getName();

        /* Weapon - Critical */
        stat1.text = weapon.getNormalAttack().ToString();
        if (weapon.getCritChance() != -1)
            stat2.text = weapon.getCritChance().ToString() + "%";
        else
            stat2.text = "0%";
    }

    IEnumerator DoubleClick_Armor()
    {
        float timer = 0;
        while (timer <= doubleClickInterval)
        {
            if (Input.GetMouseButtonDown(0))
            {
                EquipSelectedArmor();
                break;
            }

            timer += Time.deltaTime;

            yield return null;
        }
    }

    IEnumerator DoubleClick_Potion()
    {
        float timer = 0;
        while (timer <= doubleClickInterval)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DrinkPotion();
                break;
            }

            timer += Time.deltaTime;

            yield return null;
        }
    }

    IEnumerator DoubleClick()
    {
        //print("Started Double Click");

        float timer = 0;
        while(timer <= doubleClickInterval)
        {
            if (Input.GetMouseButtonDown(0))
            {
                EquipSelectedWeapon();
                break;
            }

            timer += Time.deltaTime;

            yield return null;
        }
    }

    IEnumerator FadeIn()
    {
        fadePanel.color = Color.black;

        while (fadePanel.color.a > 0)
        {
            fadePanel.color = new Color(0, 0, 0, fadePanel.color.a - 0.01f);
            yield return new WaitForSeconds(0.01f);
        }

        fadePanel.color = Color.clear;
    }

    IEnumerator gameOver()
    {

        fadePanel.color = Color.clear;

        while (fadePanel.color.a < 1)
        {
            fadePanel.color = new Color(0, 0, 0, fadePanel.color.a + 0.01f);
            yield return new WaitForSeconds(0.01f);
        }

        fadePanel.color = Color.black;

        fadePanelGameOver.color = Color.black;
        fadePanel.color = new Color(0, 0, 0, 0.55f);
        gameOverScreen.gameObject.SetActive(true);
        while (fadePanelGameOver.color.a > 0)
        {
            fadePanelGameOver.color = new Color(0, 0, 0, fadePanelGameOver.color.a - 0.01f);
            yield return new WaitForSeconds(0.01f);
        }

        fadePanelGameOver.color = Color.clear;
    }

    public void ActivateIntro()
    {
        IntroScreen.gameObject.SetActive(true);
    }

    public void ActivateBrandingTutorial()
    {
        brandingTutorial.gameObject.SetActive(true);
    }

    public void enableEscapePrompt()
    {
        escapePrompt.gameObject.SetActive(true);
    }

    public void disableEscapePrompt()
    {
        escapePrompt.gameObject.SetActive(false);
    }

    public void disableVersion1Transform()
    {
        version1EndTransform.gameObject.SetActive(false);
    }

    // In game potion code
    public void InGame_ClickedOnRedPotion()
    {
        int count = playerInventory.GetPotionTypeCount(Potion.potionType.HEALING);
        if(count > 0)
        {
            if(inGameRedPotionConfirm.color.a == 1)
            {
                inGameRedPotionConfirm.color = Color.clear;
                for (int i = 0; i < playerInventory.GetPotionsList().Count; i++)
                {
                    if (playerInventory.GetPotionsList()[i].getPotionType() == Potion.potionType.HEALING)
                    {
                        playerInventory.GetPotionsList().RemoveAt(i);
                        playerManager.ConsumePotion(Potion.potionType.HEALING);
                        UpdatePotionSlots();
                        NewPlayerValues();
                        return;
                    }
                }
            }
            else
            {
                inGameRedPotionConfirm.color = Color.white;
                StopCoroutine("InGame_ResetRedPotionConfirm");
                StartCoroutine("InGame_ResetRedPotionConfirm");
            }
        }
    }

    public void InGame_ClickedOnBluePotion()
    {
        int count = playerInventory.GetPotionTypeCount(Potion.potionType.STRENTGH);
        if (count > 0)
        {
            if (inGameBluePotionConfirm.color.a == 1)
            {
                inGameBluePotionConfirm.color = Color.clear;
                for (int i = 0; i < playerInventory.GetPotionsList().Count; i++)
                {
                    if (playerInventory.GetPotionsList()[i].getPotionType() == Potion.potionType.STRENTGH)
                    {
                        playerInventory.GetPotionsList().RemoveAt(i);
                        playerManager.ConsumePotion(Potion.potionType.STRENTGH);
                        UpdatePotionSlots();
                        NewPlayerValues();
                        return;
                    }
                }
            }
            else
            {
                inGameBluePotionConfirm.color = Color.white;
                StopCoroutine("InGame_ResetBluePotionConfirm");
                StartCoroutine("InGame_ResetBluePotionConfirm");
            }
        }
    }

    IEnumerator InGame_ResetRedPotionConfirm()
    {
        yield return new WaitForSeconds(1.5f);
        while(inGameRedPotionConfirm.color.a > 0)
        {
            inGameRedPotionConfirm.color = new Color(1, 1, 1, inGameRedPotionConfirm.color.a - 4 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator InGame_ResetBluePotionConfirm()
    {
        yield return new WaitForSeconds(1.5f);
        while (inGameBluePotionConfirm.color.a > 0)
        {
            inGameBluePotionConfirm.color = new Color(1, 1, 1, inGameBluePotionConfirm.color.a - 4 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}