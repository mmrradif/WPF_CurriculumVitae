using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;


namespace Curriculum_Vitae.Classes
{
    class Resume
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string PresentAddress { get; set; }
        public string Email { get; set; }
        public string FathersName { get; set; }
        public string MothersName { get; set; }
        public string Nationality { get; set; }
        public string Height { get; set; }
        public string BloodGroup { get; set; }
        public DateTime Birthdate { get; set; }
        public string Religion { get; set; }
        public bool MaterialStatus { get; set; }
        public string PermanentAddress { get; set; }
        public string SSCInstituteName { get; set; }
        public string SSCGroup { get; set; }
        public string SSCResult { get; set; }
        public string SSCPassingYear { get; set; }
        public string HSCInstituteName { get; set; }
        public string HSCGroup { get; set; }
        public string HSCResult { get; set; }
        public string HSCPassingYear { get; set; }
        public string Skills { get; set; }
        public string ReferenceName { get; set; }
        public string ReferenceAbout { get; set; }
        public string ImageTitle { get; set; }
        [JsonIgnore]
        public ImageSource ImageSrc { get; set; }
    }
}
