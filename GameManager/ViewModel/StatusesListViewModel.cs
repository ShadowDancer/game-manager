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
    public class StatusesListViewModel : WorkspaceViewModel
    {
        #region Fields

        #endregion // Fields

        #region Constructor

        public StatusesListViewModel(MainWindowViewModel parent)
            : base(parent)
        {
            DisplayName = "Statuses";
        }

        #endregion // Constructor

        #region Presentation Properties

        public ObservableCollection<Status> StatusesList
        {
            get
            {
                return parent.StatusesList;
            }
        
        }

        #endregion
    }
}
