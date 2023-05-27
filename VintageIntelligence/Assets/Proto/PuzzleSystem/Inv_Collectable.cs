
using UnityEngine;
using UnityEngine.UI;

public class Inv_Collectable : MonoBehaviour
{
    [SerializeField] private Image _IconImage;

    private Color _uncollectedColor;

    public void SetCollected(Pickupable pickupable)
    {
        _uncollectedColor = _IconImage.color;
        _IconImage.sprite = pickupable.Icon;

        _IconImage.color = Color.white;
    }

    public void SetDiscarded()
    {
        _IconImage.sprite = null;
        _IconImage.color = _uncollectedColor != null ? _uncollectedColor : Color.red;
    }
}
