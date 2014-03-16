using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;

namespace GameManager
{
    /// <summary>
    /// The ViewModel for the application's main window.
    /// </summary>
    public class MainWindowViewModel : WorkspaceViewModel
    {
        #region Fields

        ObservableCollection<WorkspaceViewModel> _workspaces;

        public GameManager.Model.DBEntities databaseContext = new Model.DBEntities();

        #endregion // Fields

        #region Constructor

        public MainWindowViewModel() : base(null)
        {
            base.DisplayName = "Main Window";

            // load data
            CreateCommands();
            LoadGenres();
            LoadStatuses();
            LoadSites();
            LoadGames();
            ShowGamesList();
        }

        #endregion // Constructor

        #region Commands

        /// <summary>
        /// Returns a read-only list of commands 
        /// that the UI can display and execute.
        /// </summary>
        public ReadOnlyCollection<CommandViewModel> MenuCommands
        {
            get;
            private set;
        }

        public ReadOnlyCollection<CommandViewModel> GameCommands
        {
            get;
            private set;
        }

        public ReadOnlyCollection<CommandViewModel> SiteCommands
        {
            get;
            private set;
        }

        public CommandViewModel SiteSelectionChangedCommand;
        public CommandViewModel SiteSelectionChangedClearCommand;



        void CreateCommands()
        {
            SiteSelectionChangedCommand = new CommandViewModel(
                    "",
                    new RelayCommand(param => this.SiteSelectionChanged()));
            SiteSelectionChangedClearCommand = new CommandViewModel(
                    "",
                    new RelayCommand(param => this.SiteSelectionCleared()));


            MenuCommands = new ReadOnlyCollection<CommandViewModel>(new List<CommandViewModel>
            {
                new CommandViewModel(
                    "Games",
                    new RelayCommand(param => this.ShowGamesList())),

                new CommandViewModel(
                    "Genres",
                    new RelayCommand(param => this.ShowGenresList())),

                new CommandViewModel(
                    "Statuses",
                    new RelayCommand(param => this.ShowStatusesList())),
            });

            GameCommands = new ReadOnlyCollection<CommandViewModel>(new List<CommandViewModel>
            {
                new CommandViewModel(
                    "Add Game",
                    new RelayCommand(param => this.CreateNewGame())),

                new CommandViewModel(
                    "Edit Game",
                    new RelayCommand(param => this.EditGame())),

                new CommandViewModel(
                    "Delete Game",
                    new RelayCommand(param => this.DeleteGame())),
            });

            SiteCommands = new ReadOnlyCollection<CommandViewModel>(new List<CommandViewModel>
            {
                
                new CommandViewModel(
                    "Add Site",
                    new RelayCommand(param => this.CreateNewSite())),

                new CommandViewModel(
                    "Edit Site",
                    new RelayCommand(param => this.EditSite())),

                new CommandViewModel(
                    "Delete Site",
                    new RelayCommand(param => this.DeleteSite())),
            });

        }

        #endregion // Commands

        #region Workspaces

        /// <summary>
        /// Returns the collection of available workspaces to display.
        /// A 'workspace' is a ViewModel that can request to be closed.
        /// </summary>
        public ObservableCollection<WorkspaceViewModel> Workspaces
        {
            get
            {
                if (_workspaces == null)
                {
                    _workspaces = new ObservableCollection<WorkspaceViewModel>();
                    _workspaces.CollectionChanged += this.OnWorkspacesChanged;
                }
                return _workspaces;
            }
        }

        void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.NewItems)
                    workspace.RequestClose += this.OnWorkspaceRequestClose;

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.OldItems)
                    workspace.RequestClose -= this.OnWorkspaceRequestClose;
        }

        void OnWorkspaceRequestClose(object sender, EventArgs e)
        {
            WorkspaceViewModel workspace = sender as WorkspaceViewModel;
            workspace.Dispose();
            this.Workspaces.Remove(workspace);
        }

        #endregion // Workspaces

        #region Sites

        public ObservableCollection<SiteViewModel> SitesList { get; private set; }

        void LoadSites()
        {
            var query =
                        (from d in databaseContext.Sites
                         select d);
            List<Model.Site> queryResult = query.ToList();
            SitesList = new ObservableCollection<SiteViewModel>();
            SitesList.Add(new SiteViewModel(null, this));

            foreach (Model.Site cvm in queryResult)
            {
                SiteViewModel NewModel = new SiteViewModel(cvm, this);
                SitesList.Add(NewModel);
            }
        }

        void CreateNewSite()
        {
            Model.Site site = new Model.Site();
            site.Name = "Unnamed Site";
            databaseContext.Sites.AddObject(site);
            databaseContext.SaveChanges();


            SiteViewModel workspace = new SiteViewModel(site, this);
            SitesList.Add(workspace);
            Workspaces.Add(workspace);
            SetActiveWorkspace(workspace);
        }

        void EditSite()
        {
            foreach (SiteViewModel workspace in SitesList)
            {
                if (workspace.Site == null)
                {
                    continue;
                }

                if (workspace.IsSelected && !Workspaces.Contains(workspace))
                {
                    Workspaces.Add(workspace);
                    SetActiveWorkspace(workspace);
                }
            }
        }

        void DeleteSite()
        {
            foreach (SiteViewModel workspace in SitesList.ToList())
            {
                if (workspace.IsSelected)
                {
                    if (workspace.Site == null)
                    {
                        continue;
                    }

                    databaseContext.Sites.DeleteObject(workspace.Site);
                    SitesList.Remove(workspace);
                }
            }
            databaseContext.SaveChanges();
        }

        public delegate void SiteSelectionEventhandler(object sender, Boolean anythingSelected);
        public event SiteSelectionEventhandler SiteSelectionChangedEvent;

        void SiteSelectionChanged()
        {
            if (SiteSelectionChangedEvent != null)
            {
                SiteSelectionChangedEvent(this, true);
            }
        }

        void SiteSelectionCleared()
        {
            if (SiteSelectionChangedEvent != null)
            {
                SiteSelectionChangedEvent(this, false);
            }
        }

        #endregion

        #region Games

        public ObservableCollection<GameViewModel> GamesList { get; private set; }

        void ShowGamesList()
        {
            GamesListViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is GamesListViewModel)
                as GamesListViewModel;

            if (workspace == null)
            {
                workspace = new GamesListViewModel(this);
                this.Workspaces.Add(workspace);
            }
            workspace.parent = this;
            this.SetActiveWorkspace(workspace);
        }

        void LoadGames()
        {
            var query =
                        (from d in databaseContext.Games
                         select d);
            List<Model.Game> queryResult = query.ToList();
            GamesList = new ObservableCollection<GameViewModel>();
            foreach (Model.Game cvm in queryResult)
            {
                GameViewModel NewModel = new GameViewModel(cvm, this);
                GamesList.Add(NewModel);
            }

        }

        void CreateNewGame()
        {
            Model.Game game = new Model.Game();
            game.Name = "Unnamed Game";
            databaseContext.Games.AddObject(game);
            databaseContext.SaveChanges();


            GameViewModel workspace = new GameViewModel(game, this);
            GamesList.Add(workspace);
            Workspaces.Add(workspace);
            SetActiveWorkspace(workspace);
        }

        void EditGame()
        {
            foreach (GameViewModel workspace in GamesList)
            {
                if (workspace.IsSelected && !Workspaces.Contains(workspace))
                {
                    Workspaces.Add(workspace);
                    SetActiveWorkspace(workspace);
                }
            }
        }

        void DeleteGame()
        {
            foreach (GameViewModel workspace in GamesList.ToList())
            {
                if (workspace.IsSelected)
                {
                    databaseContext.Games.DeleteObject(workspace.game);
                    GamesList.Remove(workspace);
                }
            }
            databaseContext.SaveChanges();
        }
        #endregion

        #region Genres

        void LoadGenres()
        {
            var query =
            (from d in databaseContext.Genres
             select d);
            GenresList = new ObservableCollection<Model.Genre>(query.ToList());
        }

        void ShowGenresList()
        {
            GenresListViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is GenresListViewModel)
                as GenresListViewModel;

            if (workspace == null)
            {
                workspace = new GenresListViewModel(this);
                this.Workspaces.Add(workspace);
            }
            workspace.parent = this;
            this.SetActiveWorkspace(workspace);
        }
        public ObservableCollection<Model.Genre> GenresList { get; private set; }

        #endregion

        #region Statuses

        void LoadStatuses()
        {
            var query =
            (from d in databaseContext.Statuses
             select d);
            StatusesList = new ObservableCollection<Model.Status>(query.ToList());
        }

        void ShowStatusesList()
        {
            StatusesListViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is StatusesListViewModel)
                as StatusesListViewModel;

            if (workspace == null)
            {
                workspace = new StatusesListViewModel(this);
                this.Workspaces.Add(workspace);
            }
            workspace.parent = this;
            this.SetActiveWorkspace(workspace);
        }
        public ObservableCollection<Model.Status> StatusesList { get; private set; }

        #endregion

        #region Private Helpers




        void SetActiveWorkspace(WorkspaceViewModel workspace)
        {
            Debug.Assert(this.Workspaces.Contains(workspace));

            ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.Workspaces);
            if (collectionView != null)
                collectionView.MoveCurrentTo(workspace);
        }

        #endregion // Private Helpers
    }
}