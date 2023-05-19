using System;
using FancyScrollView;

class Context : FancyScrollRectContext
{
    public int SelectedIndex = -1;
    public Action<int> OnCellClicked;
}
