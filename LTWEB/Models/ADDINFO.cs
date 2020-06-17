using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Web.UI.WebControls;

namespace LTWEB.Models
{
    public class LoadDanhSach
    {
        public int id { set; get; }
        public string name { get; set; }
        public float price { get; set; }
        public string img { get; set; }
        public string description { get; set; }
        public string meta { get; set; }
        public string size { get; set; }
        public string color { get; set; }
        public bool hide { get; set; }
        public int order { get; set; }
        public DateTime datebegin { get; set; }
        public int categoryid { get; set; }
    }
}