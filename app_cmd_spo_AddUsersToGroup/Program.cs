using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

namespace appAddUsersToGroup
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //Move_Users();
            Enter_Group("i:0#.w|fonafedom\\rstucchi", "ad_fonafegob");
            Console.ReadKey();
        }

        static void Move_Users()
        {
            //Te Loogueas en el sitio de SharePoint
            ClientContext contexto = new ClientContext("[siteurl]");
            contexto.Credentials = new NetworkCredential("[username]", "[password]");
            Web web = contexto.Web;

            GroupCollection _SiteGroups = web.SiteGroups;
            //Obtiene El grupo Usuario Nuevo
            Group _Group = _SiteGroups.GetById(6);
            contexto.Load(_Group.Users, list => list.Where(lst => lst.Email != null));
            contexto.ExecuteQuery();

            //Se dirigira a la lista con el nombre  Users
            ListCollection lstCollection = web.Lists;
            contexto.Load(lstCollection, list => list.Where(lst => lst.RootFolder.Name == "users").OrderBy(x => x.Title));
            contexto.ExecuteQuery();

            //Obtendremos la fecha de hoy
            DateTime dtNow = DateTime.Now;

            //Realizar la consulta query , lo cual obtendrá todos los datos de 2 columnas (Nombre,Creado Por)
            /*CamlQuery query = new CamlQuery { ViewXml = "<View><Query><Where><And><IsNotNull><FieldRef Name='Title' /></IsNotNull><And><And><IsNotNull><FieldRef Name='Created' /></IsNotNull><IsNotNull><FieldRef Name='Name' /></IsNotNull></And><IsNotNull><FieldRef Name='EMail' /></IsNotNull></And></And></Where></Query></View>" }*/
            ;
            CamlQuery query = new CamlQuery { ViewXml = "<View><Query><Where><And><And><IsNotNull><FieldRef Name='Title' /></IsNotNull><IsNotNull><FieldRef Name='Created' /></IsNotNull></And><And><IsNotNull><FieldRef Name='Name' /></IsNotNull><IsNotNull><FieldRef Name='EMail' /></IsNotNull></And></And></Where><OrderBy><FieldRef Name='Title' Ascending='True' /></OrderBy></Query></View>" };

            ListItemCollection queryCorrelativo = lstCollection.FirstOrDefault()?.GetItems(query);
            contexto.Load(queryCorrelativo);
            contexto.ExecuteQuery();
            DateTime today;
            DateTime Fecha;
            string LoginName = "";
            string nombrePersona = "";
            string FechaCreacion = "";
            string cont = "";
            string con = "";

            if (queryCorrelativo != null)
            //  Hacer comparación entre fecha actual - 3 meses(90días) y si es igual el "[Created]" entonces->agregar al grupo de usuarios viejos

            {

                foreach (ListItem items in queryCorrelativo)
                {
                    nombrePersona = Convert.ToString(items["Title"]);
                    //Obtiene el Grupo Usuario Nuevo en SharePoint
                    Group _NotGroup = _SiteGroups.GetById(6);
                    contexto.Load(_NotGroup.Users, lista => lista.Where(lst => lst.Title != "Cuenta del sistema"));
                    contexto.ExecuteQuery();
                    foreach (User usu in _Group.Users)
                    {
                        con += usu.Title + " \n";

                    }
                    if (con.Contains(nombrePersona))
                    {
                        foreach (User usuario in _NotGroup.Users)
                        {
                            cont += usuario.Title + " \n";
                        }
                        if (!cont.Contains(nombrePersona))
                        {
                            FechaCreacion = Convert.ToString(items["Created"]);
                            LoginName = Convert.ToString(items["Name"]);
                            Fecha = Convert.ToDateTime(items["Created"]);
                            today = dtNow;
                            double dias = Math.Round((today - Fecha).TotalDays);
                            if (dias >= 90)
                            {
                                //Enter_Group(LoginName, nombrePersona);
                            }

                        }


                    }


                }
                Console.Write("Usuarios Agreagdo: " + nombrePersona + " \n");
                Console.ReadLine();
                //Console.WriteLine(cont);
                // Console.ReadLine();
                //cont += "Usuario: " + nombrePersona + " \n" + "Fecha Creacion: " + FechaCreacion + "\n" + "Numero de Dias: " + dias + " \n";

            }


        }
        static void Enter_Group(string Login, string nombre)
        {
            ClientContext context = new ClientContext("[siteurl]");
            context.Credentials = new NetworkCredential("[username]", "[password]");            
            GroupCollection _GroupCollection = context.Web.SiteGroups;
            Group grupo = _GroupCollection.GetById(6);
            UserCreationInformation UCI = new UserCreationInformation();
            UCI.Title = nombre;
            UCI.LoginName = @Login;
            User usuario = grupo.Users.Add(UCI);
            Web web = context.Web;
            context.Load(web);
            context.ExecuteQuery();
            Move_Users();
        }
    }    
}
