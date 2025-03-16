using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewSample : MonoBehaviour
{
    private Color MixedRecyclingColor = new Color(1f, 0.5f, 0f);  // RGB equivalent of FF8000
    private Color ElectronicRecyclingColor = new Color(0.984f, 0.827f, 0.427f);
    private Color PaperRecyclingColor = new Color(0.345f, 0.537f, 0.247f);
    private Color LandfillColor = new Color(0.443f, 0.639f, 0.757f);
    private Color DonationColor = new Color(0.745f, 0.584f, 0.745f);
    private Color RequiresCleaningColor = new Color(0.502f, 0.361f, 0.133f);

    private Color CompostColor = new Color(0.624f, 0.220f, 0.220f);


    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private RectTransform _content;
    [SerializeField] private GameObject _prefabListItem;

    [Space(10)]
    [Header("Scroll view events")]
    [SerializeField] private ItemButtonEvent _eventItemClicked;
    [SerializeField] private ItemButtonEvent _eventItemOnSelect;
    [SerializeField] private ItemButtonEvent _eventItemOnSubmit;

    [Space(10)]
    [Header("Default Selected Index")]
    [SerializeField] private int _defaultSelectedIntex = 0;

    [Space(10)]
    [Header("For testing only")]
    //[SerializeField] private int _testButtonCount = 1;
    // Update is called once per frame

    private List<string> Compost = new List<string> {
"Wet Paper",
"Wet Cardboard Box",
"Apple",
"Broccoli",
"Coffee Grounds",
"Compostable Utensils",
"Used Napkin",
"Banana Peel",
"Pizza Box",
"Apple Core",
"Wine Bottle Cork",
"Bone",
"Tea Bag",
     };
    private List<string> MixedRecycling = new List<string>
    {
        "Water Bottle",
        "Cooking Oil Bottle",
        "Peanut Butter Container",
        "Shampoo Bottle",
        "Water Jug",
        "Tent",
        "Bread Bag",
        "Grocery Bag",
        "Plastic Bag",
        "Bottle Cap",
        "Yogurt Container",
        "Tetra Pack",
        "Juice Box",
        "Metal Pipe",
        "Metal Ingot",
        "Beer Can",
        "Aluminium Can",
        "Milk Carton",
        "Receipt",
        "Coffee Cup Sleeve",
        "Beer Bottle",
        "Wine Bottle",
        "Glass Cup",
    };
    private List<string> ElectronicRecycling = new List<string> {
        "Opus Card",
        "Chipped Credit Card",
        "9V Battery",
        "AA Battery",
        "Phone",
     };
    private List<string> PaperRecycling = new List<string> {
        "Cardboard Box",
        "Envelope",
        "Notebook"
     };
    // no sprite for ramen cup, cereal, cracker bag, stm ticket,padded envelops (the beige ones),tampons!, condoms
    private List<string> Landfill = new List<string>{
"Blister Packaging",
"Surgical Glove",
"Plastic Cling Wrap",
"Garbage Bag",
"Styrofoam Cup",
"Red Solo Cup",
"Coffee Cup Lid",
"Hand Cream Container",
"Toothpaste Tube",
"Chip Bag",
"Clay Vase",
"Broken Light Bulb",
"Chocolate Wrapper",
     };
    // no sprite for x (too many things)?
    private List<string> Donation = new List<string>{
"Vinyl Record",
"Sheet Protector",
"Toy Tyrannosaurus",
"Toy Brontosaurus",
"Sunglasses",

     };
    //no sprite for premade salad containers,  cream cheese container,medicine bottle,Filled Coffee cup, food cans
    private List<string> RequiresCleaning = new List<string>{
            "Peanut Butter Container",
            "cooking oil bottle",
            "Chemical Bottle",
            "Yogurt Container",
"Tetra Pack",
"Beer Bottle",
"Wine Bottle",
"Beer Can",
"Tea Bag",
"Aluminium Can"


          };
    private string CompostDescription = " When you put food waste (nitrogen) and combine it with a natural source of carbon (like paper, leaves) to produce compost material after you let it biodegrade. Compost material is often used in gardening to enrich the soil and provide nutrients to plants and vegetables Anything living can be composted but any animal products will attract a lot more bugs and animals than fruits and vegetables. The more mushed up something is, the less time it will take to biodegrade. Some plastics are meant for compost so look to buy products that advertise for that! However you should be careful when it comes to biodegradable plastic, not all types are made equal and some can decompose into microplastics.";
    private string MixedRecyclingDescription = "This is where you drop off most Plastic 1-5 and some of 7. Plastic type 6 does not go into recycling in Quebec. Metal and Glass bottles or cans also go here but just make sure they’re clean and dry first. Also don’t put in jagged metal or glass as it can be dangerous and won’t get recycled, for lids of cans make sure they’re separated as well.";
    private string ElectronicRecyclingDescription = "Most electronics can be dropped off here, but if it seems hazardous then double check online if the facility will take it like in the case of broken light bulbs.";
    private string PaperRecyclingDescription = "Non soiled paper, cardboard and other regular paper products go here. If regular paper or cardboard ends up wet or greasy they can’t be recycled but you can put them into compost as they’re a natural and biodegradable material! Things like waxed paper do not count as regular paper so don’t put them here!";

    private string LandfillDescription = "Objects that should always end up in the trash go here, even if it might be made of a recyclable material some things can often not be recycled easily because of the construction.";
    private string DonationDescription = "Many things don’t need to be thrown away and can be used by another person so consider donating it!";
    private string RequiresCleaningDescription = "A lot of things can use a quick rinse or more before you recycle them and sometimes this step is the difference between if it gets rejected and sent to a landfill or actually recycled.";

    void Start()
    {

        UpdateAllButtonNavigationReferences();
        StartCoroutine(DelayedSelectChild(_defaultSelectedIntex));
    }
    public void CreateItemsTutorialStage1()
    {
        // Debug.Log("Creating items for tutorial stage 1");
        ClearItems();

        TestCreateItems(Compost, Compost.Count, CompostColor);
        TestCreateItems(MixedRecycling, MixedRecycling.Count, MixedRecyclingColor);
        TestCreateItems(PaperRecycling, PaperRecycling.Count, PaperRecyclingColor);
        TestCreateItems(Landfill, Landfill.Count, LandfillColor);
        UpdateAllButtonNavigationReferences();
        StartCoroutine(DelayedSelectChild(_defaultSelectedIntex));
    }
    public void CreateItemsNoTutorialStage1()
    {
        // Debug.Log("Creating items for no tutorial stage 1");
        ClearItems();
        TestCreateItems(Compost, Compost.Count, CompostColor);
        TestCreateItems(MixedRecycling, MixedRecycling.Count, MixedRecyclingColor);
        TestCreateItems(ElectronicRecycling, ElectronicRecycling.Count, ElectronicRecyclingColor);
        TestCreateItems(PaperRecycling, PaperRecycling.Count, PaperRecyclingColor);
        TestCreateItems(Landfill, Landfill.Count, LandfillColor);
        TestCreateItems(Donation, Donation.Count, DonationColor);
        TestCreateItems(RequiresCleaning, RequiresCleaning.Count, RequiresCleaningColor);
        UpdateAllButtonNavigationReferences();
        StartCoroutine(DelayedSelectChild(_defaultSelectedIntex));
    }
    public void ClearItems()
    {
        foreach (Transform child in _content)
        {
            Destroy(child.gameObject);
        }
    }
    public void SelectChild(int index)
    {
        int childCount = _content.transform.childCount;

        if (index >= childCount)
        {
            return;
        }

        GameObject childObject = _content.transform.GetChild(index).gameObject;
        ItemButton item = childObject.GetComponent<ItemButton>();
        item.ObtainSelectionFocus();
    }

    public IEnumerator DelayedSelectChild(int index)
    {
        yield return new WaitForSeconds(1f);
        SelectChild(index);
    }
    private void UpdateAllButtonNavigationReferences()
    {
        ItemButton[] children = _content.transform.GetComponentsInChildren<ItemButton>();
        if (children.Length < 2)
        {
            return;
        }
        ItemButton item;
        Navigation navigation;

        for (int i = 0; i < children.Length; i++)
        {
            item = children[i];
            navigation = item.gameObject.GetComponent<Button>().navigation;
            navigation.selectOnLeft = GetNavigationLeft(i, children.Length);
            navigation.selectOnRight = GetNavigationRight(i, children.Length);
            navigation.selectOnUp = GetNavigationUp(i, children.Length);
            navigation.selectOnDown = GetNavigationDown(i, children.Length);

            item.gameObject.GetComponent<Button>().navigation = navigation;
        }
    }

    // looping navigation
    private Selectable GetNavigationRight(int indexCurrent, int totalEntires)
    {
        ItemButton item;
        if (indexCurrent == totalEntires - 1)
        {
            item = _content.transform.GetChild(0).GetComponent<ItemButton>();
        }
        else
        {
            item = _content.transform.GetChild(indexCurrent + 1).GetComponent<ItemButton>();
        }
        return item.GetComponent<Selectable>();
    }
    private Selectable GetNavigationDown(int indexCurrent, int totalEntires)
    {
        ItemButton item;
        if (indexCurrent <= (totalEntires - 1) && indexCurrent >= (totalEntires - 13))
        {
            item = _content.transform.GetChild(0).GetComponent<ItemButton>();
        }
        else
        {
            item = _content.transform.GetChild(indexCurrent + 12).GetComponent<ItemButton>();
        }
        return item.GetComponent<Selectable>();
    }
    private Selectable GetNavigationUp(int indexCurrent, int totalEntires)
    {
        ItemButton item;
        if (indexCurrent <= 11 && indexCurrent >= 0)
        {
            item = _content.transform.GetChild(totalEntires - 1).GetComponent<ItemButton>();
        }
        else
        {
            item = _content.transform.GetChild(indexCurrent - 12).GetComponent<ItemButton>();
        }
        return item.GetComponent<Selectable>();
    }

    private Selectable GetNavigationLeft(int indexCurrent, int totalEntires)
    {
        ItemButton item;
        if (indexCurrent == 0)
        {
            item = _content.transform.GetChild(totalEntires - 1).GetComponent<ItemButton>();
        }
        else
        {
            item = _content.transform.GetChild(indexCurrent - 1).GetComponent<ItemButton>();
        }
        return item.GetComponent<Selectable>();
    }

    private void TestCreateItems(List<string> array, int count, Color color)
    {
        for (int i = 0; i < count; i++)
        {

            CreateItem(array[i], color);
        }
    }
    private ItemButton CreateItem(string strName, Color MixedRecyclingColor)
    {
        GameObject gObj;
        ItemButton item;
        Image grandchild;

        gObj = Instantiate(_prefabListItem, Vector3.zero, quaternion.identity);
        gObj.transform.SetParent(_content.transform);
        gObj.transform.localScale = new Vector3(1f, 1f, 1f);
        gObj.transform.localPosition = new Vector3();
        gObj.transform.localRotation = Quaternion.Euler(new Vector3());
        gObj.name = strName;

        Transform childTransform = gObj.transform.GetChild(0);
        Image child = childTransform.GetComponent<Image>();

        if (child != null)
        {
            child.color = MixedRecyclingColor;
        }
        else
        {
            Debug.LogWarning("No Image component found on grandchild!");
        }
        Transform grandChildTransform = gObj.transform.GetChild(0).GetChild(0);
        grandchild = grandChildTransform.GetComponent<Image>();
        if (grandchild != null)
        {
            Sprite newSprite = Resources.Load<Sprite>("Sprites/Enemies/" + strName);
            if (newSprite != null)
            {
                grandchild.sprite = newSprite;
            }
            else
            {
                Debug.LogWarning("Failed to load sprite from Resources.");
            }
        }
        else
        {
            Debug.LogWarning("Grandchild Image component not found!");
        }
        item = gObj.GetComponent<ItemButton>();
        item.ItemNameValue = strName;
        item.OnSelectEvent.AddListener((ItemButton) => { HandleEventItemOnSelect(item); });
        item.OnClickEvent.AddListener((ItemButton) => { HandleEventItemOnClick(item); });
        item.OnSubmitEvent.AddListener((ItemButton) => { HandleEventItemOnSubmit(item); });

        return item;
    }

    private void HandleEventItemOnClick(ItemButton item)
    {
        _eventItemClicked.Invoke(item);
    }

    private void HandleEventItemOnSubmit(ItemButton item)
    {
        _eventItemOnSubmit.Invoke(item);
    }

    private void HandleEventItemOnSelect(ItemButton item)
    {
        descriptionText.text = " ";
        string categoryName = "";
        string extraText = "make sure to wash this before disposing of it properly! ";
        ScrollViewAutoScroll scrollViewAutoScroll = GetComponent<ScrollViewAutoScroll>();
        scrollViewAutoScroll.HandleOnSelectChange(item.gameObject);
        if (Compost.Contains(item.name))
        {
            descriptionText.text = CompostDescription;
            if (item.name == "Tea Bag")
            {
                descriptionText.text = extraText + descriptionText.text;
            }
            categoryName = "Compost";
        }
        else if (MixedRecycling.Contains(item.name))
        {
            descriptionText.text = MixedRecyclingDescription;
            if (item.name == "Peanut Butter Container" || item.name == "cooking oil bottle" || item.name == "Yogurt Container" || item.name == "Tetra Pack" || item.name == "Beer Bottle" || item.name == "Wine Bottle" || item.name == "Beer Can" || item.name == "Aluminium Can")
            {
                descriptionText.text = extraText + descriptionText.text;
            }
            categoryName = "Mixed Recycling";
        }
        else if (ElectronicRecycling.Contains(item.name))
        {
            descriptionText.text = ElectronicRecyclingDescription;
            categoryName = "Electronic Recycling";
        }
        else if (PaperRecycling.Contains(item.name))
        {
            descriptionText.text = PaperRecyclingDescription;
            categoryName = "Paper Recycling";
        }
        else if (Landfill.Contains(item.name))
        {
            descriptionText.text = LandfillDescription;
            categoryName = "Landfill";
        }
        else if (Donation.Contains(item.name))
        {
            descriptionText.text = DonationDescription;
            categoryName = "Donation";
        }
        else if (RequiresCleaning.Contains(item.name))
        {
            descriptionText.text = RequiresCleaningDescription;
            categoryName = "Requires Cleaning";
        }
        titleText.text = item.name + " (" + categoryName + ")";
        _eventItemOnSelect.Invoke(item);
    }

}
