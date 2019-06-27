﻿using JackPot.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WareHouseManagement.PCL.Common;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JackPot.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Order : ContentPage
	{
		public Order ()
		{
			InitializeComponent ();
            BindingContext = new OrderViewModel(Navigation);
            txt_Agent.Text = "Agent : "+GlobalConstant.CustomerName;
            txt_Balance.Text ="Balance: $"+ GlobalConstant.BalanceAmt.ToString();

        }
	}
}