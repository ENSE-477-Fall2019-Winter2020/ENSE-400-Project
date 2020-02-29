﻿using CareOnDemand.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CareOnDemand.Views.CustomerViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ServiceSelectionList : ContentPage
    {
        public ServiceSelectionList()
        {
            InitializeComponent();

            BindingContext = new ServiceSelectionListViewModel();
        }
    }
}