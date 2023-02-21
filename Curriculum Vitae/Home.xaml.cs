using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Curriculum_Vitae
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        SignInUp s = new SignInUp();
        public string filename = @"signInUp.json";
        public Home()
        {
            InitializeComponent();
            cvInput.Visibility = Visibility.Visible;

            if (!File.Exists(filename))
            {
                File.CreateText(filename).Close();
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            cvInput2.Visibility = Visibility.Hidden;
            cvInput.Visibility = Visibility.Visible;
        }

        // ---------->>> 1ST STEP - SIGN UP
        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            cvInput2.Visibility = Visibility.Visible;
            cvInput.Visibility = Visibility.Hidden;
        }

        // ---------->>> 2ND STEP - SIGN UP
        private void btnSignUp2_Click(object sender, RoutedEventArgs e)
        {
            string filedata = File.ReadAllText(filename);
            try
            {
                s.UserName = txtUserName2.Text.ToString();
                s.Password = txtPassword2.Text.ToString();

                var newUser = "{'UserName':'" + s.UserName + "','Password':'" + s.Password + "'}";

                if (!IsIdExists(s.UserName))
                {
                    var json = File.ReadAllText(filename);
                    var jsonObj = JObject.Parse(json);
                    var newUserArray = jsonObj.GetValue("SignInUp") as JArray;

                    var newUserSign = JObject.Parse(newUser);
                    newUserArray.Add(newUserSign);

                    jsonObj["SignInUp"] = newUserArray;
                    var newJsonResult = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);

                    File.WriteAllText(filename, newJsonResult);

                    clear();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error\nData Type MisMatched", "Message", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // ---------->>> 2ND STEP - CLEAR
        private void clear()
        {
            txtUserName2.Clear();
            txtPassword2.Clear();
            txtUserName2.Focus();
        }


        // ---------->>> 2ND STEP - SIGN UP
        private bool IsIdExists(string inputId)    //input id from input box
        {
            string filedata = File.ReadAllText(filename);
            var data = JObject.Parse((string)filedata);
            var empJson = data.GetValue("SignInUp").ToString();
            var empList = JsonConvert.DeserializeObject<List<SignInUp>>(empJson);

            var exists = empList.Find(x => x.UserName == inputId);

            if (exists != null)
            {
                MessageBox.Show($"UserName - {exists.UserName} exists\nTry Another UserName", "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                return true;
            }
            else
            {
                MessageBox.Show($"Your UserName : {s.UserName}\nYour Password : {s.Password}", "Congratulations", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
        }


        //---------->>> 2ND STEP - SIGN IN
        private bool IsIdExists2(string inputUserName, string inputPassword)    //input id from input box
        {
            string filedata = File.ReadAllText(filename);
            var data = JObject.Parse((string)filedata);
            var empJson = data.GetValue("SignInUp").ToString();
            var empList = JsonConvert.DeserializeObject<List<SignInUp>>(empJson);

            var existsUserName = empList.Find(x => x.UserName == inputUserName);
            var existsPassword = empList.Find(x => x.UserName == inputPassword);


            if (existsUserName != null && existsPassword != null)
            {
                MainWindow m = new MainWindow();
                this.Hide();
                m.Show();
                return true;
            }
            else
            {
                MessageBox.Show("Incorrect UserName or Password\nPlease Input Correct UserName & Password", "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                txtUserName.Focus();
                return false;
            }
        }


        //---------->>> 2ND STEP - SIGN IN
        private void btnSign_Click_1(object sender, RoutedEventArgs e)
        {
            string filedata = File.ReadAllText(filename);
            try
            {
                s.UserName = txtUserName.Text.ToString();
                s.Password = txtPassword.Text.ToString();

                var newUser = "{'UserName':'" + s.UserName + "','Password':'" + s.Password + "'}";

                if (!IsIdExists2(s.UserName, s.Password))
                {
                    var json = File.ReadAllText(filename);
                    var jsonObj = JObject.Parse(json);
                    var newUserArray = jsonObj.GetValue("SignInUp") as JArray;

                    var newUserSign = JObject.Parse(newUser);
                    newUserArray.Add(newUserSign);

                    jsonObj["SignInUp"] = newUserArray;
                    var newJsonResult = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);

                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error\nData Type MisMatched", "Message", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
