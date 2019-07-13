﻿using JackPot.Model;
using JackPot.Service;
using MvvmHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using WareHouseManagement.PCL.Common;
using Xamarin.Forms;

namespace JackPot.ViewModel
{
    public class CustomerWithdrawViewModel: INotifyPropertyChanged
    {
        private static long ShiftId = 0;
        ICommand btn_Close;
        ICommand btn_Withdraw;
        ICommand btn_Search;
        INavigation Navigation;
        public ObservableRangeCollection<vw_CustomerDetails> CustomerGridListObservCollection { get; set; } = new ObservableRangeCollection<vw_CustomerDetails>();
        public CustomerWithdrawViewModel(INavigation navigation)
        {
            Navigation = navigation;
            GetShiftId();
        }
        public async void GetShiftId()
        {
            var TransactionNumberVal = await new loginPageService().GetDetailByUrl(Sales.GetCurrentPanelUserShift + GlobalConstant.iPanelUserID + "&LocationId=" + GlobalConstant.LocationId);
            if (TransactionNumberVal.Status == 1)
            {
                ShiftId = JsonConvert.DeserializeObject<long>(TransactionNumberVal.Response.ToString());
            }
        }
        public ICommand btnWithdraw =>
        btn_Withdraw ?? (btn_Withdraw = new Command(async () => Withdraw()));

        public ICommand btnSearch =>
      btn_Search ?? (btn_Search = new Command(async () => SearchCustomer()));
        public void Clear()
        {
            CustomerGridListObservCollection.Clear();
            WithdrawAmt = 0;
            CustomerName = "";
            CustomerAcNo = "";
        }
        private async void SearchCustomer()
        {
            if (CustomerAcNo.Length > 2)
            {
                Clear();
                Regex reg = new Regex("[*'\",_&#^@]");
                string AccNo = reg.Replace(CustomerAcNo, string.Empty);
                var CutomerData = await new loginPageService().GetDetailByUrl(CustomerApi.GetCustomerDetailByUserNameAndPassword + AccNo+ "&Password="+WithdrawlPin);
                if (CutomerData.Status == 1)
                {
                    var CustomerDetail = JsonConvert.DeserializeObject<vw_CustomerDetails>(CutomerData.Response.ToString());
                    CustomerGridListObservCollection.Add(CustomerDetail);
                    CustomerName = CustomerDetail.Customer;
                    Depositebool = true;
                }
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Message", "Account No too Short.", "Ok");
            }
        }

        string customerAcNo;
        public string CustomerAcNo
        {
            get { return customerAcNo; }
            set
            {
                if (customerAcNo != value)
                {
                    customerAcNo = value;
                    OnPropertyChanged(nameof(CustomerAcNo));

                }
            }
        }

        string withdrawlPin;
        public string WithdrawlPin
        {
            get { return withdrawlPin; }
            set
            {
                if (withdrawlPin != value)
                {
                    withdrawlPin = value;
                    OnPropertyChanged(nameof(WithdrawlPin));

                }
            }
        }

        string customerName;
        public string CustomerName
        {
            get { return customerName; }
            set
            {
                if (customerName != value)
                {
                    customerName = value;
                    OnPropertyChanged(nameof(CustomerName));

                }
            }
        }

        double withdrawAmt;
        public double WithdrawAmt
        {
            get { return withdrawAmt; }
            set
            {
                if (withdrawAmt != value)
                {
                    withdrawAmt = value;
                    OnPropertyChanged(nameof(WithdrawAmt));

                }
            }
        }

        bool depositebool;
        public bool Depositebool
        {
            get { return depositebool; }
            set
            {
                if (depositebool != value)
                {
                    depositebool = value;
                    OnPropertyChanged(nameof(Depositebool));

                }
            }
        }



        public void Close()
        {
        }

        public async void Withdraw()
        {
            if (WithdrawAmt < 0.01)
            {
                Application.Current.MainPage.DisplayAlert("Message", "Insert Valid Amount.", "Ok");
            }
            else
            {


                var Transaction = new tblPanelUserTransaction()
                {

                    iTransactionTypeID = 8,
                    iTransactionRecordID = 0,
                    iMadeBy = GlobalConstant.iPanelUserID,
                    iLocationID = GlobalConstant.LocationId,
                    iShiftID = Convert.ToInt32(ShiftId),
                    iCustomerID = CustomerGridListObservCollection[0].CustomerID,
                    iManagerID = -9999,
                    sTransactionDetails = "Withdrawal made by customer.",
                    decAmount =Convert.ToDecimal( WithdrawAmt),
                    decNewBalance = 0,
                    dtTransactionDate = DateTime.UtcNow,
                    sMachineName = "",
                    sTransactionGUID = Guid.NewGuid()

                };
                var CustomerTransaction = new tblCustomerTransaction()
                {

                    iTransactionTypeID = 8,
                    iTransactionRecordID = 0,
                    iMadeBy = GlobalConstant.iPanelUserID,
                    sSessionID = "dghjskjdghd",
                    sIPAddress = "192.158.12.45",
                    dtCreateDateTime = DateTime.UtcNow,
                    iCustomerID = CustomerGridListObservCollection[0].CustomerID,
                    iManagerID = -9999,
                    sTransactionDetails = "Customer Account Deposit.",
                    decAmount =Convert.ToDecimal( WithdrawAmt),
                    decNewBalance = 0,
                    dtTransactionDate = DateTime.UtcNow,

                    sTransactionGUID = Guid.NewGuid()
                };
                var SaveCustomerTransaction = await new Service.CustomerService().PostCustomerTransaction(CustomerTransaction, CustomerApi.InsertCustomerTransaction);
                if (SaveCustomerTransaction.Status == 1)
                {
                    var SaveLottoTicket = await new VoidTicketService().PosttblLottoTicket(Transaction, VoidTicketApi.InsertUserTransaction);
                    if (SaveLottoTicket.Status == 1)
                    {
                        Clear();
                        Application.Current.MainPage.DisplayAlert("Message", "Success.", "Ok");
                    }

                }
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName]string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
