﻿using JackPot.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JackPot.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SearchReceipt : ContentPage
	{
        SearchReceiptViewModel ViewModel;
        public SearchReceipt ()
		{
			InitializeComponent ();
            BindingContext = ViewModel = new SearchReceiptViewModel(Navigation);
        }
	}
}