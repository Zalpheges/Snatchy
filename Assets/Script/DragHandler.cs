using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    private GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    [SerializeField] private Image buffer;

    RectTransform copy;
    GameObject begin;

    private void Awake()
    {
        raycaster = GetComponent<GraphicRaycaster>();

        eventSystem = GetComponent<EventSystem>();
    }

    public void OnBeginDrag(PointerEventData data)
    {
        if (begin = GetElement(data, "Item"))
        {
            copy = Instantiate(begin, begin.transform.position, Quaternion.identity, transform.parent).GetComponent<RectTransform>();
            copy.sizeDelta = new Vector2(100, 100);

            SetElementAlpha(begin, 0.4f);
        }
    }

    public void OnDrag(PointerEventData data)
    {
        if (copy) copy.GetComponent<RectTransform>().anchoredPosition += data.delta;
    }

    public void OnEndDrag(PointerEventData data)
    {
        Destroy(copy.gameObject);

        GameObject end = GetElement(data, "Item");
        if (!end) end = GetElement(data, "Cell");

        if (end)
        {
            SwapElements(begin, end);

            SetElementAlpha(begin, 1f);
            SetElementAlpha(end, 1f);
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (begin = GetElement(data, "Flux")) Debug.Log("Sale pute de merde");
    }

    private GameObject GetElement(PointerEventData data, string tag)
    {
        List<RaycastResult> results = new List<RaycastResult>();

        raycaster.Raycast(data, results);

        foreach (RaycastResult result in results) if (result.gameObject.tag.Equals(tag)) return result.gameObject;

        return null;
    }

    private void SwapElements(GameObject a, GameObject b)
    {
        Image[] _a = a.GetComponentsInChildren<Image>(true);
        Image[] _b = b.GetComponentsInChildren<Image>(true);

        bool active;
        for (int i = 0; i < 5; i++)
        {
            active = _a[i].gameObject.activeInHierarchy;

            buffer.sprite = _a[i].sprite;

            _a[i].sprite = _b[i].sprite;
            _b[i].sprite = buffer.sprite;

            _a[i].gameObject.SetActive(_b[i].gameObject.activeInHierarchy);
            _b[i].gameObject.SetActive(active);
        }
    }

    private void SetElementAlpha(GameObject element, float a)
    {
        Image[] children = element.GetComponentsInChildren<Image>(true);

        Color color;
        foreach (Image child in children)
        {
            color = child.color;
            color.a = a;
            child.color = color;
        }
    }
}
