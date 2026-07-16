using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [Header("エサの設定")]
    [Tooltip("スポーンさせるエサのプレハブ")]
    public GameObject foodPrefab;
    [Tooltip("マップ上に同時に存在できるエサの最大数")]
    public int maxFoodCount = 50;
    [Tooltip("エサを補充する間隔（秒）")]
    public float spawnInterval = 0.5f;

    private float timer = 0f;

    // 設定キャッシュ用変数
    private Vector3 center = Vector3.zero;
    private float radius = 10f;

    void Start()
    {
        if (PlanetManager.Instance != null)
        {
            center = PlanetManager.Instance.sphereCenter;
            radius = PlanetManager.Instance.sphereRadius;
        }

        // ゲーム開始時に、最大数の半分くらいのエサをあらかじめばらまいておく
        for (int i = 0; i < maxFoodCount / 2; i++)
        {
            SpawnFood();
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            // 現在マップ上にあるエサの数を数える
            GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
            
            // エサの数が最大数より少なければ、新しく補充する
            if (foods.Length < maxFoodCount)
            {
                SpawnFood();
            }
        }
    }

    private void SpawnFood()
    {
        if (foodPrefab == null) return;

        // Random.onUnitSphere で「半径1の球体の表面のランダムな座標」が取得できる。
        // それに実際の半径を掛けて中心座標を足すことで、星の表面のランダムな位置になる。
        Vector3 randomDirection = Random.onUnitSphere;
        Vector3 spawnPos = center + randomDirection * radius;

        // エサを生成
        GameObject food = Instantiate(foodPrefab, spawnPos, Quaternion.identity);

        // エサの「上方向」を球の外側に向ける（見た目の調整）
        food.transform.up = randomDirection;

        // ヒエラルキーが散らからないように、このスポナーの子オブジェクトにする
        food.transform.SetParent(transform);
    }
}
