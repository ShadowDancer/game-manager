using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using GameManager.Model;
using System.Linq;
using System.Windows.Data;
using System.Windows;


namespace GameManager
{
    public class GamesListViewModel : WorkspaceViewModel
    {
        #region Fields

        private String _NameFilter = "";
        private Boolean _SiteSelected = false;

        #endregion // Fields

        #region Constructor

        public GamesListViewModel(MainWindowViewModel parent)
            : base(parent)
        {
            parent.SiteSelectionChangedEvent += new MainWindowViewModel.SiteSelectionEventhandler(parent_SiteSelectionChangedEvent);
            this.RequestClose += new EventHandler(GamesListViewModel_RequestClose);
            DisplayName = "Games list";
            GamesCollection = new CollectionViewSource();
            GamesCollection.Source = GamesList;
            GamesCollection.Filter += new FilterEventHandler(GamesCollection_Filter);
        }

        void GamesListViewModel_RequestClose(object sender, EventArgs e)
        {
            parent.SiteSelectionChangedEvent -= parent_SiteSelectionChangedEvent;
        }

        void parent_SiteSelectionChangedEvent(object sender, bool anythingSelected)
        {
            SiteSelected = anythingSelected;
            GamesCollection.View.Refresh();
        }

        void GamesCollection_Filter(object sender, FilterEventArgs e)
        {
            GameViewModel game = e.Item as GameViewModel;

            if (SiteSelected)
            {
                if (game.CurrentSite != null)
                {
                    e.Accepted = game.CurrentSite.IsSelected;
                }
                else
                {
                    SiteViewModel NullSite = parent.SitesList.FirstOrDefault(n => n.Site == null);
                    e.Accepted = NullSite.IsSelected;
                        
                }
            }
            else
            {
                e.Accepted = true;
            }

            if (e.Accepted)
            {
                if (game.Name != null)
                {
                    e.Accepted = game.Name.Contains(NameFilter);
                }
            }
        }

        #endregion // Constructor

        #region Presentation Properties

        public String NameFilter
        {
            get
            {
                return _NameFilter;
            }
            set
            {
                if (value == _NameFilter)
                {
                    return;
                }

                _NameFilter = value;
                GamesCollection.View.Refresh();
                OnPropertyChanged("NameFilter");
            }
        }

        public Boolean SiteSelected
        {
            get
            {
                return _SiteSelected;
            }
            set
            {
                if (_SiteSelected == value)
                {
                    return;
                }
                _SiteSelected = value;
                GamesCollection.View.Refresh();
            }
        }

        public CollectionViewSource GamesCollection
        {
            get;
            set;
        }

        public ObservableCollection<GameViewModel> GamesList
        {
            get
            {
                return parent.GamesList;
            }
        }

        public ReadOnlyCollection<CommandViewModel> CommandList
        {
            get
            {
                return parent.GameCommands;
            }

        }
        #endregion

    }
}
