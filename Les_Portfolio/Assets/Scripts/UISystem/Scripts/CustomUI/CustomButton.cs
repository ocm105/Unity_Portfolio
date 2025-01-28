using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomButton : Button
{
    //child object
    public Image childImage;
    public Text childText;
    public TextMeshProUGUI childTextMeshPro;

    #region  Child Image State

    [SerializeField]
    private ColorBlock childImage_colors = new ColorBlock()
    {
        normalColor = Color.white,
        highlightedColor = Color.white,
        pressedColor = Color.white,
        selectedColor = Color.white,
        disabledColor = Color.white
    };
    #endregion

    #region  Child Text State
    [SerializeField]
    private ColorBlock childText_colors = new ColorBlock()
    {
        normalColor = Color.white,
        highlightedColor = Color.white,
        pressedColor = Color.white,
        selectedColor = Color.white,
        disabledColor = Color.white
    };
    #endregion

    #region  Child TextMeshPro State
    [SerializeField]
    private ColorBlock childTextMeshPro_colors = new ColorBlock()
    {
        normalColor = Color.white,
        highlightedColor = Color.white,
        pressedColor = Color.white,
        selectedColor = Color.white,
        disabledColor = Color.white
    };
    #endregion

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        switch (state)
        {
            case SelectionState.Highlighted:
                {
                    if (childImage != null)
                        childImage.color = childImage_colors.highlightedColor;

                    if (childText != null)
                        childText.color = childText_colors.highlightedColor;

                    if (childTextMeshPro != null)
                        childTextMeshPro.color = childTextMeshPro_colors.highlightedColor;
                }
                break;

            case SelectionState.Pressed:
                {
                    if (childImage != null)
                        childImage.color = childImage_colors.pressedColor;

                    if (childText != null)
                        childText.color = childText_colors.pressedColor;

                    if (childTextMeshPro != null)
                        childTextMeshPro.color = childTextMeshPro_colors.pressedColor;
                }
                break;

            case SelectionState.Selected:
                {
                    if (childImage != null)
                        childImage.color = childImage_colors.selectedColor;

                    if (childText != null)
                        childText.color = childText_colors.selectedColor;

                    if (childTextMeshPro != null)
                        childTextMeshPro.color = childTextMeshPro_colors.selectedColor;
                }
                break;

            case SelectionState.Disabled:
                {
                    if (childImage != null)
                        childImage.color = childImage_colors.disabledColor;

                    if (childText != null)
                        childText.color = childText_colors.disabledColor;

                    if (childTextMeshPro != null)
                        childTextMeshPro.color = childTextMeshPro_colors.disabledColor;
                }
                break;

            default:
                {
                    if (childImage != null)
                        childImage.color = childImage_colors.normalColor;

                    if (childText != null)
                        childText.color = childText_colors.normalColor;

                    if (childTextMeshPro != null)
                        childTextMeshPro.color = childTextMeshPro_colors.normalColor;
                }
                break;
        }

        base.DoStateTransition(state, instant);
    }
}
