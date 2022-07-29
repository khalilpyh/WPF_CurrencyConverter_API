/*
 * Name: Yuhao Peng
 * Date: 2022-07-28
 * Title: Currency Converter using API
 * Discription: A currency converter that use API 
 * which provide real time currency exchange rate 
 * for user to make real time currency conversion.
 * */

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Net.Http;
using Newtonsoft.Json;
using System.Data;
using System.Text.RegularExpressions;

namespace CurrencyConverter_API
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //create an Root object
        Root val = new Root();

        public MainWindow()
        {
            InitializeComponent();
            GetValue();
        }


        /// <summary>
        /// Call API to return back the currency data in JSON format if the HTTP request get successful response.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">URL that is provided for connecting to the API</param>
        /// <returns></returns>
        private static async Task<Root> GetData<T>(string url)
        {
            var myRoot = new Root();
            try
            {
                //create an httpclient object for sending/receiving http request/response from url
                HttpClient client = new HttpClient();
                //timespan to wait before the request times out, allowing wait for a minute
                client.Timeout = TimeSpan.FromMinutes(1);
                //given an url, returning back the the message and get the data in asynchronous way
                HttpResponseMessage response = await client.GetAsync(url);
                if(response.StatusCode == System.Net.HttpStatusCode.OK)   //check API response status code is ok
                {
                    //serialize the HTTP content into a string in asynchronous operation
                    string responseString = await response.Content.ReadAsStringAsync();
                    //deserialize the response string and store the data in JSON format
                    var responseObject = JsonConvert.DeserializeObject<Root>(responseString);

                    //display the response object information
                    //MessageBox.Show($"TimeStamp: {responseObject.timestamp}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                    //return API response
                    return responseObject;
                }
                return myRoot;
            }
            catch
            {
                return myRoot;
            }
        }

        /// <summary>
        /// A method to get currency exchange rate from API and bind the data to the ComboBoxes on UI.
        /// </summary>
        private async void GetValue()
        {
            //get the currency data from API
            val = await GetData<Root>("https://openexchangerates.org/api/latest.json?app_id=d6a7506986a84b1eb945ccd757152e8c");
            
            //load data to the combo box on UI
            BindCurrency();
        }

        /// <summary>
        /// Bind currency data to ComboBoxes on UI.
        /// </summary>
        private void BindCurrency()
        {
            //create a datatable object to hold the currency data
            DataTable datatable = new DataTable();
            //add column to datatable for display
            datatable.Columns.Add("CurrencyName");
            //add column to datatable for value
            datatable.Columns.Add("Rate");

            //add rows to datatable with currency name and value fetched from API
            datatable.Rows.Add("--SELECT--", 0);
            datatable.Rows.Add("INR", val.rates.INR);
            datatable.Rows.Add("JPY", val.rates.JPY);
            datatable.Rows.Add("USD", val.rates.USD);
            datatable.Rows.Add("NZD", val.rates.NZD);
            datatable.Rows.Add("EUR", val.rates.EUR);
            datatable.Rows.Add("CAD", val.rates.CAD);
            datatable.Rows.Add("ISK", val.rates.ISK);
            datatable.Rows.Add("PHP", val.rates.PHP);
            datatable.Rows.Add("DKK", val.rates.DKK);
            datatable.Rows.Add("CZK", val.rates.CZK);
            datatable.Rows.Add("CNY", val.rates.CNY);
            datatable.Rows.Add("HKD", val.rates.HKD);
            datatable.Rows.Add("TWD", val.rates.TWD);

            //binding datatable to comboboxes
            cboFromCurrency.ItemsSource = datatable.DefaultView;
            cboToCurrency.ItemsSource = datatable.DefaultView;

            //set display items in combo boxes
            cboFromCurrency.DisplayMemberPath = "CurrencyName";
            cboToCurrency.DisplayMemberPath = "CurrencyName";
            //set actual value behind combo box items
            cboFromCurrency.SelectedValuePath = "Rate";
            cboToCurrency.SelectedValuePath = "Rate";

            //set default combo box item selection
            cboFromCurrency.SelectedIndex = 0;
            cboToCurrency.SelectedIndex = 0;
        }

        /// <summary>
        /// A method that reset all controls on UI.
        /// </summary>
        private void ResetContent()
        {
            //clear textbox content
            txtCurrency.Text = string.Empty;
            //reset combo box item selection to default
            if (cboFromCurrency.Items.Count > 0)
                cboFromCurrency.SelectedIndex = 0;
            if (cboToCurrency.Items.Count > 0)
                cboToCurrency.SelectedIndex = 0;
            //clear label content
            lblCurrency.Content = string.Empty;
            //set focus to textbox
            txtCurrency.Focus();
        }

        /// <summary>
        /// Validate user input to the textbox which only allows interger value to be entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Text value entered by user.</param>
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            decimal result;

            //validation that ensure user entered and selected all information
            if(txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                MessageBox.Show("Please enter currency amount.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                txtCurrency.Focus();
            }
            else if (cboFromCurrency.SelectedValue == null || cboFromCurrency.SelectedIndex == 0 || cboFromCurrency.Text == "--SELECT--")
            {
                MessageBox.Show("Please select Currency From.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cboFromCurrency.Focus();
            }
            else if (cboToCurrency.SelectedValue == null || cboToCurrency.SelectedIndex == 0 || cboFromCurrency.Text == "--SELECT--")
            {
                MessageBox.Show("Please select Currency To.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cboToCurrency.Focus();
            }

            //if user select the same currency for converting then the result is exactly the value that user entered
            if (cboFromCurrency.Text == cboToCurrency.Text)
            {
                result = decimal.Parse(txtCurrency.Text);
                //display result to label
                lblCurrency.Content = $"{cboToCurrency.Text} {result.ToString("C2")}";
            }
            else
            {
                //calculate the currency base on exchange rate
                result = decimal.Parse(txtCurrency.Text) * decimal.Parse(cboToCurrency.SelectedValue.ToString()) / decimal.Parse(cboFromCurrency.SelectedValue.ToString());
                //display result to label
                lblCurrency.Content = $"{cboToCurrency.Text} {result.ToString("C2")}";
            }

        }

        /// <summary>
        /// Reset all controls on UI when clicking the Clear Button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ResetContent();
        }
    }
}
