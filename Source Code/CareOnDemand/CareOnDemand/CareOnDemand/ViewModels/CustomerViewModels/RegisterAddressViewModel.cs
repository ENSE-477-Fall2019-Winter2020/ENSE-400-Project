﻿/*
    Care on Demand Application
    Capstone 2020 - ENSE 400/477
    The Ni(c)(k)S

    Author: Shayan Khan
    Contributor(s): Nicolas Achter
    Last Modified: Apr. 06, 2020
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using CareOnDemand.Models;
using CareOnDemand.Views.SharedViews;
using CareOnDemand.Validators;
using FluentValidation;
using FluentValidation.Results;
using CareOnDemand.Data;

namespace CareOnDemand.ViewModels
{
    public class RegisterAddressViewModel : BaseCustomerDetailsViewModel
    {
        public RegisterAddressViewModel()
        {
            CreateAccountCommand = new Command(CreateAccountClicked);
            AddAddressCommand = new Command(AddAddressClicked);
        }

        public Command CreateAccountCommand { private set; get; }
        public Command AddAddressCommand { private set; get; }

        async void CreateAccountClicked()
        {
            CustomerAddressValidator customer_address_validator = new CustomerAddressValidator();
            ValidationResult results = customer_address_validator.Validate(address);

            if (!results.IsValid)
            {
                String result_messages = results.ToString("\n");
                await Application.Current.MainPage.DisplayAlert("Error", result_messages, "OK");
            }
            else
            {
                RegisterService registerModel = new RegisterService(account, address);

                try
                {
                    await registerModel.CreateCognitoUser();
                    await registerModel.CreateDatabaseUser(account);
                    await registerModel.AddAddress(address);
                    await Application.Current.MainPage.DisplayAlert("Success", "Account created succesfully. Please check your email for verification link", "OK");
                    await Application.Current.MainPage.Navigation.PushAsync(new LoginPage());
                }
                catch(Exception e)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
                }
            }

        }

        //add an aditional address to customer account
        async void AddAddressClicked()
        {
            CustomerAddressValidator customer_address_validator = new CustomerAddressValidator();
            ValidationResult results = customer_address_validator.Validate(address); //validate address

            if (!results.IsValid) //invalid - show error
            {
                String result_messages = results.ToString("\n");
                await Application.Current.MainPage.DisplayAlert("Error", result_messages, "OK");
            }
            else //valid try to save address
            {
                AddressRestService addressRestService = new AddressRestService();
                Customer_AddressRestService customer_AddressRestService = new Customer_AddressRestService();
                Address created_address = await addressRestService.SaveAddressAsync(address, true); //save address entry to db
                Customer_Address customer_Address = new Customer_Address();

                //gather customer_address data
                customer_Address.CustomerID = (int)Application.Current.Properties["customerID"];
                customer_Address.AddressID = created_address.AddressID;
                customer_Address.AddressLabel = customer_address.AddressLabel;
                try //to save customer_address entry to db
                {
                    await customer_AddressRestService.SaveCustomer_AddressAsync(customer_Address, true);
                    await Application.Current.MainPage.DisplayAlert("Success", "Address Saved", "OK");
                    await Application.Current.MainPage.Navigation.PushAsync(new AccountManagementPage());
                }
                catch (Exception e)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
                }
            }
        }
    }
}
