using UnityEngine;

public class NeedleSpawner : MonoBehaviour
{
    [Header("Needleの設定")]
    [Tooltip("スポーンさせるNeedleのプレハブ")]
    public GameObject needlePrefab;
    [Tooltip("マップ上に同時に存在できるNeedleの最大数")]
    public int maxNeedles = 10;
    
    [Header("再生成の設定")]
    [Tooltip("距離を判定するためのプレイヤー（Head）")]
    public Transform playerHead;
    [Tooltip("プレイヤーから最低限どれくらい離して生成するか")]
    public float safeDistance = 5f;
    [Tooltip("Needleの寿命の最小値（秒）")]
    public float minLifeTime = 3f;
    [Tooltip("Needleの寿命の最大値（秒）")]
    public float maxLifeTime = 7f;
    [Tooltip("数をチェックして補充する間隔（秒）")]
    public float checkInterval = 0.5f;

    private float timer = 0f;
    
    // 設定キャッシュ用変数
    private Vector3 center = Vector3.zero;
    private float radius = 10f;

    private bool isSpawning = false; // Needleの生成を有効にするかどうかのフラグ

    void OnEnable()
    {
        GameManager.Started += EnableSpawning;
        GameManager.Finished += DisableSpawning;
    }
    void OnDisable()
    {
        GameManager.Started -= EnableSpawning;
        GameManager.Finished -= DisableSpawning;
    }

    void Start()
    {
        // PlanetManagerから設定を取得
        if (PlanetManager.Instance != null)
        {
            center = PlanetManager.Instance.sphereCenter;
            radius = PlanetManager.Instance.sphereRadius;
        }
    }

    void Update()
    {
        if(!isSpawning) return;
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            timer = 0f;
            // 現在マップ上にあるNeedleの数を数える
            GameObject[] needles = GameObject.FindGameObjectsWithTag("Needle");
            
            // 数が減っていたら（寿命で消えていたら）、新しく補充する
            if (needles.Length < maxNeedles)
            {
                SpawnNeedle();
            }
        }
    }

    private void SpawnNeedle()
    {
        if (needlePrefab == null) return;

        Vector3 spawnPos = Vector3.zero;
        Vector3 randomDirection = Vector3.zero;
        bool validPositionFound = false;

        // Playerの近くを避けるため、安全な場所が見つかるまで最大10回リトライする
        int maxAttempts = 10;
        for (int i = 0; i < maxAttempts; i++)
        {
            randomDirection = Random.onUnitSphere;
            spawnPos = center + randomDirection * radius;

            if (playerHead != null)
            {
                // 生成予定地とプレイヤーの距離を測る
                float dist = Vector3.Distance(spawnPos, playerHead.position);
                if (dist >= safeDistance)
                {
                    // 安全圏なら確定
                    validPositionFound = true;
                    break;
                }
            }
            else
            {
                // プレイヤーが設定されていなければ判定せずに確定
                validPositionFound = true;
                break;
            }
        }

        // もし10回探しても安全な場所が見つからなかった場合（極端に安全距離が大きい場合など）
        if (!validPositionFound)
        {
            Debug.LogWarning("Needleの安全な生成場所が見つからなかったため、最後の計算位置で妥協して生成します。");
        }

        // Needleを生成
        GameObject needle = Instantiate(needlePrefab, spawnPos, Quaternion.identity);

        // 上方向を球の外側に向ける（見た目の調整）
        needle.transform.up = randomDirection;

        // ヒエラルキーが散らからないように子オブジェクトにする
        needle.transform.SetParent(transform);

        // 自壊スクリプトが付いていれば、ランダムな寿命をセットする
        NeedleLifetime lifetimeScript = needle.GetComponent<NeedleLifetime>();
        if (lifetimeScript != null)
        {
            float randomLife = Random.Range(minLifeTime, maxLifeTime);
            lifetimeScript.Initialize(randomLife);
        }
        else
        {
            Debug.LogWarning("NeedlePrefabに 'NeedleLifetime' スクリプトがアタッチされていません！");
        }
    }

    private void EnableSpawning()
    {
        isSpawning = true;
        playerHead = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerHead == null)
        {
            Debug.LogWarning("Playerタグのオブジェクトが見つかりません。Needleの安全距離判定が無効になります。");
        }
        // 初回は最大数まで一気に生成する
        for (int i = 0; i < maxNeedles; i++)
        {
            SpawnNeedle();
        }
    }

    private void DisableSpawning()
    {
        isSpawning = false;
    }
}
