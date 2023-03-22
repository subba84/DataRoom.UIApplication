using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DataRooms.UI.Code
{
    public static class ADHelper
    {
        private static Logger logger = LogManager.GetLogger("myAppLoggerRules");
        public static bool ADAuth(string username, string password,ADInfo ad)
        {
            logger.Debug("Entry --> ADAuth..");
            try
            {        
                try
                {
                    SearchResultCollection results;
                    using (DirectoryEntry domainEntry = new DirectoryEntry("LDAP://" + ad.IPAddress, username, password))
                    {
                        using (DirectorySearcher searcher = new DirectorySearcher(domainEntry, "userPrincipalName=" + username + "@" + Environment.UserDomainName) { Filter = string.Format("(&(objectClass=user)(samaccountname={0}))", username) })
                        {
                            results = searcher.FindAll();
                            if (results.Count > 0)
                            {
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("ADAuth..AD Error.." + ex.Message);
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("ADAuth..AD Error.." + ex.Message);
                return false;
            }
            return false;
        }

        public static List<User> GetAllADUsers(int companyid)
        {
            List<User> users = new List<User>();
            try
            {
                int usercount = 1;
                if(companyid > 0)
                {
                    logger.Debug("Company id - " + companyid);
                    ADInfo adinfo = new ADInfo();
                    var companyADDetails = DataCache.ADConfiguration.Where(x => x.CompanyId == companyid);
                    if (companyADDetails != null && companyADDetails.Count() > 0)
                    {
                        adinfo = companyADDetails.First();
                    }
                    using (DirectoryEntry domainEntry = new DirectoryEntry("LDAP://" + adinfo.IPAddress))
                    {
                        DirectorySearcher search = new DirectorySearcher(domainEntry);
                        search.PageSize = 7000;
                        search.SearchScope = SearchScope.Subtree;
                        //search.Filter = "(&(objectCategory=*)(objectClass=*))";//(objectClass=user) (objectClass=user) DC=BE
                        search.PropertiesToLoad.Add("samaccountname");
                        search.PropertiesToLoad.Add("mail");
                        search.PropertiesToLoad.Add("usergroup");
                        search.PropertiesToLoad.Add("displayname");//first name
                        SearchResult result;
                        logger.Debug("Get All AD Users..AD Interaction..");
                        SearchResultCollection resultCol = search.FindAll();
                        if (resultCol != null)
                        {
                            for (int counter = 0; counter < resultCol.Count; counter++)
                            {
                                string UserNameEmailString = string.Empty;
                                result = resultCol[counter];

                                //User objUser = new User();
                                //var emailDetails = result.Properties["mail"];
                                //var samaccountDetails = result.Properties["samaccountname"];
                                //var displayNameDetails = result.Properties["displayname"];
                                //if (emailDetails != null && emailDetails.Count > 0)
                                //    objUser.EmailId = (String)result.Properties["mail"][0];
                                //else
                                //    objUser.EmailId = "";
                                //if (samaccountDetails != null && samaccountDetails.Count > 0)
                                //    objUser.UserName = (String)result.Properties["samaccountname"][0];
                                //else
                                //    objUser.UserName = String.Empty;
                                //if (displayNameDetails != null && displayNameDetails.Count > 0)
                                //    objUser.FullName = (String)result.Properties["displayname"][0];
                                //else
                                //    objUser.FullName = String.Empty;
                                //objUser.CompanyId = adinfo.CompanyId;
                                //objUser.CompanyName = adinfo.CompanyName;
                                //objUser.IsADUser = true;
                                //objUser.IsActive = true;
                                //users.Add(objUser);

                                if (result.Properties.Contains("samaccountname") &&
                                         result.Properties.Contains("mail") &&
                                    result.Properties.Contains("displayname"))
                                {
                                    User objUser = new User();
                                    objUser.EmailId = (String)result.Properties["mail"][0];
                                    objUser.UserName = (String)result.Properties["samaccountname"][0];
                                    objUser.FullName = (String)result.Properties["displayname"][0];
                                    objUser.CompanyId = adinfo.CompanyId;
                                    objUser.CompanyName = adinfo.CompanyName;
                                    objUser.IsADUser = true;
                                    objUser.IsActive = true;
                                    users.Add(objUser);
                                }
                                usercount++;
                            }
                        }
                    }
                }
                else
                {
                    logger.Debug("Company id - 0");
                    var adConfigs = DataCache.ADConfiguration;
                    if(adConfigs!=null && adConfigs.Count() > 0)
                    {
                        foreach(var adinfo in adConfigs.ToList())
                        {
                            using (DirectoryEntry domainEntry = new DirectoryEntry("LDAP://" + adinfo.IPAddress))
                            {
                                DirectorySearcher search = new DirectorySearcher(domainEntry);
                                search.PageSize = 7000;
                                search.SearchScope = SearchScope.Subtree;
                                //search.Filter = "(&(objectClass=user)(objectCategory=person))";
                                search.PropertiesToLoad.Add("samaccountname");
                                search.PropertiesToLoad.Add("mail");
                                search.PropertiesToLoad.Add("usergroup");
                                search.PropertiesToLoad.Add("displayname");//first name
                                SearchResult result;
                                SearchResultCollection resultCol = search.FindAll();
                                if (resultCol != null)
                                {
                                    for (int counter = 0; counter < resultCol.Count; counter++)
                                    {
                                        string UserNameEmailString = string.Empty;
                                        result = resultCol[counter];
                                        if (result.Properties.Contains("samaccountname") &&
                                                 result.Properties.Contains("mail") &&
                                            result.Properties.Contains("displayname"))
                                        {
                                            User objUser = new User();
                                            objUser.EmailId = (String)result.Properties["mail"][0];
                                            objUser.UserName = (String)result.Properties["samaccountname"][0];
                                            objUser.FullName = (String)result.Properties["displayname"][0];
                                            objUser.CompanyId = adinfo.CompanyId;
                                            objUser.CompanyName = adinfo.CompanyName;
                                            objUser.IsADUser = true;
                                            objUser.IsActive = true;
                                            users.Add(objUser);
                                        }
                                    }
                                }
                                else
                                {
                                    logger.Debug("No AD Users");
                                }
                            }
                        }
                    } 
                }

                logger.Debug("Incremental Users Count - " + usercount);
            }
            catch (Exception ex)
            {
                logger.Error("Get All AD Users..AD Error.." + ex.Message);
            }
            return users;
        }



        public async static Task ADSync(int companyid)
        {
            try
            {
                List<User> users = GetAllADUsers(companyid);
                DataCacheLoadService _dataCacheLoadService = new DataCacheLoadService();
                //logger.Debug("Users Count..." + users.Count);
                await _dataCacheLoadService.SyncADUsers(users);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}