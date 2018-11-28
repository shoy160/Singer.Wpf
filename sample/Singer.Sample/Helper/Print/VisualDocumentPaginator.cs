using ESignature.Business;
using System;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Singer.Sample.Helper.Print
{
    /// <summary> DrawingVisual 分页打印 </summary>
    public class VisualDocumentPaginator : DocumentPaginator
    {
        private readonly ContainerVisual _containerVisual;
        private Size _pageSize;

        /// <summary> DrawingVisual 分页打印 构造函数 </summary>
        public VisualDocumentPaginator(ContainerVisual containerVisual, Size pageSize)
        {
            _containerVisual = containerVisual;
            _pageSize = pageSize;
        }

        /// <summary> 获取打印尺寸 </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static Size GetSize(PageMediaSizeName name)
        {
            try
            {
                var mediaSize = LocalPrintServer.GetDefaultPrintQueue()
                    .GetPrintCapabilities()
                    .PageMediaSizeCapability
                    .FirstOrDefault(x => x.PageMediaSizeName == name);

                if (mediaSize?.Width != null && mediaSize.Height.HasValue)
                    return new Size(mediaSize.Width.Value, mediaSize.Height.Value);
            }
            catch (Exception ex)
            {
                Const.DefaultLogger.Error(ex.Message, ex);
            }
            var defaultSize = new PageMediaSize(PageMediaSizeName.ISOA4);
            return new Size(defaultSize.Width ?? 0, defaultSize.Height ?? 1100);
        }

        public override DocumentPage GetPage(int pageNumber)
        {
            //我们使用A4纸张大小
            return new DocumentPage(_containerVisual.Children[pageNumber], _pageSize, new Rect(_pageSize),
                new Rect(_pageSize));
        }

        public override bool IsPageCountValid => true;

        public override int PageCount => _containerVisual.Children.Count;

        public override Size PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        public override IDocumentPaginatorSource Source => null;
    }
}
