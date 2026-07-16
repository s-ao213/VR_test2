using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject controllerPrefab; // プレイヤーのコントローラーのPrefab
    [SerializeField] private GameObject headPrefab; // プレイヤーの頭のPrefab
    [SerializeField] private GameObject BodyPrefab; // プレイヤーの体のPrefab
    public List<GameObject> BodyList = new List<GameObject>(); // プレイヤーのリスト
    private GameObject controller;

    void OnEnable()
    {
        // エサを食べたときのイベントを購読し、AddBodyを実行する
        Head.OnFoodEaten += AddBody;
        GameManager.Started += InitializePlayer; // ゲーム開始時にプレイヤーを初期化
    }
    void OnDisable()
    {
        // イベントの購読を解除
        Head.OnFoodEaten -= AddBody;
        GameManager.Started -= InitializePlayer; // ゲーム開始時のイベント購読を解除
    }

    void InitializePlayer()
    {
        RemoveBody();

        // コントローラーと頭を生成
        if (controllerPrefab != null && headPrefab != null)
        {
            controller = Instantiate(controllerPrefab, Vector3.zero, Quaternion.identity);
            GameObject head = Instantiate(headPrefab, Vector3.zero, Quaternion.identity);
            head.transform.SetParent(transform); // プレイヤーオブジェクトの子として設定
            SphereFollower follower = head.GetComponent<SphereFollower>();
            if (follower != null)
            {
                follower.target = controller.transform; // 追従対象をプレイヤーオブジェクトに設定
            }
            BodyList.Add(head); // BodyListに頭を追加
        }
    }

    public void AddBody()
    {
        if (BodyPrefab == null) return;

        // 追従対象（ターゲット）を決める
        // まだ胴体がない場合はbody、ある場合は一番最後の胴体をターゲットにする
        Transform targetTransform = BodyList.Count > 0 ? BodyList[BodyList.Count - 1].transform : transform;

        // BodyPrefabをターゲットの現在位置にインスタンス化してBodyListに追加
        GameObject newBody = Instantiate(BodyPrefab, targetTransform.position, Quaternion.identity);
        
        newBody.transform.SetParent(transform); 

        // 新しい移動スクリプト (ChildrenFollow) を取得してターゲットを設定
        ChildrenFollow follower = newBody.GetComponent<ChildrenFollow>();
        if (follower != null)
        {
            follower.target = targetTransform; // 追従対象を設定
        }
        else
        {
            Debug.LogWarning("BodyPrefabに ChildrenFollow コンポーネントがアタッチされていません！");
        }

        BodyList.Add(newBody);
    }

    public void RemoveBody()
    {
        if (controller != null)
        {
            Destroy(controller);
            controller = null;
        }
        if (BodyList.Count > 0)
        {
            GameObject lastBody = BodyList[BodyList.Count - 1];
            BodyList.RemoveAt(BodyList.Count - 1);
            Destroy(lastBody);
        }
        BodyList.Clear(); // BodyListをクリア
    }
}
