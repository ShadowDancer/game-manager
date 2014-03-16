using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using GameManager.Model;
using System.Linq;
using System.Windows.Data;


namespace GameManager
{
    public class GameViewModel : WorkspaceViewModel, IDataErrorInfo
    {
        #region Fields

        public Game game;
        bool _isSelected;

        #endregion // Fields

        #region Constructor

        public GameViewModel(Game gameObj, MainWindowViewModel parent) : base(parent)
        {
            game = gameObj;
            RequestClose += new EventHandler(GameViewModel_RequestClose);


            // location
            var query = (from d in parent.databaseContext.Locations
                         where d.ID == game.Location
                         select d);

            Model.Location loc = query.FirstOrDefault();
            Location = (loc != null) ? loc.Path : ""; 

        }


        #endregion // Constructor

        #region Game Properties

        public string Location
        {
            get;
            set;
        }
        
        public string Name
        {
            get { return game.Name; }
            set
            {
                if (value == game.Name)
                    return;

                game.Name = value;

                base.OnPropertyChanged("Name");
                base.OnPropertyChanged("DisplayName");
            }
        }

        public int Rating
        {
            get { return game.Rating; }
            set 
            { 
                game.Rating = value;
                OnPropertyChanged("Rating");
            }
        }

        public int? Status
        {
            get { return game.Status; }
            set
            {
                game.Status = value;
                OnPropertyChanged("Status");
            }
        }
        #endregion // Customer Properties

        #region Presentation Properties


        public override string DisplayName
        {
            get
            {
                return game.Name;
            }
        }

        public Genre CurrentGenre
        {
            get
            {
                var query = (from d in parent.databaseContext.Genres
                where d.ID == game.Genre
                select d);

                return query.FirstOrDefault();
            }
            set
            {


                if (value != null)
                {
                    if (value.ID == game.Genre)
                    {
                        return;
                    }

                    game.Genre = value.ID;
                }
                else
                {
                    if (game.Genre == null)
                    {
                        return;
                    }

                    game.Genre = null;
                }

                OnPropertyChanged("CurrentGenre");
            }

        }

        public SiteViewModel CurrentSite
        {
            get
            {
                return SitesList.FirstOrDefault(n => n.Site != null && n.Site.ID == game.Site);
            }
            set
            {


                if (value != null)
                {
                    if (value.Site.ID == game.Site)
                    {
                        return;
                    }

                    game.Site = value.Site.ID;
                }
                else
                {
                    if (game.Site == null)
                    {
                        return;
                    }

                    game.Site = null;
                }

                OnPropertyChanged("CurrentSite");
            }

        }

        public ObservableCollection<Genre> GenresList
        {
            get
            {
                return parent.GenresList;
            }
        }

        public ObservableCollection<SiteViewModel> SitesList
        {
            get
            {
                return parent.SitesList;
            }
        }

        public ObservableCollection<Model.Status> StatusesList
        {
            get
            {
                return parent.StatusesList;
            }
        }

        public string SiteName
        {
            get
            {
                var query = (from d in parent.databaseContext.Sites
                             where d.ID == game.Site
                             select d);

                var result = query.FirstOrDefault();

                return (result != null) ? result.Name : "";
            }

        }
        /// <summary>
        /// Gets/sets whether this customer is selected in the UI.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected)
                    return;

                _isSelected = value;

                base.OnPropertyChanged("IsSelected");
            }
        }

        #endregion // Presentation Properties

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error
        {
            get { return (game as IDataErrorInfo).Error; }
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                string error = null;

                switch (propertyName)
                {
                    case "Name":
                        error = ValidateName();
                        break;
                }

                // Dirty the commands registered with CommandManager,
                // such as our Save command, so that they are queried
                // to see if they can execute now.
                CommandManager.InvalidateRequerySuggested();

                return error;
            }
        }

        string ValidateName()
        {
            if(string.IsNullOrEmpty(Name))
            {
                return "Name is invalid";
            }

            return null;
        }



        void GameViewModel_RequestClose(object sender, EventArgs e)
        {
            if (ValidateName() != null)
            {
                Name = "Unnamed site";
            }

            // save location
            var query = (from d in parent.databaseContext.Locations
                         where d.Path == Location
                         select d);
            Model.Location loc = query.FirstOrDefault();
            if (loc == null)
            {
                loc = new Location();
                loc.Path = Location;
                parent.databaseContext.Locations.AddObject(loc);
                parent.databaseContext.SaveChanges();
            }
            game.Location = loc.ID;



            parent.databaseContext.SaveChanges();
        }

        #endregion // IDataErrorInfo Members
    }
}
