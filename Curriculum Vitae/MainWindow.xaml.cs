using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;
using Curriculum_Vitae.Classes;
using Newtonsoft.Json;
using Microsoft.Win32;

namespace Curriculum_Vitae
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string filename = @"resume.json";
        public FileInfo TempImageFile { get; set; }
        public BitmapImage DefaultImage => new BitmapImage(new Uri(GetImagePath() + "default.png"));


        public FileInfo OldImageFile { get; set; }

        public MainWindow()
        {
            InitializeComponent();


            List<string> Religion = new List<string>()
            {
                "Islam",
                "Hinduism",
                "Buddhism",
                "Christianity"
            };
            this.cbReligion.ItemsSource = Religion;
            this.cbReligion.SelectedIndex = 0;

            List<string> BloodGroup = new List<string>()
            {
                "A+",
                "B+",
                "AB+",
                "O+",
                "A-",
                "B-",
                "AB-",
                "O-"
            };
            this.cbBloodGroup.ItemsSource = BloodGroup;
            this.cbBloodGroup.SelectedIndex = 3;

            var path = Path.GetDirectoryName(GetImagePath());
            if (!File.Exists(filename))
            {
                File.CreateText(filename).Close();
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            ImgDisplay.Source = DefaultImage;
            rbUnMarried.IsChecked = true;
            ShowData();

        }


        private bool IsIdExists(int inputId)    //input id from input box
        {
            string filedata = File.ReadAllText(filename);
            var data = JObject.Parse((string)filedata);              //parse file data as JObject
            var empJson = data.GetValue("Employees").ToString();
            var empList = JsonConvert.DeserializeObject<List<Resume>>(empJson);

            var exists = empList.Find(x => x.Id == inputId);                 //return employee if id found, else return null

            if (exists != null)
            {
                MessageBox.Show($"ID - {exists.Id} exists\nTry with different Id", "Message", MessageBoxButton.OK, MessageBoxImage.Warning);
                return true;
            }
            else
            {
                MessageBox.Show("Resume Create Successfully", "Congratulations", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

        }

        private bool IsValidJson(string data)   //check whether file contains json format or not
        {

            try
            {
                var temp = JObject.Parse(data);  //Try to parse json data if can't will throw exception
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool IsExists(string data)      //Check if exists parent node ('Employees') in json file
        {
            string filedata = File.ReadAllText(filename);
            var jsonObject = JObject.Parse(filedata);
            var empJson = jsonObject[data];     //If not exists return null

            return (empJson != null) ? true : false;
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            btnUpdate.Visibility = Visibility.Visible;
            btnInsert1.Visibility = Visibility.Hidden;
            btnUpload.Visibility = Visibility.Hidden;
            btnImageUpdate.Visibility = Visibility.Visible;


             Button b = sender as Button;
            Resume empbtn = b.CommandParameter as Resume;

            cvPart2.Visibility = Visibility.Hidden;
            cvPart1.Visibility = Visibility.Visible;

            txtId.Text = empbtn.Id.ToString();
            txtId.IsEnabled = false;
            txtName.Text = empbtn.Name;
            txtPhone.Text = empbtn.Phone;
            txtAddress.Text = empbtn.PresentAddress;
            txtEmail.Text = empbtn.Email;
            txtFather_sName.Text = empbtn.FathersName;
            txtMother_sName.Text = empbtn.MothersName;
            txtNationality.Text = empbtn.Nationality;
            txtHeight.Text = empbtn.Height;
            txtPermanentAddress.Text = empbtn.PermanentAddress;
            cbBloodGroup.Text = empbtn.BloodGroup;
            birthdate.Text = empbtn.Birthdate.ToString();
            cbReligion.Text = empbtn.Religion;

            if (empbtn.MaterialStatus == true)
            {
                rbMarried.IsChecked = true;
            }
            else if (empbtn.MaterialStatus == false)
            {
                rbUnMarried.IsChecked = true;
            }

            txtSSCInstituteName.Text = empbtn.SSCInstituteName;
            txtSSCGroup.Text = empbtn.SSCGroup;
            txtSSCResult.Text = empbtn.SSCResult;
            txtSSCPassingYear.Text = empbtn.HSCPassingYear;
            txtHSCInstituteName.Text = empbtn.HSCInstituteName;
            txtHSCgroup.Text = empbtn.HSCGroup;
            txtHSCResult.Text = empbtn.HSCResult;
            txtHSCpassingYear.Text = empbtn.HSCPassingYear;
            txtAddSkills.Text = empbtn.Skills;
            txtReferenceName.Text = empbtn.ReferenceName;
            txtAboutHis.Text = empbtn.ReferenceAbout;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var jsonD = File.ReadAllText(filename);
            var jsonObj = JObject.Parse(jsonD);
            var empJson = jsonObj.GetValue("Employees").ToString();
            var empList = JsonConvert.DeserializeObject<List<Resume>>(empJson);

            Button b = sender as Button;
            Resume empbtn = b.CommandParameter as Resume;
            int empId = empbtn.Id;

            MessageBoxResult result = MessageBox.Show($"Are you want to delete ID - {empId}", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes) //if press 'Yes' on delete confirmation
            {
                empList.Remove(empList.Find(x => x.Id == empId));   //Remove the employee from the list
                JArray empArray = JArray.FromObject(empList);       //Convert List<Employee> to JArray
                jsonObj["Employees"] = empArray;                    //Add JArray to 'Employees' JProperty
                var newJsonResult = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);

                FileInfo thisFile = new FileInfo(GetImagePath() + empbtn.ImageTitle);
                if (thisFile.Name != "default.png") //Delete image (Not default image)
                {
                    thisFile.Delete();
                }

                File.WriteAllText(filename, newJsonResult);

                MessageBox.Show("Data Deleted Successfully !!", "Delete", MessageBoxButton.OK, MessageBoxImage.Question);
                ShowData();
                AllClear();
            }
            else
            {
                return;
            }
        }

        public void ShowData()
        {


            var json = File.ReadAllText(filename);

            if (!IsValidJson(json))
            {
                return;
            }

            var jsonObj = JObject.Parse(json);
            var empJson = jsonObj.GetValue("Employees").ToString();
            var empList = JsonConvert.DeserializeObject<List<Resume>>(empJson);
            empList = empList.OrderBy(x => x.Id).ToList();

            foreach (var item in empList)
            {
                item.ImageSrc = ImageInstance(new Uri(GetImagePath() + item.ImageTitle));
            }
            lstEmployee.ItemsSource = empList;
            lstEmployee.Items.Refresh();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        public ImageSource ImageInstance(Uri path)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = path;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.DecodePixelWidth = 300;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
        public string GetImagePath()
        {
            var currentAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            string assemblyDirectory = Path.GetDirectoryName(currentAssembly.Location);
            string ImagePath = Path.GetFullPath(Path.Combine(assemblyDirectory, @"..\..\Img\"));

            return ImagePath;
        }

        public void AllClear()
        {
            txtId.Clear();
            txtName.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            txtEmail.Clear();
            txtFather_sName.Clear();
            txtMother_sName.Clear();
            txtNationality.Clear();
            txtHeight.Clear();
            txtPermanentAddress.Clear();
            txtSSCInstituteName.Clear();
            txtSSCGroup.Clear();
            txtSSCResult.Clear();
            txtSSCPassingYear.Clear();
            txtHSCInstituteName.Clear();
            txtHSCgroup.Clear();
            txtHSCResult.Clear();
            txtHSCpassingYear.Clear();
            txtAddSkills.Clear();
            txtReferenceName.Clear();
            txtAboutHis.Clear();
            txtId.IsEnabled = true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Resume em = new Resume();

            em.ImageTitle = (TempImageFile != null) ? $"{int.Parse(txtId.Text) + TempImageFile.Extension}" : "default.png";
            em.Id = int.Parse(txtId.Text);
            em.Name = txtName.Text;
            em.Phone = txtPhone.Text;
            em.PresentAddress = txtAddress.Text;
            em.Email = txtEmail.Text;
            em.FathersName = txtFather_sName.Text;
            em.MothersName = txtMother_sName.Text;
            em.Nationality = txtNationality.Text;
            em.Height = txtHeight.Text;
            em.BloodGroup = cbBloodGroup.SelectedItem.ToString();
            em.Birthdate = Convert.ToDateTime(birthdate.ToString());
            em.Religion = cbReligion.SelectedItem.ToString();
            em.PermanentAddress = txtPermanentAddress.Text;
            em.SSCInstituteName = txtSSCInstituteName.Text;
            em.SSCGroup = txtSSCGroup.Text;
            em.SSCResult = txtSSCResult.Text;
            em.SSCPassingYear = txtSSCPassingYear.Text;

            em.HSCInstituteName = txtHSCInstituteName.Text;
            em.HSCGroup = txtHSCgroup.Text;
            em.HSCResult = txtHSCResult.Text;
            em.HSCPassingYear = txtHSCpassingYear.Text;
            em.Skills = txtAddSkills.Text;
            em.ReferenceName = txtReferenceName.Text;
            em.ReferenceAbout = txtAboutHis.Text;


            if (rbMarried.IsChecked == true)
            {
                em.MaterialStatus = true;
            }
            else if (rbUnMarried.IsChecked == true)
            {
                em.MaterialStatus = false;
            }

            string filedata = File.ReadAllText(filename);
            if (IsValidJson(filedata) && IsExists("Employees") && !IsIdExists(em.Id))
            {
                var data = JObject.Parse(filedata);
                var empJson = data.GetValue("Employees").ToString();
                var empList = JsonConvert.DeserializeObject<List<Resume>>(empJson);
                empList.Add(em);
                JArray empArray = JArray.FromObject(empList);
                data["Employees"] = empArray;
                var newJsonResult = JsonConvert.SerializeObject(data, Formatting.Indented);

                if (TempImageFile != null)
                {
                    TempImageFile.CopyTo(GetImagePath() + em.ImageTitle);
                    TempImageFile = null;
                    ImgDisplay.Source = DefaultImage;
                }
                File.WriteAllText(filename, newJsonResult);
            }

            if (!IsValidJson(filedata))
            {
                var emp = new { Employees = new Resume[] { em } };
                string newJsonResult = JsonConvert.SerializeObject(emp, Formatting.Indented);
                if (TempImageFile != null)
                {
                    TempImageFile.CopyTo(GetImagePath() + em.ImageTitle);
                    TempImageFile = null;
                    ImgDisplay.Source = DefaultImage;
                }
                File.WriteAllText(filename, newJsonResult);
            }
            ShowData();
            AllClear();
            lstEmployee.Visibility = Visibility.Visible;
            
        }

        private void btnUpload_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Image Files(*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png;";
            fd.Title = "Select an Image";
            if (fd.ShowDialog().Value == true)
            {
                ImgDisplay.Source = new BitmapImage(new Uri(fd.FileName));
                TempImageFile = new FileInfo(fd.FileName);
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (txtId.Text == "" | txtName.Text == "" | txtPhone.Text == "" | txtAddress.Text == "" | txtEmail.Text == "" | txtFather_sName.Text == "" | txtMother_sName.Text == "" | txtNationality.Text == "" | txtHeight.Text == "" | txtPermanentAddress.Text == "")
            {
                if (txtId.Text == "")
                {
                    MessageBoxResult show;
                    show = MessageBox.Show("Missing Serial Number", "Message", MessageBoxButton.OK);
                    if (show == MessageBoxResult.OK)
                    {
                        txtId.Focus();
                    }
                }
                else if (txtName.Text == "")
                {
                    MessageBoxResult show;
                    show = MessageBox.Show("Missing Title", "Message", MessageBoxButton.OK);
                    if (show == MessageBoxResult.OK)
                    {
                        txtName.Focus();
                    }
                }
                else if (txtPhone.Text == "")
                {
                    MessageBoxResult show;
                    show = MessageBox.Show("Missing Phone Number", "Message", MessageBoxButton.OK);
                    if (show == MessageBoxResult.OK)
                    {
                        txtPhone.Focus();
                    }
                }
                else if (txtAddress.Text == "")
                {
                    MessageBoxResult show;
                    show = MessageBox.Show("Missing Address", "Message", MessageBoxButton.OK);
                    if (show == MessageBoxResult.OK)
                    {
                        txtAddress.Focus();
                    }
                }
                else if (txtEmail.Text == "")
                {
                    MessageBoxResult show;
                    show = MessageBox.Show("Missing Email", "Message", MessageBoxButton.OK);
                    if (show == MessageBoxResult.OK)
                    {
                        txtEmail.Focus();
                    }
                }
                else if (txtFather_sName.Text == "")
                {
                    MessageBoxResult show;
                    show = MessageBox.Show("Missing Father's Name", "Message", MessageBoxButton.OK);
                    if (show == MessageBoxResult.OK)
                    {
                        txtFather_sName.Focus();
                    }
                }
                else if (txtMother_sName.Text == "")
                {
                    MessageBoxResult show;
                    show = MessageBox.Show("Missing Mother's Name", "Message", MessageBoxButton.OK);
                    if (show == MessageBoxResult.OK)
                    {
                        txtMother_sName.Focus();
                    }
                }
                else if (txtNationality.Text == "")
                {
                    MessageBoxResult show;
                    show = MessageBox.Show("Missing Natinality", "Message", MessageBoxButton.OK);
                    if (show == MessageBoxResult.OK)
                    {
                        txtNationality.Focus();
                    }
                }
                else if (txtHeight.Text == "")
                {
                    MessageBoxResult show;
                    show = MessageBox.Show("Missing Height", "Message", MessageBoxButton.OK);
                    if (show == MessageBoxResult.OK)
                    {
                        txtHeight.Focus();
                    }
                }
                else if (txtPermanentAddress.Text == "")
                {
                    MessageBoxResult show;
                    show = MessageBox.Show("Missing Permanent Address", "Message", MessageBoxButton.OK);
                    if (show == MessageBoxResult.OK)
                    {
                        txtPermanentAddress.Focus();
                    }
                }
            }
            else
            {
                cvPart1.Visibility = Visibility.Hidden;
                cvPart2.Visibility = Visibility.Visible;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            cvPart2.Visibility = Visibility.Hidden;
            cvPart1.Visibility = Visibility.Visible;
            lstEmployee.Visibility = Visibility.Visible;
        }

        private void View_Click(object sender, RoutedEventArgs e)
        {
            View v = new View();

            Button b = sender as Button;
            Resume empbtn = b.CommandParameter as Resume;

            v.tbName_Copy.Text = empbtn.Name;
            v.tbPhone_Copy.Text = empbtn.Phone;
            v.tbAddress_Copy.Text = empbtn.PresentAddress;
            v.tbEmail_Copy.Text = empbtn.Email;
            v.tbName_Copy1.Text = empbtn.Name;
            v.tbFathersName_Copy.Text = empbtn.FathersName;
            v.tbMother_sName_Copy.Text = empbtn.MothersName;
            v.tbHeight_Copy.Text = empbtn.Height;
            v.tbBloodGroup_Copy.Text = empbtn.BloodGroup;
            v.tbReligion_Copy.Text = empbtn.Religion;
            v.tbNationality_Copy.Text = empbtn.Nationality;
            v.tbPermanentAddress_Copy.Text = empbtn.PermanentAddress;
            v.tbInstitute_Copy.Text = empbtn.SSCInstituteName;
            v.tbGroup_Copy.Text = empbtn.SSCGroup;
            v.tbResult_Copy.Text = empbtn.SSCResult;
            v.tbPassingYear2_Copy.Text = empbtn.SSCPassingYear;
            v.tbInstitute2_Copy.Text = empbtn.HSCInstituteName;
            v.tbGroup1_Copy1.Text = empbtn.HSCGroup;
            v.tbResult2_Copy.Text = empbtn.HSCResult;
            v.tbPassingYear2_Copy.Text = empbtn.HSCResult;
            v.tbAddSkills_Copy.Text = empbtn.Skills;
            v.tbReferenceName_Copy.Text = empbtn.ReferenceName;
            v.tbReferenceAbout_Copy.Text = empbtn.ReferenceAbout;
            v.myImage.Source = empbtn.ImageSrc;

            this.Hide();
            v.Show();

            lstEmployee.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            cvPart2.Visibility = Visibility.Hidden;
            cvPart1.Visibility = Visibility.Visible;
            lstEmployee.Visibility = Visibility.Visible;
            AllClear();
        }


        // --------------------------------------- Update -----------------//

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var Idu = Convert.ToInt32(txtId.Text);
            var Nameu = txtName.Text;
            var Phoneu = txtPhone.Text;
            var Addressu = txtAddress.Text;
            var Emailu = txtEmail.Text;
            var FathersNameu = txtFather_sName.Text;
            var MothersNameu = txtMother_sName.Text;
            var Nationalityu = txtNationality.Text;
            var Heightu = txtHeight.Text;
            var BloodGroupu = cbBloodGroup.SelectedItem.ToString();
            var Birthdateu = Convert.ToDateTime(birthdate.ToString());
            var Religionu = cbReligion.SelectedItem.ToString();
            var PermanentAddressu = txtPermanentAddress.Text;
            var MaterialStatusu = true;

            if (rbMarried.IsChecked == true)
            {
                MaterialStatusu = true;
            }
            else if (rbUnMarried.IsChecked == true)
            {
                MaterialStatusu = false;
            }

            var sscInstituteu = txtSSCInstituteName.Text;
            var ssGroupu = txtSSCGroup.Text;
            var sscResultu = txtSSCResult.Text;
            var sscPassingYearu = txtSSCPassingYear.Text;

            var hscInstituteu = txtHSCInstituteName.Text;
            var hscGroupu = txtHSCgroup.Text;
            var hscResultu = txtHSCResult.Text;
            var hscPassingYearu = txtHSCpassingYear.Text;

            var Skissu = txtAddSkills.Text;
            var refNameu = txtReferenceName.Text;
            var refDetailsu = txtAboutHis.Text;


            var json = File.ReadAllText(filename);
            var jsonObj = JObject.Parse(json);
            var empJson = jsonObj.GetValue("Employees").ToString();
            var empList = JsonConvert.DeserializeObject<List<Resume>>(empJson);

            foreach (var item in empList.Where(x => x.Id == Idu))
            {
                item.Name = Nameu;
                item.Phone = Phoneu;
                item.PresentAddress = Addressu;
                item.Email = Emailu;
                item.FathersName = FathersNameu;
                item.MothersName = MothersNameu;
                item.Nationality = Nationalityu;
                item.Height = Heightu;
                item.PermanentAddress = PermanentAddressu;
                item.BloodGroup = BloodGroupu;
                item.Birthdate = Birthdateu;
                item.Religion = Religionu;
                item.MaterialStatus = MaterialStatusu;
                item.SSCInstituteName = sscInstituteu;
                item.SSCGroup = ssGroupu;
                item.SSCResult = sscResultu;
                item.SSCPassingYear = sscPassingYearu;
                item.HSCInstituteName = hscInstituteu;
                item.HSCGroup = hscGroupu;
                item.HSCResult = hscResultu;
                item.HSCPassingYear = hscPassingYearu;
                item.Skills = Skissu;
                item.ReferenceName = refNameu;
                item.ReferenceAbout = refDetailsu;

                OldImageFile = (item.ImageTitle != "default.png") ? new FileInfo(GetImagePath() + item.ImageTitle) : null;
                if (TempImageFile != null && OldImageFile == null)
                {
                    TempImageFile.CopyTo(GetImagePath() + item.Id + TempImageFile.Extension);
                    item.ImageTitle = item.Id + TempImageFile.Extension;
                    TempImageFile = null;
                }
                if (OldImageFile != null && TempImageFile != null)
                {
                    item.ImageTitle = item.Id + TempImageFile.Extension;
                    OldImageFile.Delete();
                    TempImageFile.CopyTo(GetImagePath() + Idu + TempImageFile.Extension);
                    TempImageFile = null;
                }

                var empArray = JArray.FromObject(empList);
                jsonObj["Employees"] = empArray;
                string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                File.WriteAllText(filename, output);


                MessageBox.Show("Resume Update Successfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                btnUpdate.Visibility = Visibility.Hidden;
                btnInsert1.Visibility = Visibility.Visible;
                btnUpload.Visibility = Visibility.Visible;
                btnImageUpdate.Visibility = Visibility.Hidden;

                ShowData();
            }
        }

        private void btnImageUpdate_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Image Files(*.jpg; *.jpeg; *.png;)|*.jpg; *.jpeg; *.png;";
            fd.Title = "Select an Image";
            if (fd.ShowDialog().Value == true)
            {
                ImgDisplay.Source = ImageInstance(new Uri(fd.FileName));
                TempImageFile = new FileInfo(fd.FileName);
            }
        }

        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            cvPart1.Visibility = Visibility.Hidden;
            cvPart2.Visibility = Visibility.Visible;
            lstEmployee.Visibility = Visibility.Visible;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result;
            result = MessageBox.Show("Exit Resume Creator", "Are You Sure???", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.No)
            {
                MainWindow m = new MainWindow();
                m.Show();
            }
        }
    }
}
