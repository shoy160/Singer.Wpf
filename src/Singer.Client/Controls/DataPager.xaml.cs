using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Singer.Core;

namespace Singer.Client.Controls
{
    public partial class DataPager : INotifyPropertyChanged
    {
        public DataPager()
        {
            InitializeComponent();
            PageIndex = 1;

            CmbPageSize.SelectionChanged += (sender, e) =>
            {
                SetValue(PageSizeProperty, CmbPageSize.SelectedValue);
                RaisePageChanged();
            };
            BtnFirst.Click += (s, e) =>
            {
                PageIndex = 1;
                RaisePageChanged();
            };
            BtnPrev.Click += (s, e) =>
            {
                --PageIndex;
                RaisePageChanged();
            };
            BtnNext.Click += (s, e) =>
            {
                ++PageIndex;
                RaisePageChanged();
            };
            BtnLast.Click += (s, e) =>
            {
                PageIndex = PageCount;
                RaisePageChanged();
            };
            BtnJump.Click += (s, e) =>
            {
                var index = TxtPage.Text.CastTo(1);
                if (index < 1 || index > PageCount)
                {
                    KMessageBox.Alert("页码无效");
                    TxtPage.Text = PageIndex.ToString();
                    return;
                }
                PageIndex = index;
                RaisePageChanged();
            };
            TxtPage.GotFocus += (s, e) =>
            {
                (e.Source as TextBox)?.SelectAll();
            };

            Loaded += (sender, e) =>
            {
                CmbPageSize.SelectedIndex = 0;
            };
        }

        #region 依赖属性 & 事件
        public static readonly DependencyProperty PageIndexProperty = DependencyProperty.Register(
            "PageIndex", typeof(int), typeof(DataPager), new UIPropertyMetadata(1));

        public int PageIndex
        {
            get { return (int)GetValue(PageIndexProperty); }
            set { SetValue(PageIndexProperty, value); }
        }

        public static readonly DependencyProperty PageSizeProperty = DependencyProperty.Register(
            "PageSize", typeof(int), typeof(DataPager), new UIPropertyMetadata(10));

        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }

        public static readonly DependencyProperty TotalProperty = DependencyProperty.Register(
            "Total", typeof(int), typeof(DataPager), new UIPropertyMetadata(0, (sender, e) =>
            {
                var dp = sender as DataPager;
                if (dp == null) return;
                dp.RaisePageChanged(false);
            }));

        public int Total
        {
            get { return (int)GetValue(TotalProperty); }
            set { SetValue(TotalProperty, value); }
        }

        public static readonly DependencyProperty PageSizesProperty = DependencyProperty.Register(
            "PageSizes", typeof(string), typeof(DataPager), new UIPropertyMetadata("5,10,20", (sender, e) =>
            {
                var dp = sender as DataPager;
                if (dp == null) return;
                dp.OnPropertyChanged(nameof(PageSizeItems));
            }));

        public string PageSizes
        {
            get { return (string)GetValue(PageSizesProperty); }
            set { SetValue(PageSizesProperty, value); }
        }

        public static readonly RoutedEvent PageChangedEvent = EventManager.RegisterRoutedEvent("PageChanged",
            RoutingStrategy.Bubble, typeof(PageChangedEventHandler), typeof(DataPager));

        /// <summary>
        /// 分页更改事件
        /// </summary>
        public event PageChangedEventHandler PageChanged
        {
            add { AddHandler(PageChangedEvent, value); }
            remove { RemoveHandler(PageChangedEvent, value); }
        }

        #endregion

        #region 通知属性
        private List<int> _pageSizeItems;

        /// <summary> 每页显示数列表 </summary>
        public List<int> PageSizeItems
        {
            get
            {
                _pageSizeItems = _pageSizeItems ?? new List<int>();
                if (!string.IsNullOrWhiteSpace(PageSizes))
                {
                    _pageSizeItems = PageSizes.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(t => t.CastTo(0))
                        .OrderBy(t => t).ToList();
                }
                return _pageSizeItems;
            }
            set
            {
                if (_pageSizeItems == value)
                    return;
                _pageSizeItems = value;
                OnPropertyChanged(nameof(PageSizeItems));
            }
        }

        private int _pageCount;

        /// <summary> 总页数 </summary>
        public int PageCount
        {
            get { return _pageCount; }
            set
            {
                _pageCount = value;
                OnPropertyChanged(nameof(PageCount));
            }
        }

        private int _skip;

        public int Skip
        {
            get { return _skip; }
            set
            {
                _skip = value;
                OnPropertyChanged(nameof(Skip));
            }
        }

        #endregion

        //public ICommand PageChangeCommand { get; }

        public delegate void PageChangedEventHandler(object sender, PageChangedEventArgs args);

        private PageChangedEventArgs _pageChangedEventArgs;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary> 引发分页更改事件 </summary>
        private void RaisePageChanged(bool raiseEvent = true)
        {
            PageIndex = PageIndex <= 0 ? 1 : PageIndex;
            PageSize = PageSize <= 0 ? 10 : PageSize;
            PageCount = (int)Math.Ceiling(Total / (double)PageSize);

            if (PageIndex > PageCount)
                PageIndex = PageCount;

            if (raiseEvent)
            {
                if (_pageChangedEventArgs == null)
                {
                    _pageChangedEventArgs = new PageChangedEventArgs(PageChangedEvent, PageSize, PageIndex);
                }
                else
                {
                    _pageChangedEventArgs.PageSize = PageSize;
                    _pageChangedEventArgs.PageIndex = PageIndex;
                }
                RaiseEvent(_pageChangedEventArgs);
            }

            BtnFirst.IsEnabled = BtnPrev.IsEnabled = PageIndex > 1;
            BtnNext.IsEnabled = BtnLast.IsEnabled = PageIndex < PageCount;
        }
    }
    /// <summary>
    /// 分页更改参数
    /// </summary>
    public class PageChangedEventArgs : RoutedEventArgs
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }

        public PageChangedEventArgs(RoutedEvent routeEvent, int pageSize, int pageIndex)
            : base(routeEvent)
        {
            PageSize = pageSize;
            PageIndex = pageIndex;
        }
    }
}
