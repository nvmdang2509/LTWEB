using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTWEB.Models;

namespace LTWEB.Controllers
{
    public class UserController : Controller
    {
        DataClasses1DataContext db1 = new DataClasses1DataContext();
        // GET: User
        [HttpGet]
        public ActionResult Login()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection, ACCOUNT acc)
        {
            var dn1 = collection["dangnhap"];
            var dk1 = collection["dangky"];
            if (!String.IsNullOrEmpty(dn1))
            {
                var dn = collection["username"];
                var mk = collection["password"];
                if (String.IsNullOrEmpty(dn))
                {
                    ViewData["Loi1"] = "Vui lòng nhập tên đăng nhập !";
                }
                else if (String.IsNullOrEmpty(mk))
                {
                    ViewData["Loi2"] = "Vui lòng nhập mật khẩu !";
                }
                else
                {
                    var a = from b in db1.ACCOUNTs
                            join c in db1.ROLEs on b.ID_ROLE equals c.ID_ROLE
                            where Equals(b.MATKHAU, mk) && Equals(b.HOTEN, dn)
                            select new { b, c };
                    foreach (var e in a)
                    {
                        Session["ID_ROLE"] = e.c.ID_ROLE;
                        Session["id"] = e.b.IDACC;
                        Session["Taikhoan"] = dn;
                        return RedirectToAction("Index", "User");
                    }
                    ViewBag.Messenger = "Tên đăng nhập hoặc mật khẩu không đúng ";
                }
                return View();
            }
            else if (!String.IsNullOrEmpty(dk1))
            {
                var HOTEN = collection["HOTEN"];
                var MATKHAU = collection["MATKHAU"];
                var SDT = collection["SDT"];
                var DIACHI = collection["DIACHI"];
                var ID_ROLE = collection["ID_ROLE"];
                if (String.IsNullOrEmpty(HOTEN))
                {
                    ViewData["Loi1"] = "Vui lòng nhập họ tên khách hàng";
                }
                else if (String.IsNullOrEmpty(MATKHAU))
                {
                    ViewData["Loi2"] = "Vui lòng nhập mật khẩu";
                }
                else if (String.IsNullOrEmpty(SDT))
                {
                    ViewData["Loi3"] = "Vui lòng nhập số điện thoại của bạn";
                }
                else if (String.IsNullOrEmpty(DIACHI))
                {
                    ViewData["Loi4"] = "Vui lòng nhập địa chỉ";
                }
                else
                {
                    ViewBag.Messenger = "Đăng kí thành công";

                    acc.HOTEN = HOTEN;
                    acc.MATKHAU = MATKHAU;
                    acc.SDT = SDT;
                    acc.DIACHI = DIACHI;
                    acc.ID_ROLE = 2;
                    db1.ACCOUNTs.InsertOnSubmit(acc);
                    db1.SubmitChanges();
                }
                return View();
            }
            return View();
        }

        public ActionResult Category()
        {
            return View();
        }

        public ActionResult Index()
        {
            getBaiviet();
            return View();
        }

        public ActionResult GetMenu()
        {
            var v = from t in db1.MENUs
                    where t.hide == true
                    orderby t.order ascending
                    select t;
            return PartialView(v.ToList());
        }
        [NonAction]
        public ActionResult getBaiviet()
        {
            IList<LoadDanhSach> baiviet = new List<LoadDanhSach>();


            var a = from b in db1.PRODUCTs
                    where b.hide == true
                    orderby b.datebegin descending
                    select b;
            foreach (var d in a)
            {
                baiviet.Add(new LoadDanhSach
                {
                    id = d.id,
                    name = d.name,
                    price = float.Parse(d.price.ToString()),
                    description = d.description,
                    idsize = Int32.Parse(d.idsize.ToString()),
                    idcolor = Int32.Parse(d.idcolor.ToString()),
                    img = d.img,
                    datebegin = DateTime.Parse(d.datebegin.ToString()),
                });
            }
            return View(baiviet);
        }
    }
}