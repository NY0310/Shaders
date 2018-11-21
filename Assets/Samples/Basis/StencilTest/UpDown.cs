using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpDown : MonoBehaviour {

    //フェードに掛ける時間
    [SerializeField]
    float moveTime = 120.0f;
    //時間
    float time = 0.0f;
    //時間を加算するか
    bool isTimeUp = false;


    // Use this for initialization
    void Start()
    {
    }

    void Update()
    {
        //learpでアルファ値を補完
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, time * 60 / moveTime * 2, gameObject.transform.position.z);
        TimeUpdate();
        IsTimeUp();
    }

    /// <summary>
    /// 時間を更新する
    /// </summary>
    void TimeUpdate()
    {
        if (isTimeUp)
        {
            time += Time.deltaTime;
        }
        else
        {
            time -= Time.deltaTime;
        }
    }

    /// <summary>
    /// タイムを加算するか決定
    /// </summary>
    void IsTimeUp()
    {
        if (!isTimeUp && time * 60 <= 0.0f)
        {
            isTimeUp = true;
        }
        if (isTimeUp && time * 60 >= moveTime)
        {
            isTimeUp = false;
        }
    }
}
