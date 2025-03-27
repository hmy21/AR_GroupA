using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 画出默认 Inspector（变量）
        DrawDefaultInspector();

        // 获取当前正在查看的 Player 实例
        Player player = (Player)target;

        // 添加按钮
        if (GUILayout.Button("test getDamage()"))
        {
            player.getDamage();
        }
        if (GUILayout.Button("test resetPlayer()"))
        {
            player.resetPlayer();
        }
    }
}
