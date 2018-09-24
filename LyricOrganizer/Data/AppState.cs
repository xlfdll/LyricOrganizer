using System;
using System.Collections.ObjectModel;

using Xlfdll;

namespace LyricOrganizer
{
    public class AppState : ObservableObject
    {
        public AppState()
        {
            this.Status = "Ready";
            this.TypeIndex = 0;
            this.Page = 1;

            this.Results = new ObservableCollection<LyricItem>();
            this.Providers = new ObservableCollection<ILyricProvider>();

            this.InitializeProviders();

            AppState.Current = this;
        }

        #region UI States

        private String _status;
        private Boolean _isBusy;
        private LyricItem _currentItem;
        private Int32 _page;

        public String Status
        {
            get { return _status; }
            set { this.SetField<String>(ref _status, value); }
        }
        public Boolean IsBusy
        {
            get { return _isBusy; }
            set { this.SetField<Boolean>(ref _isBusy, value); }
        }
        public LyricItem CurrentItem
        {
            get { return _currentItem; }
            set { this.SetField<LyricItem>(ref _currentItem, value); }
        }
        public Int32 Page
        {
            get { return _page; }
            set { this.SetField<Int32>(ref _page, value); }
        }

        #endregion

        #region Data Input

        private String _keyword;
        private Int32 _typeIndex;

        public String Keyword
        {
            get { return _keyword; }
            set { this.SetField<String>(ref _keyword, value); }
        }

        public Int32 TypeIndex
        {
            get { return _typeIndex; }
            set { this.SetField<Int32>(ref _typeIndex, value); }
        }

        #endregion

        public String ActiveKeyword { get; set; }

        public ObservableCollection<LyricItem> Results { get; }
        public ObservableCollection<ILyricProvider> Providers { get; }

        private void InitializeProviders()
        {
            this.Providers.Add(new JLyricProvider());
        }

        public static AppState Current { get; private set; }
    }
}