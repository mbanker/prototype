using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    GameManager gm;
    bool freshSelect; // marker that this is the piece that was just selected
    MeshRenderer ren;
    Color baseColor;
    Color selectColor;
    Color deactivateColor;

    // Triggers
    public static bool triggerSelect;
    
    // States
    enum States
    {
        Active,
        Selected,
        Deactivated
    };
    States state;

    void Start()
    {
        // Initialize
        var gameManager = GameObject.FindWithTag("GameController") as GameObject;
        gm = gameManager.GetComponent<GameManager>();
        state = States.Active;
        ren = GetComponent<MeshRenderer>();
        baseColor = ren.material.color;
        selectColor = ChangeColor(baseColor, satAdd: 0.4f, valAdd: 0.4f); // more saturated and lighter
        deactivateColor = ChangeColor(baseColor, satFactor: 0.2f, valAdd: -0.2f); // very unsaturated and darker;

    }

    private void Update()
    {
        // Set color
        switch (state)
        {
            case States.Selected:
                ren.material.color = selectColor;
                break;
            case States.Deactivated:
                ren.material.color = deactivateColor;
                break;
            default: // Active case
                ren.material.color = baseColor;
                break;
        }

        // Selection triggered
        if (PlayerManager.triggerSelect)
        {
            // This Piece
            if (freshSelect)
            {
                state = States.Selected;
            }
            else // Another piece
            {
                state = States.Active;
            }
        }
    }


    // Handles triggers set during update (mostly from Game Manager)
    void LateUpdate()
    {
        // Board changed position
        if (gm.triggerChange)
        {

        }

        // Reset vars
        freshSelect = false;
        triggerSelect = false;
    }

    public bool CheckSurrounded()
    {
        return false;
    }

    void Deactivate()
    {

    }

    private void OnMouseDown()
    {
        PlayerManager.triggerSelect = true;
        freshSelect = true;
    }

    // Can multiply and/or add saturation and value (will multiply first; most likely want one or the other)
    Color ChangeColor(Color baseCol, float satFactor = 1f, float satAdd = 0f, float valFactor = 1f, float valAdd = 0f)
    {
        Color.RGBToHSV(baseCol, out float h, out float s, out float v);
        s = s * satFactor;
        s = s + satAdd;
        s = Mathf.Clamp(s, 0f, 1f);
        v = v * valFactor;
        v = v + valAdd;
        v = Mathf.Clamp(v, 0f, 1f);
        return Color.HSVToRGB(h, s, v);
    }
}
