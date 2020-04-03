﻿using CareOnDemand.Views.CarePartnerViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace CareOnDemand.ViewModels.CarePartnerViewModels
{
    public class PastOrdersViewModel : BaseCarePartnerOrdersViewModel
    {
        public PastOrdersViewModel()
        {
            PastOrders = new List<OrdersList>();
            ActivityIndicatorVisible = true;
            ActivityIndicatorRunning = true;
            GetPastOrders();
        }

        public List<OrdersList> PastOrders { get; set; }

        private OrdersList selectedOrder;
        public OrdersList SelectedOrder
        {
            get => selectedOrder;
            set
            {
                selectedOrder = value;

                if (selectedOrder == null)
                    return;

                care_partner_selected_order = selectedOrder.CustomerOrder;

                OrderSelected();
            }
        }
        async void OrderSelected()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new ViewPastOrders());
        }
        async void GetPastOrders()
        {
            string[] newOrderStatusArray = { "Completed", "Cancelled" };

            PastOrders = await GetOrdersFromDb(newOrderStatusArray);

            ActivityIndicatorRunning = false;
            ActivityIndicatorVisible = false;
            OnPropertyChanged(nameof(ActivityIndicatorRunning));
            OnPropertyChanged(nameof(ActivityIndicatorVisible));
            OnPropertyChanged(nameof(PastOrders));
        }
    }
}
