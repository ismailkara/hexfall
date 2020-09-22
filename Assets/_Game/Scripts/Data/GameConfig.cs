using UnityEngine;

[CreateAssetMenu]
public class GameConfig : ScriptableObject
{
    public int boardWidth;
    public int boardHeight;
    public Color[] colors;

    public int getRandomType()
    {
        return Random.Range(0, colors.Length);
    }

    public Color getColorOfType(int type)
    {
        return colors[type];
    }
}
