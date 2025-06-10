using UnityEngine;

public class StatusSystem : MonoBehaviour
{
    [Header("Status system")]
    [SerializeField] private GameObject hpBar;
    [SerializeField] private GameObject kiBar;
    private int maxHp = 3000, maxKi = 100;
    [SerializeField] private int currentHp, currentKi;
    private Color originalColor;
    private Setting settingScript;
    private void Awake()
    {
        currentHp = maxHp;
        currentKi = maxKi;

        if (hpBar == null)
        {
            hpBar = GameObject.Find("GameController/System/Player/Hp/Hp bar");
        }

        if (kiBar == null)
        {
            kiBar = GameObject.Find("GameController/System/Player/Ki/Ki bar");
        }
        settingScript = FindObjectOfType<Setting>();
        if (settingScript == null)
        {
            Debug.LogError("Setting script not found!");
        }
    }


    private void Start()
    {
        SpriteRenderer spriteRenderer = hpBar.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void Update()
    {
        UpdateSystem();
    }
    public void UpdateSystem()
    {
        float healthPercentage = (float)currentHp / maxHp;
        float kiPercentage = (float)currentKi / maxKi;
        hpBar.transform.localScale = new Vector3(healthPercentage, 1f);
        kiBar.transform.localScale = new Vector3(kiPercentage, 1f);
        currentKi = Mathf.Clamp(currentKi, 0, maxKi);
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        //Ping pong change color
        if (healthPercentage < 0.3f)
        {
            float pingPongValue = Mathf.PingPong(Time.time * 2f, 1f);
            Color barColor = new Color(0.0f, 0.6f, 0.2f);
            Color pingPongColor = Color.Lerp(barColor, Color.blue, pingPongValue);
            SetColor(pingPongColor);
        }
        else
        {
            SetColor(originalColor);
        }
    }

    public void SetColor(Color color)
    {
        SpriteRenderer spriteR = hpBar.GetComponent<SpriteRenderer>();
        if (spriteR != null)
        {
            spriteR.color = color;
        }
        else
        {
            Debug.LogError("Sprite renderer null");
        }
    }

    public void TakeDamage(int dmg)
    {
        CurrentHp -= dmg;
        if(CurrentHp <= 0)
        {
            gameObject.SetActive(false);
            currentHp = 0;
            Debug.Log("Died");
            if (settingScript != null)
            {
                settingScript.ShowPlayAgainPanel();
            }
            Time.timeScale = 0f;
            return;
        }
        Debug.Log("- hp ");
    }

    public void UseKi(int amount)
    {
        CurrentKi -= amount;
        Debug.Log($"Use {amount} ki");
        if (CurrentKi < 0) CurrentKi = 0;
    }

    public void AuraKi(int amount)
    {
        CurrentKi += amount;
        if(CurrentKi > MaxKi) CurrentKi = MaxKi;
    }

    //Getter, setter
    public int MaxHp {  get =>  maxHp; set => maxHp = value;}
    public int MaxKi { get => maxKi; set => maxKi = value;}
    public int CurrentHp { get => currentHp; set => currentHp = Mathf.Clamp(value,0,maxHp);}
    public int CurrentKi { get => currentKi; set => currentKi = Mathf.Clamp(value, 0, maxKi); }
}
