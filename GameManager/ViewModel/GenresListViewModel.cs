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
    public class GenresListViewModel : WorkspaceViewModel
    {
        #region Fields

        #endregion // Fields

        #region Constructor

        public GenresListViewModel(MainWindowViewModel parent)
            : base(parent)
        {
            DisplayName = "Genres";

            RequestClose += new EventHandler(GenresListViewModel_RequestClose);
        }

        void GenresListViewModel_RequestClose(object sender, EventArgs e)
        {
            parent.databaseContext.SaveChanges();
        }

        #endregion // Constructor

        #region Presentation Properties

        public ObservableCollection<Genre> GenresList
        {
            get
            {
                return parent.GenresList;
            }
        }

        #endregion
    }
}
