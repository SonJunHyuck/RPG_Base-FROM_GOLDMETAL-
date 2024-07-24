using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Boss boss;
    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject startZone;

    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;
    public int EnemisCnt
    {
        get { return enemyCntA + enemyCntB + enemyCntC + enemyCntD; }
    }

    [Header("InGame")]
    public Transform[] enemyZones;
    public GameObject[] enemyPrefabs;
    public List<int> enemyList;

    [Header("UI")]
    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject gameOverPanel;
    public Text maxScoreText;
    public Text scoreText;
    public Text stageText;
    public Text playTimeText;
    public Text playerHealthText;
    public Text playerAmmoText;
    public Text playerCoinText;
    public Image weaponImage1;
    public Image weaponImage2;
    public Image weaponImage3;
    public Image weaponImageR;
    public Text enemyAText;
    public Text enemyBText;
    public Text enemyCText;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;

    [Header("GameOver")]
    public Text curScoreText;
    public Text bestText;

    private void Awake()
    {
        enemyList = new List<int>();
        maxScoreText.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));

        if (!PlayerPrefs.HasKey("MaxScore"))
            PlayerPrefs.SetInt("MaxScore", 0);
    }

    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }

    public void StageStart()
    {
        isBattle = true;
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);

        foreach (Transform zone in enemyZones)
        {
            zone.gameObject.SetActive(true);
        }

        StartCoroutine(InBattle());
    }

    public void StageEnd()
    {
        player.transform.position = Vector3.up * 2.7f;

        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZone.SetActive(true);

        foreach(Transform zone in enemyZones)
        {
            zone.gameObject.SetActive(false);
        }

        isBattle = false;
        stage++;
    }

    IEnumerator InBattle()
    {
        if(stage % 5 == 0)
        {
            enemyCntD++;
            GameObject instantEnemy = Instantiate(enemyPrefabs[3], enemyZones[0].position, enemyZones[0].rotation);
            boss = instantEnemy.GetComponent<Boss>();
            boss.target = player.transform;
            boss.gameManager = this;
        }
        else
        {
            for (int index = 0; index < stage; index++)
            {
                int random = Random.Range(0, 3);
                enemyList.Add(random);

                switch (random)
                {
                    case 0:
                        enemyCntA++;
                        break;
                    case 1:
                        enemyCntB++;
                        break;
                    case 2:
                        enemyCntC++;
                        break;
                }
            }

            while (enemyList.Count > 0)
            {
                int randomZone = Random.Range(0, 4);
                GameObject instantEnemy = Instantiate(enemyPrefabs[enemyList[0]], enemyZones[randomZone].position, enemyZones[randomZone].rotation);
                Enemy enemy = instantEnemy.GetComponent<Enemy>();
                enemy.target = player.transform;
                enemy.gameManager = this;
                enemyList.RemoveAt(0);

                yield return new WaitForSeconds(5.0f);
            }
        }
        
        while(EnemisCnt > 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(4.0f);

        boss = null;
        StageEnd();
    }

    public void GameOver()
    {
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
        curScoreText.text = scoreText.text;

        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if(player.score > maxScore)
        {
            bestText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void Update()
    {
        if(isBattle)
        {
            playTime += Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        scoreText.text = string.Format("{0:n0}", player.score);
        stageText.text = "STAGE " + stage;
        
        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);
        playTimeText.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second);

        playerHealthText.text = player.health + " / " + player.maxHealth;
        playerCoinText.text = string.Format("{0:n0}", player.coin);

        if (!player.IsEquip)
            playerAmmoText.text = "- / " + player.ammo;
        else if (player.equipWeapon.type == Weapon.Type.Melee)
            playerAmmoText.text = "- / " + player.ammo;
        else
            playerAmmoText.text = player.equipWeapon.curAmmo + " / " + player.equipWeapon.maxAmmo;

        weaponImage1.color = new Color(1, 1, 1, player.hasWeapon[0] ? 1 : 0);
        weaponImage2.color = new Color(1, 1, 1, player.hasWeapon[1] ? 1 : 0);
        weaponImage3.color = new Color(1, 1, 1, player.hasWeapon[2] ? 1 : 0);
        weaponImageR.color = new Color(1, 1, 1, player.hasGrenade > 0 ? 1 : 0);  // grenade

        enemyAText.text = "x" + enemyCntA.ToString();
        enemyBText.text = "x" + enemyCntB.ToString();
        enemyCText.text = "x" + enemyCntC.ToString();

        if(boss != null)
        {
            float bossHealthRate = boss.curHealth / boss.maxHealth;
            bossHealthGroup.anchoredPosition = Vector3.down * 30;
            bossHealthBar.localScale = new Vector3(bossHealthRate, 1, 1);
        }
        else
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 200;
        }
    }
}
