using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using GameManager.Model;
using System.Linq;
using System.Windows.Data;


namespace GameManager
{
    public class SiteViewModel : WorkspaceViewModel, IDataErrorInfo
    {
        #region Fields

        public Site Site;
        bool _isSelected;

        #endregion // Fields

        #region Constructor

        public SiteViewModel(Site siteObj, MainWindowViewModel parent) : base(parent)
        {
            Site = siteObj;
            RequestClose += new EventHandler(SiteViewModel_RequestClose);

            if (Site != null)
            {
                // url
                var query = (from d in parent.databaseContext.Locations
                             where d.ID == Site.URL
                             select d);

                Model.Location loc = query.FirstOrDefault();
                URL = (loc != null) ? loc.Path : "";
            }
        }


        #endregion // Constructor

        #region Site Properties

        public string User
        {
            get 
            {
                if (Site != null)
                {
                    return Site.User;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                if (value == Site.User)
                    return;

                Site.User = value;

                base.OnPropertyChanged("User");
            }
        }

        public string URL
        {
            get;
            set;
        }

        public string Password
        {
            get { return Site.Password; }
            set
            {
                if (value == Site.Password)
                    return;

                Site.Password = value;

                base.OnPropertyChanged("Password");
            }
        }

        public string Name
        {
            get 
            {
                if (Site != null)
                {
                    return Site.Name;
                }
                else
                {
                    return "None";
                }
                
            }
            set
            {
                if (value == DisplayName)
                    return;

                Site.Name = value;

                base.OnPropertyChanged("Name");
                base.OnPropertyChanged("DisplayName");
            }
        }

        #endregion // Customer Properties

        #region Presentation Properties


        public override string DisplayName
        {
            get
            {
                return Name;
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
            get { return (Site as IDataErrorInfo).Error; }
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



        void SiteViewModel_RequestClose(object sender, EventArgs e)
        {
            if (ValidateName() != null)
            {
                Name = "Unnamed site";
            }

            // save url
            var query = (from d in parent.databaseContext.Locations
                         where d.Path == URL
                         select d);
            Model.Location loc = query.FirstOrDefault();
            if (loc == null)
            {
                loc = new Location();
                loc.Path = URL;
                parent.databaseContext.Locations.AddObject(loc);
            }
            Site.URL = loc.ID;


            parent.databaseContext.SaveChanges();
        }

        #endregion // IDataErrorInfo Members
    }
}
