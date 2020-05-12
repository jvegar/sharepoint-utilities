using System;
using System.Linq;
using System.Net;
using System.Security;
using Microsoft.SharePoint.Client;
using SP = Microsoft.SharePoint.Client;

namespace Microsoft.SDK.SharePointServices.Samples
{
    class UsingItemCollectionPosition
    {
        static void Main()
        {
            bool O365 = true;
            //string siteUrl = "http://intranetinfra.granaymontero.com.pe/sitios/cd";
            string siteUrl = "https://gymcompe.sharepoint.com/teams/viales-controldegestion";
            string listName= "Control de gestión - DNN";
            ClientContext clientContext;
            if (O365)
            {                
                var pwd = "s1st3m42020$";
                var passWord = new SecureString();
                foreach (char c in pwd.ToCharArray()) passWord.AppendChar(c);
                SharePointOnlineCredentials myCred = new SharePointOnlineCredentials("serviciosti@gym.com.pe", passWord);
                clientContext = new ClientContext(siteUrl);
                clientContext.Credentials = myCred;
            }
            else
            {
                NetworkCredential myCred = new NetworkCredential("serviciosti", "s1st3m42020$");
                clientContext = new ClientContext(siteUrl);
                clientContext.Credentials = myCred;
            }

            LogWriter lw = new LogWriter("");

            SP.List oList = clientContext.Web.Lists.GetByTitle(listName);

            ListItemCollectionPosition itemPosition = null;

            while (true)
            {
                CamlQuery camlQuery = new CamlQuery();

                camlQuery.ListItemCollectionPosition = itemPosition;

                camlQuery.ViewXml = "<View Scope='RecursiveAll'><RowLimit>5000</RowLimit></View>";

                ListItemCollection collListItem = oList.GetItems(camlQuery);

                clientContext.Load(collListItem);

                clientContext.ExecuteQuery();

                itemPosition = collListItem.ListItemCollectionPosition;

                foreach (ListItem oListItem in collListItem)
                {
                    Console.WriteLine("Title: {0}:", oListItem["FileRef"]);
                    lw.LogWrite((oListItem["FileRef"]).ToString()+";"+ (oListItem["FSObjType"]).ToString()+";"+ (oListItem["File_x0020_Size"]).ToString());
                }
                
                if (itemPosition == null)
                {
                    break;
                }

                //Console.WriteLine("\n" + itemPosition.PagingInfo + "\n");
            }
        }

    }
}