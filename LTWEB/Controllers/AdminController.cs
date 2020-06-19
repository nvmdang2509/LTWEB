using LTWEB.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace LTWEB.Controllers
{

    public class AdminController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult LogOut()
        {
            Session.RemoveAll();
            return RedirectToAction("Login", "Admin");
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var usdn = collection["Hoten"];
            var usmk = collection["MatKhau"];

            if (String.IsNullOrEmpty(usdn))
            {
                ViewData["Loi1"] = "Nhap ten dang nhap";
            }
            else if (String.IsNullOrEmpty(usmk))
            {
                ViewData["Loi2"] = "Nhap mat khau";
            }
            else
            {
                ACCOUNT cUs = db.ACCOUNTs.SingleOrDefault(n => n.HOTEN == usdn && n.MATKHAU == usmk);
                if (cUs != null)
                {
                    Session["Taikhoan"] = usdn;
                    return RedirectToAction("Index", "Admin");
                }
                else

                    ViewBag.Messenger = "Tên đăng nhập hoặc mật khẩu không đúng ";
            }
            return View();
        }


        [HttpGet]
        public ActionResult Register()
        {

            return View();
        }
        public ActionResult Home(int id)
        {
            var getid = Url.RequestContext.RouteData.Values["id"];
            var a = from b in db.PRODUCTs
                    where Equals(b.id, getid)
                    select b;
            foreach (var c in a)
            {
                ViewBag.ID = c.id;
                db.PRODUCTs.DeleteOnSubmit(c);
                db.SubmitChanges();
                ViewData["Succ"] = "Đã xóa thành công !";
                return RedirectToAction("Getlistbaiviet");
            }
            Response.StatusCode = 404;
            return null;

        }

        [HttpPost]

        public ActionResult Register(FormCollection collection, ACCOUNT acc)
        {

            var dn1 = collection["dangnhap"];
            var dk1 = collection["dangky"];
            if (!String.IsNullOrEmpty(dn1))
            {
                var dn = collection["username"];
                var mk = collection["password"];
                if (String.IsNullOrEmpty(dn))
                {
                    ViewData["Loi1"] = "Nhap ten dang nhap";
                }
                else if (String.IsNullOrEmpty(mk))
                {
                    ViewData["Loi2"] = "Nhap mat khau";
                }
                else
                {

                    var a = from b in db.ACCOUNTs
                            join c in db.ROLEs on b.ID_ROLE equals c.ID_ROLE
                            where Equals(b.MATKHAU, mk) && Equals(b.HOTEN, dn)
                            select new { b, c };
                    foreach (var e in a)
                    {
                        Session["ID_ROLE"] = e.c.ID_ROLE;
                        Session["Taikhoan"] = dn;
                        return RedirectToAction("Trangchu", "Home");
                    }
                    //ADMIN ad = db1.ADMINs.SingleOrDefault(n => n.TaiKhoan == dn && n.MatKhau == mk);
                    ViewBag.Messenger = "Tên đăng nhập hoặc mật khẩu không đúng ";
                }
                return View();
            }
            else if (!String.IsNullOrEmpty(dk1))
            {
                //CUSTOMER cus = new CUSTOMER();
                var HOTEN = collection["HOTEN"];
                var MATKHAU = collection["MATKHAU"];
                var SDT = collection["SDT"];
                var DIACHI = collection["DIACHI"];
                var ID_ROLE = collection["ID_ROLE"];
                if (String.IsNullOrEmpty(HOTEN))
                {
                    ViewData["Loi1"] = "Nhap ho ten KH";
                }
                else if (String.IsNullOrEmpty(MATKHAU))
                {
                    ViewData["Loi2"] = "Vui long nhap mat khau";
                }
                else if (String.IsNullOrEmpty(SDT))
                {
                    ViewData["Loi3"] = "Vui long nhap so dien thoai";
                }
                else if (String.IsNullOrEmpty(DIACHI))
                {
                    ViewData["Loi4"] = "Vui long nhap dia chi";
                }
                else
                {
                    ViewBag.Messenger = "Dang ki thanh cong";

                    acc.HOTEN = HOTEN;
                    acc.MATKHAU = GetMD5(MATKHAU);
                    acc.SDT = SDT;
                    acc.DIACHI = DIACHI;
                    acc.ID_ROLE = 1;
                    db.ACCOUNTs.InsertOnSubmit(acc);
                    db.SubmitChanges();
                    //return RedirectToAction("Index");
                }
                return View();
            }
            return View();
        }
        [NonAction]
        public ActionResult get1baiviet()
        {
            IList<PRODUCT> baiviet = new List<PRODUCT>();


            var a = from b in db.PRODUCTs
                    
                    orderby b.datebegin descending
                    select b;
            foreach (var d in a)
            {
                baiviet.Add(new PRODUCT
                {
                    id = d.id,
                    name = d.name,
                    price = d.price,
                    img = d.img,
                    description = d.description,
                    meta = d.meta,
                    size = d.size,
                    color = d.color,
                    datebegin = DateTime.Parse(d.datebegin.ToString())
                });
            }
            return View(baiviet);
        }

        [NonAction]
        public ActionResult GetbaivietID(int id)
        {
            IList<PRODUCT> baiviet = new List<PRODUCT>();

            var a = from b in db.PRODUCTs
                    orderby b.datebegin descending
                    where b.id == id
                    select b;
            foreach (var d in a)
            {
                baiviet.Add(new PRODUCT
                {
                    id = d.id,
                    name = d.name,
                    price = d.price,
                    img = d.img,
                    description = d.description,
                    meta = d.meta,
                    size = d.size,
                    color = d.color,
                    datebegin = d.datebegin,
                    categoryid =d.categoryid,
                    //ACCOUNT = d.ACCOUNT,
                    order =d.order
                });
            }
            return View(baiviet);
        }
        [HttpGet, ValidateInput(false)]
        public ActionResult Getlistbaiviet()
        {
            if (Equals(Session["ID_ROLE"], 2)) return RedirectToAction("Index", "User");
            if (Session["TaiKhoan"] == null)
            {
                ViewData["Loi1"] = "Vui lòng đăng nhập tài khoản";
            }

            var a = from b in db.PRODUCTs
                    orderby b.datebegin descending
                    where b.hide == true
                    select b;
            return View(a.ToList());
        }
        [HttpGet]
        public ActionResult DeleteUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DeleteUser(int id)
        {
            var a = from b in db.PRODUCTs
                    where b.id == id
                    select b;
            foreach (var c in a)
            {
                ViewBag.ID = c.id;
                db.PRODUCTs.DeleteOnSubmit(c);
                db.SubmitChanges();
                return RedirectToAction("Index","Admin");
            }
            Response.StatusCode = 404;
            return null;
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost, ValidateInput(false)]

        public ActionResult Create(FormCollection collection, PRODUCT add, HttpPostedFileBase fileupload)
        {
            var name = collection["name"];
            var price = collection["price"];
            var description = collection["description"];
            var idsize = collection["idsize"];
            var idcolor = collection["idcolor"];
            var NGAYUP = String.Format("{0:MM/dd/yyyy}", collection["NGAYUP"]);
            if (String.IsNullOrEmpty(name))
            {
                ViewData["Loi1"] = "Vui lòng nhập tiêu đề"; return View();
            }
            else if (String.IsNullOrEmpty(price))
            {
                ViewData["Loi1"] = "Vui lòng nhập giá"; return View();
            }
            else if (String.IsNullOrEmpty(description))
            {
                ViewData["Loi1"] = "Vui lòng nhập nội dung"; return View();
            }
            //else if (String.IsNullOrEmpty(meta))
            //{
            //    ViewData["Loi1"] = "Vui lòng nhập mô tả"; return View();
            //}
            //else if (String.IsNullOrEmpty(size))
            //{
            //    ViewData["Loi1"] = "Vui lòng nhập size"; return View();
            //}
            //else if (String.IsNullOrEmpty(color))
            //{
            //    ViewData["Loi1"] = "Vui lòng nhập màu"; return View();
            //}
            //else if (String.IsNullOrEmpty(order))
            //{
            //    ViewData["Loi1"] = "Vui lòng nhập order"; return View();
            //}
            else
            {
                if (ModelState.IsValid)
                {
                    if (fileupload == null) { ViewData["Loi8"] = "Nhập hình ảnh"; return View(); }
                    var filename1 = DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetFileName(fileupload.FileName);
                    var path1 = Path.Combine(Server.MapPath("~/Content/HinhAnh"), filename1);
                    if (System.IO.File.Exists(path1))
                    {
                        ViewData["Err1"] = "(*) Hình ảnh đã tồn tại !";
                        return View();
                    }
                    else
                    {

                        //add.id = Int32.Parse(Session["id"].ToString());
                        add.img = filename1;
                        add.name = name;
                        add.price = float.Parse(price);
                        add.description = description;
                        add.idsize = Int32.Parse(idsize);
                        add.idcolor = Int32.Parse(idcolor);
                        add.hide = false;
                        add.datebegin = DateTime.Now;
                        db.PRODUCTs.InsertOnSubmit(add);
                        db.SubmitChanges();
                        fileupload.SaveAs(path1);


                    }
                }
            }
            ViewData["Err2"] = "Đăng bài thành công";
            return RedirectToAction("Getlistbaiviet");
        }

        public static string GetMD5(string str)
        {

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] bHash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

            StringBuilder sbHash = new StringBuilder();

            foreach (byte b in bHash)
            {

                sbHash.Append(String.Format("{0:x2}", b));

            }

            return sbHash.ToString();
        }
        [HttpGet]

        public ActionResult CreateMenu()
        {
            return View();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CreateMenu(FormCollection collection,MENU menu)
        {
            var name = collection["name"];
            var link = collection["link"];
            if (String.IsNullOrEmpty(name))
            {
                ViewData["Loi1"] = "Vui lòng nhập tên menu"; return View();
            }
            else
            {
                menu.name = name;
                menu.link = link;
                menu.hide = true;
            }
            return View();
        }
        [HttpGet, ValidateInput(false)]
        public ActionResult Getlistmenu()
        {
            var id = Url.RequestContext.RouteData.Values["id"];
            if (id != null)
                GetbaivietID(Int32.Parse(id.ToString()));
            else Getmenu();
            return View();
        }
        [NonAction]
        public ActionResult Getmenu()
        {
            IList<PRODUCT> baiviet = new List<PRODUCT>();


            var a = from b in db.MENUs
                    select b;
            foreach (var d in a)
            {
                baiviet.Add(new PRODUCT
                {
                    id = d.id,
                    name = d.name,
                    meta = d.meta,
                    hide = d.hide,
                    order = d.order,
                    datebegin = DateTime.Parse(d.databegin.ToString())
                });
            }
            return View(baiviet);
        }

        [HttpGet]
        public ActionResult Capnhat()
        {
            var id = Url.RequestContext.RouteData.Values["id"];
            if (id == null) return RedirectToAction("ListBaiViet");
            return LoadCapnhat(Int32.Parse(id.ToString()));
        }
        [HttpPost]
        public ActionResult Capnhat(FormCollection collection)
        {

            if (Session["TaiKhoan"] == null)
                return RedirectToAction("Index", "home");
            var getidUser = Url.RequestContext.RouteData.Values["id"];
            LoadCapnhat(Int32.Parse(getidUser.ToString()));
            var name = collection["name"];
            var price = collection["price"];
            var meta = collection["meta"];
            var datebegin = String.Format("{0:mm/dd/yyyy}", collection["/*ngayup*/"]);



            if (String.IsNullOrEmpty(name))
                ViewData["Err5"] = "Vui lòng nhập tiêu đề!";
            else if (String.IsNullOrEmpty(price))
                ViewData["Err5"] = "Vui lòng nhập tên đường!";
            //else if (String.IsNullOrEmpty(TENQUAN))
            //    ViewData["Err5"] = "Vui lòng chọn Giới tính!";
            else if (String.IsNullOrEmpty(meta))
                ViewData["Err5"] = "Vui lòng nhập diện tích!";
            else
            {
                var query1 = from acc in db.PRODUCTs
                             where Equals(acc.id, getidUser)
                             select acc;
                foreach (var info in query1)
                {
                    info.name = name;
                    info.price = float.Parse(price);
                    info.meta = meta;
                    info.datebegin = DateTime.Now;
                }
                db.SubmitChanges();
                ViewData["Err55"] = "(*) Cập nhật thành công !";
                return RedirectToAction("Getlistbaiviet", "Admin");
            }
            return View();
        }

        private ActionResult LoadCapnhat(int id)
        {
            IList<LoadDanhSach> infoacc = new List<LoadDanhSach>();
            var query = from acc in db.PRODUCTs
                        where Equals(acc.id, id)
                        select acc;

            var infoaccs = query.ToList();
            foreach (var info in infoaccs)
            {
                infoacc.Add(new LoadDanhSach()
                {
                    //id = info.id,
                    //TENQUAN =info.TENQUAN,
                    name = info.name,
                    meta = info.meta,
                    //ngayup =info.NGAYUP,
                    description = info.description,
                    price = float.Parse(info.price.ToString()),
                });
            }
            return View(infoacc);
        }

        public ActionResult ListBaiViet1()
        {
            if (Equals(Session["ID_ROLE"], 2)) return RedirectToAction("Index", "User");
            if (Session["TaiKhoan"] == null)
            {
                ViewData["Loi1"] = "Vui lòng đăng nhập tài khoản";
            }

            var a = from b in db.PRODUCTs
                    orderby b.datebegin descending
                    where b.hide == false
                    select b;
            return View(a.ToList());
        }

        [HttpGet]
        public ActionResult Showbai()
        {
            var getid = Url.RequestContext.RouteData.Values["id"];
            var a = from b in db.PRODUCTs
                    where Equals(b.id, getid) && b.hide == false
                    select b;
            foreach (var c in a)
            {
                c.hide = true;
                db.SubmitChanges();
            }
            return RedirectToAction("Getlistbaiviet");
        }
    }
}