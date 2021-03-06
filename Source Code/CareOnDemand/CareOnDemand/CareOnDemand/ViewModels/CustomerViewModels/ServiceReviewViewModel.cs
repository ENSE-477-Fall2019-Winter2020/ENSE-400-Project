﻿/*
    Care on Demand Application
    Capstone 2020 - ENSE 400/477
    The Ni(c)(k)S

    Author: Shayan Khan
    Last Modified: Apr. 07, 2020
*/
using CareOnDemand.Data;
using CareOnDemand.Models;
using CareOnDemand.Views.CustomerViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CareOnDemand.ViewModels
{
    /*
     * This class defines bindings and functions relating to elements on the ServiceReviewPage. It inherits variables and objects
     * from the BaseServiceAndOrderViewModel class.
     */
    public class ServiceReviewViewModel : BaseServiceAndOrderViewModel
    {
        // Constuctor that calls functions to initialize the bindings
        public ServiceReviewViewModel()
        {
            GetFullUserAddress();
            GetDatetime();
            GetFinalPrice();
            SubmitOrderCommand = new Command(async () => await SubmitOrderClicked());
        }

        // Bindings used on this page
        public Command SubmitOrderCommand { private set; get; }
        public string Address { get; set; }
        public string Recipient
        {
            get => recipient.FullName;
            set
            {
                recipient.FullName = value;
            }
        }

        public string DateString { get; set; }
        public string TimeString { get; set; }
        public string FinalPrice { get; set; }
        public string AdditionalInstructions { get; set; }

        // Function that formats the user address into one string to display on the page
        public void GetFullUserAddress()
        {
            Address = user_address.AddrLine1.Trim() + ", " + user_address.City.Trim() + ", " + user_address.Province.Trim() + ", " + user_address.PostalCode.Trim();
        }

        // Function that formate the date and time to display on the page
        public void GetDatetime()
        {
            DateString = selected_date.Date.ToString("yyyy-MM-dd");
            TimeString = new DateTime(selected_time.Ticks).ToString("h:mm tt");
        }

        /* Function that calculates the final prices. It uses the REST service to retrieve the services in the order and their 
         * price and formats it to display on the page
        */
        async void GetFinalPrice()
        {
            ServiceRestService serviceRestService = new ServiceRestService();

            float total = 0;

            foreach (var service in user_order_service)
            {
                var retrieved_service = await serviceRestService.GetServiceByIDAsync(service.ServiceID);
                int price = retrieved_service.ServicePrice;
                total += price * service.RequestedLength;
            }

            FinalPrice = "$" + total.ToString();
            OnPropertyChanged(nameof(FinalPrice));
        }

        // Function that runs when the user submits the order. Uses the REST services to retrieve and store information in the database
        async Task SubmitOrderClicked()
        {
            CustomerRestService customerRestService = new CustomerRestService();
            OrderStatusRestService orderStatusRestService = new OrderStatusRestService();
            OrderRestService orderRestService = new OrderRestService();
            Order_ServiceRestService order_ServiceRestService = new Order_ServiceRestService();

            var customer = await customerRestService.GetCustomerByAccountIDAsync(recipient.AccountID);

            user_order.AddressID = user_address.AddressID;
            user_order.CustomerID = customer[0].CustomerID;
            user_order.OrderInstructions = AdditionalInstructions;
            user_order.OrderForID = 0;
            user_order.PaymentMethodID = 0;

            // Retrieve list of Order Statuses from the database
            var order_statuses = await orderStatusRestService.RefreshDataAsync();

            // Assign to order the OrderStatusID of New
            foreach(var order_status in order_statuses)
            {
                if (order_status.Status.Trim() == "New")
                {
                    user_order.OrderStatusID = order_status.OrderStatusID;
                    break;
                }
            }

            DateTime date = DateTime.Parse(DateString + " " + TimeString);

            user_order.RequestedTime = date;
            user_order.CreationTime = date;

            var created_order = await orderRestService.SaveOrderAsync(user_order, true);

            foreach (var service in user_order_service)
            {
                service.OrderID = created_order.OrderID;
                await order_ServiceRestService.SaveOrder_SericeAsync(service, true);
            }

            await Application.Current.MainPage.DisplayAlert("Success", "Order placed successfully!", "OK");
            user_order = null;
            Application.Current.MainPage.Navigation.InsertPageBefore(new CustomerNavBar(), Application.Current.MainPage.Navigation.NavigationStack[0]);
            await Application.Current.MainPage.Navigation.PopToRootAsync();

        }
    }
}
