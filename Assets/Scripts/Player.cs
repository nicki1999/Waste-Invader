using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public Projectile[] laserPrefab;

    public Sprite hitSprite;
    public Sprite idleSprite;
    public Sprite[] movingSprites;
    public Sprite[] shootSprites;
    public float animationSpeed;

    private int shootingFrame;
    private int movingFrame;
    private bool shooting;
    private bool damaged;
    public bool moving;
    public bool movingLeft;

    public int selectedWeapon = 0;
    public int Health;
    public int maxHealth;
    public Slider healthSlider;

    public bool flashing;

    public bool laserActive { get; private set; }

    public Text neededWeaponText;
    //public Text neededWeaponNumberText;
    public Image neededWeaponImage;
    public Sprite[] neededWeaponImages;

    public Color[] WeaponColors;
    public LayerMask EnemyMask;

    public System.Action killed;

    private SpriteRenderer spriteRenderer;

    public AudioManager audioManager;

    public GameManager gameManager;

    public bool Tutorial = true;
    private bool leftArrowClicked = false;
    private bool rightArrowClicked = false;

    public GameObject leftArrow;
    public Sprite leftArrowPressed;
    public GameObject rightArrow;
    public Sprite righttArrowPressed;
    private Image leftArrowImage;
    private Image rightArrowImage;
    private Sprite normalLeftArrow;
    private Sprite normalRightArrow;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(AnimatePlayer), animationSpeed, animationSpeed);
        leftArrowImage = leftArrow.GetComponent<Image>();
        rightArrowImage = rightArrow.GetComponent<Image>();
        normalLeftArrow = leftArrowImage.sprite;
        normalRightArrow = rightArrowImage.sprite;

    }
    public void MoveLeftButton()
    {
        leftArrowClicked = true;
        leftArrowImage.sprite = leftArrowPressed;
    }
    public void MoveLeftButtonStop()
    {
        leftArrowClicked = false;
        leftArrowImage.sprite = normalLeftArrow;
    }
    // Handles movement and shooting
    public void MoveRightButton()
    {
        rightArrowClicked = true;
        rightArrowImage.sprite = righttArrowPressed;
    }
    public void MoveRightButtonStop()
    {
        rightArrowClicked = false;
        rightArrowImage.sprite = normalRightArrow;
    }
    // Handles movement and shooting
    private void Update()
    {
        Vector3 position = transform.position;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || Input.GetAxis("Horizontal") < -0.25f || leftArrowClicked)
        {
            position.x -= speed * Time.deltaTime;
            movingLeft = true;
            moving = true;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || Input.GetAxis("Horizontal") > 0.25f || rightArrowClicked)
        {
            position.x += speed * Time.deltaTime;
            movingLeft = false;
            moving = true;
        }

        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.LeftArrow) && Input.GetAxis("Horizontal") >= -0.25f && !leftArrowClicked &&
            !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.RightArrow) && Input.GetAxis("Horizontal") <= 0.25f && !rightArrowClicked)
        {
            moving = false;
        }


        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            selectedWeapon = 0;
            if (!laserActive)
                audioManager.Play("Shoot1");
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            selectedWeapon = 1;
            if (!laserActive)
                audioManager.Play("Shoot2");
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Joystick1Button2))
        {
            selectedWeapon = 2;
            if (!laserActive)
                audioManager.Play("Shoot2");
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Joystick1Button3))
        {
            selectedWeapon = 3;
            if (!laserActive)
                audioManager.Play("Shoot4");
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Joystick1Button4))
        {
            selectedWeapon = 4;
            if (!laserActive)
                audioManager.Play("Shoot5");
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Joystick1Button5))
        {
            selectedWeapon = 5;
            if (!laserActive)
                audioManager.Play("Shoot6");
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Joystick1Button6))
        {
            selectedWeapon = 6;
            if (!laserActive)
                audioManager.Play("Shoot7");
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Joystick1Button7))
        {
            selectedWeapon = 7;
            if (!laserActive)
                audioManager.Play("Shoot8");
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (killed != null)
                killed.Invoke();
        }

        // Clamps to prevent player from moving off the map
        position.x = Mathf.Clamp(position.x, Camera.main.ViewportToWorldPoint(Vector3.zero).x + 6.6f, Camera.main.ViewportToWorldPoint(Vector3.right).x - 6.6f);
        transform.position = position;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.up), 30f, EnemyMask);

        if (hit)
        {
            int hitEnemy = -1;

            if (gameManager.Stage == 1)
            {
                hitEnemy = hit.collider.gameObject.GetComponent<Enemy>().Stage1EnemyType[0] - 1;
            }
            else if (gameManager.Stage == 2)
            {
                hitEnemy = hit.collider.gameObject.GetComponent<Enemy>().Stage2EnemyType[0] - 1;
            }

            if (hit.collider.gameObject.GetComponent<Enemy>() != null || hit.collider.gameObject.GetComponent<TutorialEnemyScript>() != null)
            {
                if (hit.collider.gameObject.GetComponent<Enemy>())
                    neededWeaponText.text = hit.collider.gameObject.GetComponent<Enemy>().EnemyName;
                else
                {
                    neededWeaponText.text = hit.collider.gameObject.GetComponent<TutorialEnemyScript>().EnemyName;
                    hitEnemy = hit.collider.gameObject.GetComponent<TutorialEnemyScript>().Stage1EnemyType - 1;
                }

                if (gameManager.Stage <= 1)
                {
                    /*if (hitEnemy == 0)
                    {
                        //neededWeaponNumberText.text = "1";
                        neededWeaponImage.sprite = neededWeaponImages[1];
                    }
                    else if (hitEnemy == 1)
                    {
                        //neededWeaponNumberText.text = "2";
                        neededWeaponImage.sprite = neededWeaponImages[2];
                    }
                    else if (hitEnemy == 2)
                    {
                        //neededWeaponNumberText.text = "3";
                        neededWeaponImage.sprite = neededWeaponImages[3];
                    }
                    else if (hitEnemy == 3)
                    {
                        //neededWeaponNumberText.text = "4";
                        neededWeaponImage.sprite = neededWeaponImages[4];
                    }
                    else if (hitEnemy == 4)
                    {
                        //neededWeaponNumberText.text = "5";
                        neededWeaponImage.sprite = neededWeaponImages[5];
                    }
                    else if (hitEnemy == 5)
                    {
                        //neededWeaponNumberText.text = "6";
                        neededWeaponImage.sprite = neededWeaponImages[6];
                    }
                    else if (hitEnemy == 6)
                    {
                        //neededWeaponNumberText.text = "7";
                        neededWeaponImage.sprite = neededWeaponImages[7];
                    }
                    else if (hitEnemy == 7)
                    {
                        //neededWeaponNumberText.text = "8";
                        neededWeaponImage.sprite = neededWeaponImages[8];
                    }
                    else*/
                    if (hit.collider.gameObject.GetComponent<Enemy>())
                    {
                        neededWeaponImage.sprite = hit.collider.gameObject.GetComponent<Enemy>().animationImages[0];
                    }
                    else if (hit.collider.gameObject.GetComponent<TutorialEnemyScript>())
                    {
                        neededWeaponImage.sprite = hit.collider.gameObject.GetComponent<TutorialEnemyScript>().animationImages[0];
                    }
                    //neededWeaponImage.color = hit.collider.gameObject.GetComponent<SpriteRenderer>().color;
                }
                else if (gameManager.Stage == 2)
                {
                    neededWeaponImage.sprite = hit.collider.gameObject.GetComponent<Enemy>().animationImages[0];
                    //neededWeaponImage.color = Color.white;
                }
            }
        }
        else
        {
            neededWeaponText.text = "";
            //neededWeaponNumberText.text = "";
            neededWeaponImage.sprite = neededWeaponImages[0];
        }

        if (movingLeft)
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    // Shoots a laser if one does not already exist
    public void Shoot()
    {
        shooting = true;
        if (!laserActive)
        {
            laserActive = true;

            Projectile laser = Instantiate(laserPrefab[selectedWeapon], transform.position, Quaternion.identity);
            laser.destroyed += OnLaserDestroyed;
        }
    }

    // On destruction, sets laser bool to false to allow next to be shot
    private void OnLaserDestroyed(Projectile laser)
    {
        laserActive = false;
    }

    // When touching an enemy, die
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (killed != null)
                killed.Invoke();
        }
    }

    public void IncorrectHit()
    {
        StartCoroutine(FlashPlayer());
        Health -= 1;
        if (Health == 0)
        {
            spriteRenderer.color = new Color(1, 1, 1, 1);
            flashing = false;
            StopCoroutine(FlashPlayer());
            if (killed != null)
                killed.Invoke();
        }
        damaged = true;

        healthSlider.value = Health;
    }

    public void FireWeaponButton(int selectedWeapon, string audioType)
    {
        this.selectedWeapon = selectedWeapon;
        if (!laserActive)
            audioManager.Play(audioType);
        Shoot();
    }
    private IEnumerator FlashPlayer()
    {
        if (flashing)
            yield return null;
        else
        {
            flashing = true;
            for (int i = 0; i < 5; i++)
            {
                spriteRenderer.color = new Color(1, 0, 0, 1);
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.color = new Color(1, 1, 1, 1);
                yield return new WaitForSeconds(0.1f);
            }
            flashing = false;
            yield return null;
        }
    }

    private void AnimatePlayer()
    {
        if (shooting)
        {
            shootingFrame++;

            if (shootingFrame >= shootSprites.Length)
            {
                shootingFrame = 0;
                shooting = false;
            }

            spriteRenderer.sprite = shootSprites[shootingFrame];
        }
        else if (moving)
        {
            movingFrame++;

            if (movingFrame >= movingSprites.Length)
            {
                movingFrame = 0;
            }

            spriteRenderer.sprite = movingSprites[movingFrame];
        }
        else if (damaged)
        {
            spriteRenderer.sprite = hitSprite;
            damaged = false;
        }
        else
        {
            spriteRenderer.sprite = idleSprite;
        }
    }

    private void OnDisable()
    {
        spriteRenderer.color = new Color(1, 1, 1, 1);
        flashing = false;
        StopCoroutine(FlashPlayer());
    }
}
