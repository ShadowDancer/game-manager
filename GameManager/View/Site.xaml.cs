﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameManager.View
{
    /// <summary>
    /// Interaction logic for Site.xaml
    /// </summary>
    public partial class Site : UserControl
    {
        public Site()
        {
            InitializeComponent();

            var d = DataContext;
        }
    }
}
