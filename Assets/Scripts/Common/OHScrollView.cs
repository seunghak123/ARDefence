using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OHScrollView : MonoBehaviour 
{
    [SerializeField]
    private RectTransform _itemPrototype;
    [SerializeField]
    private RectTransform _scrollViewTransform;
    [SerializeField]
    private RectTransform _scrollContent;
    [SerializeField]
    private ScrollRect _scrollRect;
    [SerializeField, Range(0, 30)]
    private int instantateItemCount = 9;
    [SerializeField]
    private int lintItemCount = 0;
    [SerializeField]
    private int gapSizex = 0;
    [SerializeField]
    private int gapSizey = 0;
    [SerializeField]
    private int maxCount = 20;

    [SerializeField]
    private E_SCROLLDIRECT direction;

    public OnItemPositionChange onUpdateItem = new OnItemPositionChange();

    [System.NonSerialized]
    public LinkedList<RectTransform> itemList = new LinkedList<RectTransform>();
    
    protected float diffPreFramePosition = 0;

    protected int currentItemNo = 0;
    private int SceneSizeCount = 1;
    public IList itemInfoLists ;
    public enum E_SCROLLDIRECT
    {
        VERTICAL,
        HORIZONTAL,
    }

    // cache component

    private RectTransform rectTransform;
    protected RectTransform ScrollRectTransform
    {
        get
        {
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
            return rectTransform;
        }
    }

    private float anchoredPosition
    {
        get
        {
            return direction == E_SCROLLDIRECT.VERTICAL ? -ScrollRectTransform.anchoredPosition.y : ScrollRectTransform.anchoredPosition.x;
        }
    }
    public int lineItemCount
    {
        get
        {
            if (direction == E_SCROLLDIRECT.VERTICAL)
            {
                return (int)(_scrollViewTransform.rect.width / (gapSizex + _itemPrototype.sizeDelta.x));
            }
            else
            {
                return (int)(_scrollViewTransform.rect.height / (gapSizey + _itemPrototype.sizeDelta.y));
            }
        }
    }

    private float itemScale = -1;
    public float ItemScale
    {
        get
        {
            if (_itemPrototype != null && itemScale == -1)
            {
                itemScale = direction == E_SCROLLDIRECT.VERTICAL ? _itemPrototype.sizeDelta.y : _itemPrototype.sizeDelta.x;
            }
            return itemScale;
        }
    }

    private bool IsInit = false;
    private void UpdateItemInfo<T>(int index,GameObject target) where T : CommonScrollItemData
    {
        target.GetComponent<IInfiniteScrollSetup<T>>().OnUpdateItem(target, itemInfoLists[index] as T);
    }
    public void InitScrollView<T>(List<T> infos) where T : CommonScrollItemData
    {
        IsInit = true;
        onUpdateItem.AddListener(UpdateItemInfo<T>);
        SetInfoList(infos);
        maxCount = infos.Count;
        instantateItemCount = instantateItemCount > maxCount ? maxCount : instantateItemCount;
        List<IInfiniteScrollSetup<T>> controllers = new List<IInfiniteScrollSetup<T>>();

        var scrollRect = _scrollRect;
        scrollRect.horizontal = direction == E_SCROLLDIRECT.HORIZONTAL;
        scrollRect.vertical = direction == E_SCROLLDIRECT.VERTICAL;
        scrollRect.content = ScrollRectTransform;
        _itemPrototype.gameObject.SetActive(false);
        Vector2 pivotvecter = Vector2.zero;
        _scrollContent.anchoredPosition = new Vector3(0, 0, 0);
        GridLayoutGroup gridGroup = _scrollContent.GetComponent<GridLayoutGroup>();
        gridGroup.cellSize = new Vector2(_itemPrototype.sizeDelta.x, _itemPrototype.sizeDelta.y);
        gridGroup.spacing = new Vector2(gapSizex, gapSizey);

        switch (gridGroup.startCorner)
        {
            case GridLayoutGroup.Corner.UpperLeft:
                pivotvecter.x = 0;
                pivotvecter.y = 1;
                break;
            case GridLayoutGroup.Corner.UpperRight:
                pivotvecter.x = 1;
                pivotvecter.y = 1;
                break;
            case GridLayoutGroup.Corner.LowerLeft:
                pivotvecter.x = 0;
                pivotvecter.y = 0;
                break;
            case GridLayoutGroup.Corner.LowerRight:
                pivotvecter.x = 1;
                pivotvecter.y = 0;
                break;
        }

        _itemPrototype.pivot = pivotvecter;
        for (int i = 0; i < instantateItemCount; i++)
        {
            RectTransform item = null;
            if (itemList.Count > i)
            {
                item = itemList.ElementAt(i);
                controllers.Add(item.GetComponent<IInfiniteScrollSetup<T>>());
            }
            else
            {
                item = GameObject.Instantiate(_itemPrototype) as RectTransform;
                controllers.Add(item.GetComponent<IInfiniteScrollSetup<T>>());
            }
            item.SetParent(_scrollContent, false);
            item.name = i.ToString();
            item.anchoredPosition = direction == E_SCROLLDIRECT.VERTICAL ? new Vector2((ItemScale+ gapSizex) * (i% lineItemCount), (-ItemScale-gapSizey) * (i / lineItemCount)) :
                new Vector2((ItemScale+gapSizex) * (i/ lineItemCount), (-ItemScale - gapSizey) *( i % lineItemCount));
            itemList.AddLast(item);

            item.gameObject.SetActive(true);
            controllers[i].OnUpdateItem(item.gameObject, itemInfoLists[i] as T);
        }
        foreach (var controller in controllers)
        {
            controller.OnPostSetupItems();
        }
    }
    public void SetInfoList<T>(List<T> infos) where T : CommonScrollItemData
    {
        itemInfoLists = infos;
    }
    void Update()
    {
        if(!IsInit)
        {
            return;
        }
        if (itemList.First == null)
        {
            return;
        }
        int gapsize = direction == E_SCROLLDIRECT.VERTICAL ? gapSizey : gapSizex;
        Vector3 limitPos = Vector3.zero;
        if (direction == E_SCROLLDIRECT.VERTICAL)
        {
            if (ScrollRectTransform.anchoredPosition.y < 0)
            {
                ScrollRectTransform.anchoredPosition = new Vector2(ScrollRectTransform.anchoredPosition.x, 0);
                return;
            }
            SceneSizeCount = Mathf.RoundToInt(ScrollRectTransform.sizeDelta.y / (ItemScale + gapsize));
            limitPos = new Vector3(0, (maxCount / lineItemCount) * (ItemScale + gapsize) - (SceneSizeCount-1)*(ItemScale+gapsize), 0);
        }
        else
        {
            if (ScrollRectTransform.anchoredPosition.x < 0)
            {
                ScrollRectTransform.anchoredPosition = new Vector2(0, ScrollRectTransform.anchoredPosition.y);
                return;
            }
            SceneSizeCount = Mathf.RoundToInt(ScrollRectTransform.sizeDelta.x /(ItemScale + gapsize));
            limitPos = new Vector3(-(maxCount / lineItemCount) * (ItemScale + gapsize) + (SceneSizeCount - 1) * (ItemScale + gapsize), 0, 0);
        }
        while (anchoredPosition - diffPreFramePosition < -(ItemScale+ gapsize))
        {
            if (currentItemNo + instantateItemCount >= maxCount)
            {
                _scrollViewTransform.anchoredPosition = limitPos;
                break;
            }
            var item = itemList.First.Value;
            itemList.RemoveFirst();
            itemList.AddLast(item);
            var pos = (ItemScale+ gapsize) * (instantateItemCount/ lineItemCount);
            item.anchoredPosition = (direction == E_SCROLLDIRECT.VERTICAL) ? new Vector2((_itemPrototype.sizeDelta.x + gapSizex) *((currentItemNo + instantateItemCount) % lineItemCount), -pos+ diffPreFramePosition) : 
                new Vector2(pos+ diffPreFramePosition, (-_itemPrototype.sizeDelta.y - gapSizey) * ((currentItemNo + instantateItemCount) % lineItemCount));

            onUpdateItem.Invoke(currentItemNo + instantateItemCount, item.gameObject);

            item.gameObject.transform.name = (currentItemNo + instantateItemCount).ToString();
            currentItemNo++;

            if ((currentItemNo + instantateItemCount) % lineItemCount  == 0|| currentItemNo + instantateItemCount == maxCount)
            {
                if (direction == E_SCROLLDIRECT.VERTICAL)
                {
                    diffPreFramePosition -= ItemScale + gapsize;
                }
                else
                {
                    diffPreFramePosition += ItemScale + gapsize;
                }
            }
        }

        while (anchoredPosition - diffPreFramePosition > 0)
        {
            if (currentItemNo - 1 < 0)
            {
                _scrollContent.anchoredPosition = new Vector3(0, 0, 0);
                break;
            }
            currentItemNo--;
            if (currentItemNo% lineItemCount == 0)
            {
                diffPreFramePosition += ItemScale + gapsize;
            }

            var item = itemList.Last.Value;
            itemList.RemoveLast();
            itemList.AddFirst(item);

            var pos = (ItemScale + gapsize) * (currentItemNo/ lineItemCount);
            item.anchoredPosition = (direction == E_SCROLLDIRECT.VERTICAL) ? new Vector2((_itemPrototype.sizeDelta.x + gapSizex) *Mathf.Abs((currentItemNo % lineItemCount)), -pos) : new Vector2(pos, (-_itemPrototype.sizeDelta.y- gapSizey) * Mathf.Abs((currentItemNo % lineItemCount)));
            onUpdateItem.Invoke(currentItemNo, item.gameObject);
            item.gameObject.transform.name = currentItemNo.ToString();
        }
    }
    public interface IInfiniteScrollSetup<T> where T : CommonScrollItemData
    {
        void OnPostSetupItems();
        void OnUpdateItem(GameObject obj,T infos);
    }
    [System.Serializable]
    public class OnItemPositionChange : UnityEngine.Events.UnityEvent<int, GameObject> { }
}

