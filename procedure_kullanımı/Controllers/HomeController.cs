using procedure_kullanımı.Models;
using procedure_kullanımı.Models.Managers;
using procedure_kullanımı.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace procedure_kullanımı.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [HttpGet]
        public ActionResult homepage()
        {
            DatabaseContext db = new DatabaseContext();
            List<Persons> kisiler_listesi = db.db_context_kisiler.ToList(); //select * from kisiler //aynı zamanda database olusumunu tetikler bu select islemi

            HomePageViewModel model = new HomePageViewModel();
            model.kisiler_primitive_object = db.db_context_kisiler.ToList(); //database den liste şeklinde kişileri döner
            model.adresler_primitive_object= db.db_context_adresler.ToList(); //database den liste şeklinde adresleri döner
            ViewBag.control = true;
            List<my_view_class> result_1 = db.EXECUTE_my_view(); //View den gelen veriyi de burada aldık sayfaya göndermedik sadece örnek olsun diye buraya kadar getirdik.

            List<my_view_class> result_2 = result_1.Where(x => x.yas >= 40).ToList();//istersen gelen View verisini burada Where gibi birçok metodla bir daha sorgulayıp bir çok işlem yapabilirsin.

            List<my_view_class> result_3 = db.EXECUTE_my_view_with_parameters(40);

            //result_3 ile result_2  un aynı sorguyu döndüğüne dikkat ediniz fark result_2 de veriyi result_1 ile direkt getirip controller da where sorgusu yapıp result_2 yi elde ettik
            //result_3 de ise direkt result_1 i döndüren metodun sql sonuna bir where ve @p0,parametre ekleyip başka bir metod daha yaratıp direk DatabaseContext.cs den sonucun sorgulanmış
            //halini üstelikte bizim istediğimiz gibi parametre gönderebildiğimiz bir tik daha gelişmiş halini yazdık.
            //DbSet olarakta ekleme yolu var onu sen araştır murat abi isterseniz araştırın dedi.
            return View(model);



        }

        //[HttpPost, ActionName("homepage")]
        [HttpPost]
        public ActionResult homepage(HomePageViewModel c)
        {
            DatabaseContext db = new DatabaseContext();
            List<my_procedure_class> result = db.EXECUTE_my_procedure(c.alt_sinir, c.ust_sinir);

            HomePageViewModel md = new HomePageViewModel();
            md.my_procedure_class_primitive_object = result.ToList();
            ViewBag.control = false;
            // md.my_procedure_class_primitive_object = result.ToList();

            return View(md);
        }
    }
}

//NOT: ayrıca her defasında proje database i silip bir daha çalıştırmana gerek yok her Procedure ve View eklediğinde zaten management studio da sen 
// direkt procedure veya view i çalıştırınca projeye ekliyor sende gerekli kodları .NET tarafında ekleyip karsılayacagı sınıfları ve çağırılacagı metodu yazarsan zaten 
// var olan Management Studio tarafında eklediğin procedure veya view i HomeController altında bulunan Action içerisinden rahatlıkla çağırabilirsin.