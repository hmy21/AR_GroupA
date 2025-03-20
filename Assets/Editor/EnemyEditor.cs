using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Enemy))]  // 让 Editor 绑定到 Enemy 组件
public class EnemyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 先绘制默认 Inspector 面板
        DrawDefaultInspector();

        // 取得当前脚本对象
        Enemy enemy = (Enemy)target;

        // 在 Inspector 界面添加一个按钮
        if (GUILayout.Button("Shot"))
        {
            // 调用 Shot 方法
            enemy.Shot();
        }
        if (GUILayout.Button("test"))
        {
            // 调用 Shot 方法
            enemy.testFuntion();
        }
    }
}
