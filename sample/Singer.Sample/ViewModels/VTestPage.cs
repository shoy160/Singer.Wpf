namespace Singer.Sample.ViewModels
{
    public class VTestPage : VBase
    {
        private int _total;

        public int Total
        {
            get => _total;
            set
            {
                _total = value;
                OnPropertyChanged(() => Total);
            }
        }

        public VTestPage()
        {
            Total = 200;
        }
    }
}
