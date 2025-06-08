using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserGUI : MonoBehaviour
{
    private IUserAction action;
    GUIStyle labelStyle = new GUIStyle();
    GUIStyle titleStyle = new GUIStyle();
    GUIStyle infoStyle = new GUIStyle();
    private bool game_start = false;
    private float deltatime = 0;
    public scoreRecorder recorder;
    public AudioSource music;
    public AudioClip shoot;
    public Camera cam;
    private float holdTime = 0;
    private int arrow_sum = 21;

    void Start()
    {
        action = SSDirector.GetInstance().CurrentScenceController as IUserAction;
        recorder = singleton<scoreRecorder>.Instance;
        
        // 初始化音频
        music = gameObject.AddComponent<AudioSource>();
        music.playOnAwake = false;
        shoot = Resources.Load<AudioClip>("music/shoot");
        music.clip = shoot;

        // 设置文字样式
        titleStyle.fontSize = 100;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.normal.textColor = new Color(1, 0.8f, 0, 1); // 金色
        
        labelStyle.fontSize = 48;
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.normal.textColor = Color.white;
        
        infoStyle.fontSize = 36;
        infoStyle.alignment = TextAnchor.MiddleCenter;
        infoStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1);

        holdTime = 0;
        cam = Camera.main;
    }

    void Update()
    {
        if (game_start)
        {
            if (action.haveArrowOnPort())
            {
                deltatime += Time.deltaTime;
                Vector3 fwd = cam.transform.forward;
                fwd.Normalize();

                if (deltatime >= 1.3)
                {
                    holdTime = deltatime;
                    action.Shoot(holdTime);
                    holdTime = 0;
                    deltatime = 0;
                }
            }
            else if(arrow_sum > 0)
            {
                deltatime += Time.deltaTime;
                if (deltatime >= 0.5)
                {
                    action.create();
                    deltatime = 0;
                    music.Play();
                    arrow_sum--;
                }
            }
        }
    }

    private void OnGUI()
    {
        action = SSDirector.GetInstance().CurrentScenceController as IUserAction;
        
        // 屏幕中心坐标
        float centerX = Screen.width / 2;
        float centerY = Screen.height / 2;

        if (game_start)
        {
            // 游戏状态信息 - 左上角
            GUI.Label(new Rect(20, 20, 400, 60), "风向: " + action.GetWind(), labelStyle);
            GUI.Label(new Rect(20, 90, 400, 60), "分数: " + recorder.score.ToString(), labelStyle);

            // 游戏结束提示 - 屏幕中央
			if (arrow_sum == 0)
			{
				if (recorder.score >= 50)
				{
					GUI.Label(new Rect(centerX - 300, centerY - 150, 600, 100), "恭喜胜利!", titleStyle);
					GUI.Label(new Rect(centerX - 300, centerY - 30, 600, 80), "最终得分: " + recorder.score.ToString(), labelStyle);
				}
				else
				{
					GUI.Label(new Rect(centerX - 300, centerY - 150, 600, 100), "游戏结束", titleStyle);
					GUI.Label(new Rect(centerX - 300, centerY - 30, 600, 80), "得分: " + recorder.score + " (需要50分获胜)", labelStyle);
				}

				// 增大"再玩一次"按钮
				if (GUI.Button(new Rect(centerX - 120, centerY + 100, 240, 100), "再玩一次"))
				{
					Time.timeScale = 1;
					action.Restart();
				}
				Time.timeScale = 0;
			}
        }
        else
        {
            // 游戏开始界面
            Time.timeScale = 1;
            
            // 标题
            GUI.Label(new Rect(centerX - 350, centerY - 250, 700, 150), "AR射箭游戏", titleStyle);
            
            // 游戏说明
            GUI.Label(new Rect(centerX - 350, centerY - 80, 700, 80), "用20支箭射击目标", infoStyle);
            GUI.Label(new Rect(centerX - 350, centerY + 10, 700, 80), "获得50分即可获胜", infoStyle);
            GUI.Label(new Rect(centerX - 350, centerY + 100, 700, 80), "风向会影响箭的飞行轨迹", infoStyle);
            
            // 增大开始按钮
            if (GUI.Button(new Rect(centerX - 150, centerY + 200, 300, 120), "开始游戏"))
            {
                game_start = true;
                action.BeginGame();
            }
        }
    }
}