﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace procedure_kullanımı.Models.Managers
{
    public class DatabaseContext:DbContext
    {
        public DbSet<Persons> db_context_kisiler { get; set; }
        public DbSet<Addresses > db_context_adresler { get; set; }

        public DatabaseContext()
        {
            Database.SetInitializer(new DbInitializer());
        }

        public List<my_procedure_class> EXECUTE_my_procedure(int adres_idm, int kisi_idm)
        {
            return Database.SqlQuery<my_procedure_class>("exec my_procedure @p0 , @p1", adres_idm, kisi_idm).ToList();
        }

        public List<my_view_class> EXECUTE_my_view()
        {
            return Database.SqlQuery<my_view_class>("select * from vw_getter_information").ToList(); //istersen AsQueryable<> olarakta alabilirsin.
        }

        //aynı view yada procedure burada bir where daha atıp yada order by group by atıp sonunda onuda parametre de ekleyip onu da ayrı metod yapabilirsin!!!
        public List<my_view_class> EXECUTE_my_view_with_parameters(int min_age)
        {
            return Database.SqlQuery<my_view_class>("select * from vw_getter_information where yas>=@p0",min_age).ToList(); //istersen AsQueryable<> olarakta alabilirsin.
        }

    }

   

    public class DbInitializer:CreateDatabaseIfNotExists<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
            #region
            //kisiler insert ediliyor
            for (int i = 0; i < 15; i++)
            {
                Persons kisi = new Persons();
                kisi.ad = FakeData.NameData.GetFirstName();
                kisi.soyad = FakeData.NameData.GetSurname();
                kisi.yas = FakeData.NumberData.GetNumber(10, 90);

                context.db_context_kisiler.Add(kisi);

            }

            context.SaveChanges();


            //adresler insert ediliyor

            List<Persons> tum_kisiler = context.db_context_kisiler.ToList();
            foreach (Persons kisi in tum_kisiler)
            {
                for (int i = 0; i < FakeData.NumberData.GetNumber(1, 5); i++)
                {
                    Addresses adres = new Addresses();
                    adres.adres_tanim = FakeData.PlaceData.GetAddress();
                    adres.kisi = kisi;

                    context.db_context_adresler.Add(adres);
                }
            }
            context.SaveChanges();
            #endregion

            context.Database.ExecuteSqlCommand
                (
                @"create procedure my_procedure

	            @p0 int,
	            @p1 int

                as 
                begin

	            select Addresses.adres_tanim,Addresses.ID,Addresses.kisi_ID,Persons.ad,Persons.soyad,Persons.yas
	            from Persons inner join Addresses
	            on Persons.ID=Addresses.kisi_ID
	            where Addresses.ID>=@p0 and Persons.ID<@p1
	            order by Addresses.ID 
                end"
                );


            context.Database.ExecuteSqlCommand
                (
                @"create view vw_getter_information 
                as
                select ps.ad,ps.soyad,ps.yas,adrs.adres_tanim
                from Persons as ps inner join Addresses as adrs 
                on ps.ID=adrs.kisi_ID"

                );

        }
        //buraya isterseniz direkt sql kodunuzu isterseniz management studio da sql kodunu çalıştırdıktan sonra ortaya çıkan
        //procedure veya view için oluşan procedure veya view e select top 1000 rows ile de arka tarafta oluşan koduda burada kullanabilirsin
        //kendin yazdığın koduda da. ilki kendi ikincisi arka taraf kodu.
    }

   

}