using UnityEngine;
using EasingCore;

namespace MoleMole
{
    public class LevelViewContext : BaseContext
    {
        public LevelViewContext() : base(UIType.LevelView)
        {

        }
    }
    public class LevelView : BaseView
    {
        [SerializeField] ScrollView scrollView = default;

        ItemData[] items;

        public override void OnEnter(BaseContext context)
        {
            GenerateCells(GameConstManager.LevelCount);
        }

        public override void OnExit(BaseContext context)
        {
            base.OnExit(context);
        }

        public override void OnPause(BaseContext context)
        {
            base.OnPause(context);
        }

        public override void OnResume(BaseContext context)
        {
            base.OnResume(context);
        }

        void GenerateCells(int dataCount)
        {
            items = new ItemData[dataCount];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new ItemData(i.ToString(), i);
            }

            scrollView.UpdateData(items);
            SelectCell();
        }

        void SelectCell()
        {
            if (scrollView.DataCount == 0)
            {
                return;
            }
            int toIndex = GameConstManager.LevelCount - 1 - (GameManager.Instance.nowLevelIndex / GameConstManager.SinglePageLevelItem);
            scrollView.ScrollTo(toIndex, 0.3f, Ease.InOutQuint, Alignment.Lower);
        }

        public void UpdateData(int index)
        {
            scrollView.UpdateData(items);
            int toIndex = GameConstManager.LevelCount - 1-(index / GameConstManager.SinglePageLevelItem);
            Debug.Log("To index:"+toIndex);
            scrollView.ScrollTo(toIndex, 0.3f, Ease.InOutQuint, Alignment.Lower);
        }

        public void OnCancelHomeBtnClick()
        {
            Singleton<ContextManager>.Instance.Push(new StartViewContext());
        }
    }
}
