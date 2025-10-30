using UnityEngine;
using System.Collections;

public class NinjaController : MonoBehaviour
{
    [Header("Sprite Sets")]
    public Sprite[] blueSprites;   // 3 frames
    public Sprite[] redSprites;    // 3 frames
    public Sprite[] greenSprites;  // 3 frames

    private SpriteRenderer spriteRenderer;
    private int currentFrame = 0;
    private bool isSwinging = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetSpriteSet("Blue"); // default at start
    }

    // Called when player finishes typing a word
    public void Swing(int level)
    {
        if (!isSwinging)
            StartCoroutine(SwingAnimation(level));
    }

    IEnumerator SwingAnimation(int level)
    {
        isSwinging = true;
        Sprite[] activeSet = GetSpriteSet(level);

        for (int i = 0; i < activeSet.Length; i++)
        {
            spriteRenderer.sprite = activeSet[i];
            yield return new WaitForSeconds(0.08f); // quick frame change
        }

        // Reset to first frame
        spriteRenderer.sprite = activeSet[0];
        isSwinging = false;
    }

    public void SetSpriteSet(string color)
    {
        Sprite[] activeSet = color switch
        {
            "Blue" => blueSprites,
            "Red" => redSprites,
            "Green" => greenSprites,
            _ => blueSprites
        };
        spriteRenderer.sprite = activeSet[0];
    }

    private Sprite[] GetSpriteSet(int level)
    {
        if (level <= 2) return blueSprites;   // Levels 1–3
        if (level <= 5) return redSprites;    // Levels 4–6
        return greenSprites;                  // Levels 7–9
    }
}
