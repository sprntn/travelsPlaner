using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using travels_server_side.DBcontext;
using travels_server_side.Entities;
using travels_server_side.Iservices;
using travels_server_side.Models;

namespace travels_server_side.Services
{
    public class SitesService : ISitesService
    {
        private readonly TravelsDbContext _travelDbContext;
        private readonly IMapper _mapper;

        private readonly int NumOfSitesToUser = 30;//המספר לדוגמא ולא לפי חישוב

        public SitesService(TravelsDbContext travelsDbContext, IMapper mapper)
        {
            _travelDbContext = travelsDbContext;
            _mapper = mapper;
        }

        public int addSite(SitesDTO site)//בדרך הזאת אי אפשר להחזיר את המספר של האתר
        {
            //validation here
            //if(validation failed){
            //  return 3;
            //}
            //SitesEO newSite = _mapper.Map<SitesEO>(site);
            SitesEO newSite = new SitesEO()
            {
                //averageRating = site.siteAverageRating,
                imageSource = site.imageSource,
                mainCategoryFK = site.mainCategoryFK,
                managerEmailFK = site.managerEmail,
                siteDescription = site.siteDescription,
                siteName = site.siteName,
                siteId = site.siteId,
                //visitsNum = site.visitsNum,
                webSite = site.webSite
            };
            if(newSite == null)
            {
                return 2;
            }
            _travelDbContext.sites.Add(newSite);

            int check = _travelDbContext.SaveChanges();
            return check;
        }

        public SitesDTO getSite(int siteId)
        {
            SitesDTO site = _mapper.Map<SitesDTO>(_travelDbContext.sites.FirstOrDefault(site => site.siteId == siteId));
            return site;
        }

        public List<SitesDTO> getSites()
        {
            List<SitesDTO> sites = _travelDbContext.sites.Select(site => _mapper.Map<SitesDTO>(site)).ToList();
            return sites;
        }

        private Dictionary<int, int> setCategoriesRating(string userEmail)
        {
            Dictionary<int, int> categorySize = new Dictionary<int, int>();
            List<UserPreferencesEO> preferences = _travelDbContext.preferences.Where(u => u.userEmail == userEmail).ToList();
            int totalRating = preferences.Sum(p => p.categoryRating);
            foreach (UserPreferencesEO preference in preferences)
            {
                //categorySize.Add(preference.categoryId, (preference.categoryRating / totalRating) * NumOfSitesToUser);
                categorySize.Add(preference.categoryId, preference.categoryRating * NumOfSitesToUser / totalRating);
            }
            //בהמשך-הוספת גודל לקטגוריות לפי היסטוריה והשוואה למשתמשים אחרים

            return categorySize;
        }
        
        private List<SitesDTO> getCategorySites_work_without_user_rating(int categoryId, int sitesNum)
        {
            //בהמשך - למצוא את האתרים הטובים ביותר לפי הסטוריה ומשתמשים דומים

            /*
            string query = "SELECT * FROM sites_tbl s INNER JOIN ( SELECT v.siteIdFK, AVG(v.rating)* COUNT(*) AS rating FROM " +
                "visits_tbl v INNER JOIN sites_tbl s ON s.siteId = v.siteIdFK WHERE s.mainCategoryFK = " + categoryId + " GROUP " + 
                "BY v.siteIdFK ) v ON s.siteId = v.siteIdFK ORDER BY v.rating DESC";
            var topSites = _travelDbContext.sites.FromSqlRaw(query).ToList();
            */

            var visitsGroup = from visit in _travelDbContext.visits
                       group visit by visit.siteIdFK into site
                       //orderby site.Average(s => s.rating)
                       select new
                       {
                           siteId = site.Key,
                           //visitsNum = site.Count(),//v => v.siteIdFK == a.Key),
                           //averageRating = site.Average(s => s.rating)
                           combinedRating = site.Count() * site.Average(s => s.rating)
                       };

            var topSites = (from visit in visitsGroup
                          join site in _travelDbContext.sites
                          on visit.siteId equals site.siteId
                          where site.mainCategoryFK == categoryId
                          orderby visit.combinedRating descending
                          select site).Take(sitesNum).ToList();


            List <SitesDTO> sites = new List<SitesDTO>();

            foreach (var row in topSites)
            {
                SitesDTO newSite = new SitesDTO()
                {
                    imageSource = row.imageSource,
                    mainCategoryFK = row.mainCategoryFK,
                    managerEmail = row.managerEmailFK,
                    //siteAverageRating = row.averageRating,
                    siteDescription = row.siteDescription,
                    siteId = row.siteId,
                    siteName = row.siteName,
                    //visitsNum = row.visitsNum,
                    webSite = row.webSite
                };
                sites.Add(newSite);
            }

            return sites;
        }

        private List<SitesDTO> getCategorySites(int categoryId, int sitesNum) 
        {
            var visitsGroup = from visit in _travelDbContext.visits
                              group visit by visit.siteIdFK into site
                              //orderby site.Average(s => s.rating)
                              select new
                              {
                                  siteId = site.Key,
                                  //visitsNum = site.Count(),//v => v.siteIdFK == a.Key),
                                  averageRating = site.Average(s => s.rating),
                                  //combinedRating = site.Count() * site.Average(s => s.rating)
                                  sumRating = site.Sum(s => s.rating)
                              };

            var topSites = (from visit in visitsGroup
                           join site in _travelDbContext.sites
                           on visit.siteId equals site.siteId
                           where site.mainCategoryFK == categoryId
                           orderby (visit.averageRating + visit.sumRating) descending//בהמשך צריך להוסיף מקדמים ליצירת החישוב
                           select new {site, visit.averageRating}).Take(sitesNum).ToList();

            List<SitesDTO> sites = new List<SitesDTO>();

            foreach (var row in topSites)
            {
                SitesDTO newSite = new SitesDTO()
                {
                    imageSource = row.site.imageSource,
                    mainCategoryFK = row.site.mainCategoryFK,
                    managerEmail = row.site.managerEmailFK,
                    siteDescription = row.site.siteDescription,
                    siteId = row.site.siteId,
                    siteName = row.site.siteName,
                    webSite = row.site.webSite,
                    //siteAverageRating = row.averageRating
                };
                sites.Add(newSite);
            }

            return sites;
        }

        private List<SitesDTO> getCategorySites_v1(int categoryId, int sitesNum, string userEmail)
        {
            var visitsGroup = from visit in _travelDbContext.visits
                              group visit by visit.siteIdFK into site
                              //orderby site.Average(s => s.rating)
                              select new
                              {
                                  siteId = site.Key,
                                  //visitsNum = site.Count(),//v => v.siteIdFK == a.Key),
                                  averageRating = site.Average(s => s.rating),
                                  //combinedRating = site.Count() * site.Average(s => s.rating)
                                  sumRating = site.Sum(s => s.rating),
                                  //visitCount = site.Count()
                              };

            
            var topSites = from visit in visitsGroup
                            join site in _travelDbContext.sites
                            on visit.siteId equals site.siteId
                            where site.mainCategoryFK == categoryId
                            //orderby (visit.averageRating + visit.sumRating) descending//בהמשך צריך להוסיף מקדמים ליצירת החישוב
                            select new { site, combinedRating = (visit.averageRating + visit.sumRating) };
            

            var ratedSites = (from row in topSites
                             join rating in _travelDbContext.ratings
                             on row.site.siteId equals rating.siteId
                             where rating.userEmail == userEmail
                             orderby (row.combinedRating + rating.rating)
                             select new {s = row.site,r = rating.rating }).Take(sitesNum).ToList();
            
            List <SitesDTO> sites = new List<SitesDTO>();

            foreach (var row in ratedSites)
            {
                SitesDTO newSite = new SitesDTO()
                {
                    imageSource = row.s.imageSource,
                    mainCategoryFK = row.s.mainCategoryFK,
                    managerEmail = row.s.managerEmailFK,
                    siteDescription = row.s.siteDescription,
                    siteId = row.s.siteId,
                    siteName = row.s.siteName,
                    webSite = row.s.webSite,
                    siteAverageRating = (double)row.r
                };
                sites.Add(newSite);
            }

            return sites;
        }

        private List<SitesDTO> getCategorySites(int categoryId, int sitesNum, string userEmail)
        {
            /*
            var visitsGroup = from visit in _travelDbContext.visits
                              group visit by visit.siteIdFK into site
                              //orderby site.Average(s => s.rating)
                              select new
                              {
                                  siteId = site.Key,
                                  //siteId = site != null ? site.Key : -1,


                                  //visitsNum = site.Count(),//v => v.siteIdFK == a.Key),
                                  
                                  averageRating = site.Average(s => s.rating),
                                  //averageRating = site != null ? site.Average(s => s.rating) : -1


                                  //combinedRating = site.Count() * site.Average(s => s.rating)
                                  //sumRating = site.Sum(s => s.rating),
                                  //visitCount = site.Count()
                              };

            var ratedSites = (from site in _travelDbContext.sites.Where(s => s.mainCategoryFK == categoryId)
                              join visit in visitsGroup
                              on site.siteId equals visit.siteId into vs
                              from vSite in vs.DefaultIfEmpty()
                              join rating in _travelDbContext.ratings.Where(r => r.userEmail == userEmail)
                              on site.siteId equals rating.siteId into rs
                              from rSite in rs.DefaultIfEmpty()
                              select new
                              {
                                  //site,
                                  //vSite,
                                  //rSite

                                  newSite = site,
                                  //rating = rSite != null ? rSite.rating : vSite != null ? vSite.averageRating : -1

                                  //rating = rSite?.rating ?? 0
                                  rating = rSite != null ? rSite.rating : -1
                                  //rating = vSite != null ? rSite.rating : -1




                                  //visitsRating = vSite.averageRating,
                                  //visitsRating = vSite?.averageRating ?? -1  ,

                                  //visitsRating = vSite != null ? vSite.averageRating: -1,
                                  //visitsRating = vSite.averageRating != -1? -2: -3,// != null ? vSite.averageRating: -1,

                                  //visitsRating = vSite != null ? vSite.averageRating : -1,

                                  //visitsRating = 1,

                                  //userRating = rSite.rating
                                  //userRating = rSite?.rating ?? -1

                                  //userRating = rSite != null ? rSite.rating: -1

                                  //userRating = 2

                              }).Take(sitesNum).ToList();
            */

            /*
            //for now just the site with the user rating in Average rating property
            var ratedSites = (from site in _travelDbContext.sites//צריך להוסיף תנאי where
                     join rating in _travelDbContext.ratings
                     on site.siteId equals rating.siteId into rs
                     from rSite in rs//.DefaultIfEmpty()
                     select new
                     {
                         newSite = site,
                         newRAting = rSite.rating
                     }).Take(sitesNum).ToList();
            */
            
            
            var dupRatedSites = (from site in _travelDbContext.sites.Where(s => s.mainCategoryFK == categoryId)
                            join rating in _travelDbContext.ratings.Where(r => r.userEmail == userEmail)
                            on site.siteId equals rating.siteId into rs
                            from rSite in rs.DefaultIfEmpty()
                            join visit in _travelDbContext.visits
                            on site.siteId equals visit.siteIdFK into vs
                            from vSite in vs.DefaultIfEmpty()
                            select new
                            {
                                newSite = site,
                                rating = rSite != null ? rSite.rating : vSite != null ? vSite.rating : 0
                            }).ToList();

            var ratedSites = from s in dupRatedSites
                             //orderby s.rating descending
                             group s by s.newSite.siteId into gSite
                             select new
                             {
                                 site = gSite.FirstOrDefault(s => s.newSite.siteId == gSite.Key)
                             };
            


            List <SitesDTO> sites = new List<SitesDTO>();

            foreach (var row in ratedSites)
            {
                SitesDTO newSite = new SitesDTO()
                {
                    imageSource = row.site.newSite.imageSource,
                    //imageSource = row.newSite.imageSource,
                    //imageSource = row.site.imageSource,
                    mainCategoryFK = row.site.newSite.mainCategoryFK,
                    managerEmail = row.site.newSite.managerEmailFK,
                    siteDescription = row.site.newSite.siteDescription,
                    siteId = row.site.newSite.siteId,
                    siteName = row.site.newSite.siteName,
                    webSite = row.site.newSite.webSite,
                    siteAverageRating = row.site.rating 
                };
                sites.Add(newSite);
            }

            return sites;
        }

        private List<SitesDTO> getCategorySites_notinuse(int categoryId, int sitesNum, string userEmail)
        {
            List<SitesDTO> sites = _travelDbContext.sites.Where(s => s.mainCategoryFK == categoryId).
                GroupJoin(_travelDbContext.visits, s => s.siteId, v => v.siteIdFK, (site, visits) => new
                {
                    newSite = site,
                    siteVisits = visits
                    //rating = visits.Average(v => v.rating)
                }).Select(s => new SitesDTO()
                {
                    imageSource = s.newSite.imageSource,
                    mainCategoryFK = categoryId,
                    managerEmail = s.newSite.managerEmailFK,
                    siteDescription = s.newSite.siteDescription,
                    siteId = s.newSite.siteId,
                    siteName = s.newSite.siteName,
                    webSite = s.newSite.webSite,
                    //siteAverageRating = s.rating
                    siteAverageRating = s.siteVisits.Average(v => v.rating)
                    //site = s.newSite,
                    //rating = s.siteVisits.Average(v => v.rating)
                    //rating = s.rating
                    
                }).ToList();
            
            return sites;
;        }

        public List<SitesDTO> getSuggestedSites(string userEmail)
        {
            /*
             *  - in setCategoriesRating
             *  
             * 1)get user preferences
             * 2)get user's rating history
             * 3)get user's visits history
             * 4)combine rating to each category
             * 5)define the rate of sites for each category
             * 
             *  - in getCategorySites
             *  
             * 6)get best similar users
             * 7)for each category get the best much sites by the similar users
             * 8)clean duplicate sites
             */
            List<SitesDTO> sites = new List<SitesDTO>();
            Dictionary<int, int> SitesNumToCategory = setCategoriesRating(userEmail);
            foreach (KeyValuePair<int, int> entry in SitesNumToCategory)//loop on the dictionary
            {
                //sites.AddRange(getCategorySites(entry.Key, entry.Value));
                sites.AddRange(getCategorySites(entry.Key, entry.Value, userEmail));
            }
            sites.Sort((a,b) => b.siteAverageRating.CompareTo(a.siteAverageRating));

            return sites;
        }

        public List<SitesDTO> getSuggestedSites()
        {
            var visitsGroup = from visit in _travelDbContext.visits
                              group visit by visit.siteIdFK into site
                              select new
                              {
                                  siteId = site.Key,
                                  //visitsNum = site.Count(),//v => v.siteIdFK == a.Key),
                                  averageRating = site.Average(s => s.rating),
                                  sumRating = site.Sum(s => s.rating)
                              };

            var topSites = (from visit in visitsGroup
                            join site in _travelDbContext.sites
                            on visit.siteId equals site.siteId
                            orderby (visit.averageRating + visit.sumRating) descending//בהמשך צריך להוסיף מקדמים ליצירת החישוב
                            select new { site, visit.averageRating }).Take(NumOfSitesToUser).ToList();

            List<SitesDTO> sites = new List<SitesDTO>();

            foreach (var row in topSites)
            {
                SitesDTO newSite = new SitesDTO()
                {
                    imageSource = row.site.imageSource,
                    mainCategoryFK = row.site.mainCategoryFK,
                    managerEmail = row.site.managerEmailFK,
                    siteDescription = row.site.siteDescription,
                    siteId = row.site.siteId,
                    siteName = row.site.siteName,
                    webSite = row.site.webSite,
                    siteAverageRating = (double)row.averageRating
                };
                sites.Add(newSite);
            }

            return sites;
        }

        public List<SitesDTO> getSuggestedSites_work_v1()
        {
            string query = "SELECT * FROM( SELECT TOP 5 v.siteIdFK,  COUNT(v.siteIdFK) AS 'count', AVG(rating) * COUNT(v.siteIdFK) AS 'combined " +
                "rating' FROM visits_tbl v GROUP BY v.siteIdFK ORDER BY 'combined rating' DESC) t INNER JOIN sites_tbl s ON t.siteIdFK = s.siteId";

            //var result = _travelDbContext.visits.FromSqlRaw(query).ToList();
            var result = _travelDbContext.sites.FromSqlRaw(query).ToList();

            List<SitesDTO> sites = new List<SitesDTO>();

            foreach (var row in result) 
            {
                SitesDTO newSite = new SitesDTO()
                {
                    imageSource = row.imageSource,
                    mainCategoryFK = row.mainCategoryFK,
                    managerEmail = row.managerEmailFK,
                    //siteAverageRating = row.averageRating,
                    siteDescription = row.siteDescription,
                    siteId = row.siteId,
                    siteName = row.siteName,
                    //visitsNum = row.visitsNum,
                    webSite = row.webSite
                };
                sites.Add(newSite);
            }

            return sites;
        }

        public List<SitesDTO> getSuggestedSites_v1()
        {
            List<SitesDTO> sites = new List<SitesDTO>();

            var sitesList = from visit in _travelDbContext.visits
                            group visit by visit.siteIdFK into site
                            orderby site.Average(s => s.rating)
                            select new
                            {
                                siteId = site.Key,
                                visitsNum = site.Count(),//v => v.siteIdFK == a.Key),
                                averageRating = site.Average(s => s.rating)
                            };
            foreach (var site in sitesList)
            {
                SitesDTO newSite = getSite(site.siteId);
                sites.Add(newSite);
            }
            return sites;
        }

        public List<SitesDTO> getSuggestedSites_v2()
        {
            var result = from site in _travelDbContext.sites
                      join visit in _travelDbContext.visits
                      on site.siteId equals visit.siteIdFK into groupVisit
                        let visitNum = groupVisit.Count()
                        let visitAvg = groupVisit.Average(v => v.rating)
                        orderby visitNum * visitAvg descending
                        select new 
                        {
                            site, 
                            visitNum, 
                            visitAvg
                        };

            List<SitesDTO> sitesList = new List<SitesDTO>();
            foreach(var row in result)
            {
                //SitesDTO newSite = _mapper.Map<SitesDTO>(row.site);
                SitesDTO newSite = new SitesDTO()
                {
                    imageSource = row.site.imageSource,
                    mainCategoryFK = row.site.mainCategoryFK,
                    managerEmail = row.site.managerEmailFK,
                    //siteAverageRating = row.site.averageRating,
                    siteDescription = row.site.siteDescription,
                    siteId = row.site.siteId,
                    siteName = row.site.siteName,
                    //visitsNum = row.site.visitsNum,
                    webSite = row.site.webSite
                };
                sitesList.Add(newSite);
            }
            return sitesList;
        }

        public List<SitesDTO> getSuggestedSites_v3()
        {
            List<SitesDTO> sites = new List<SitesDTO>();

            var res = from site in _travelDbContext.sites
                      join visit in _travelDbContext.visits
                      on site.siteId equals visit.siteIdFK // into joinedTbl
                      group site by visit into g
                      let numVisits = g.Count()
                      orderby numVisits descending
                      select new
                      {
                          g.Key.siteIdFK,
                          numVisits,
                      };
            foreach (var row in res)
            {
                SitesDTO newSite = getSite(row.siteIdFK);
                sites.Add(newSite);
            }
            return sites;
        }

        /*
        public List<SitesDTO> getSuggestedSites_v5()
        {
            //List<SitesDTO> sites = _travelDbContext.sites.GroupJoin()


            List<SitesDTO> sites = (List<SitesDTO>)(from visit in _travelDbContext.visits
                                                    join site in _travelDbContext.sites
                                                    on visit.siteIdFK equals site.siteId into joinedTbl
                                                    group visit by visit.siteIdFK into groupedVisit
                                                    orderby (groupedVisit.Average(g => g.rating) * groupedVisit.Count()) descending
                                                    select new SitesDTO()
                                                    {

                                                    };
            
            /*
            var z = from site in _travelDbContext.sites
                    join visit in _travelDbContext.visits
                    on site.siteId equals visit.siteIdFK into a
                    select new { site };
            

            var s = from site in _travelDbContext.sites
                    join visit in _travelDbContext.visits
                    on site.siteId equals visit.siteIdFK into a
                    select site;

            var y = from site in _travelDbContext.sites
                    join visit in _travelDbContext.visits
                    on site.siteId equals visit.siteIdFK into a
                    select new SitesDTO()
                    {
                        imageSource = site.imageSource,
                        siteId = site.siteId,
                        mainCategoryFK = site.mainCategoryFK,
                        managerEmail = site.managerEmailFK,
                        siteAverageRating = site.averageRating,
                        siteDescription = site.siteDescription,
                        siteName = site.siteName,
                        visitsNum = site.visitsNum,
                        webSite = site.webSite
                    };
            
                        var x = from visit in _travelDbContext.visits
                                join site in _travelDbContext.sites
                                on visit.siteIdFK equals site.siteId into joinedTbl
                                //group....
                                //order by....
                                select new SitesDTO()
                                {
                                    imageSource = 
                                }

            var w = from site in _travelDbContext.sites
                    join visit in _travelDbContext.visits
                    on site.siteId equals visit.siteIdFK into joinedTbl
                    select new
                    {
                        webSite = site.webSite,
                        visitsNum = site.visitsNum,
                        siteName = site.siteName,
                        imageSource = site.imageSource,
                        managerEmail = site.managerEmailFK,
                        mainCategoryFK = site.mainCategoryFK,
                        siteAverageRating = site.averageRating,
                        siteDescription = site.siteDescription,
                        siteId = site.siteId
                    };        

            
            

            List < SitesDTO > siteList = new List<SitesDTO>();
            
            foreach (var site in w)
            {
                siteList.Add(site);
            }


            foreach (var site in s)
            {
                
                SitesDTO newSite = new SitesDTO()
                {
                    imageSource = site.imageSource,
                    mainCategoryFK = site.mainCategoryFK,
                    managerEmail= site.managerEmailFK,
                    siteAverageRating = site.averageRating,
                    siteDescription = site.siteDescription,
                    siteId = site.siteId,
                    siteName = site.siteName,
                    visitsNum = site.visitsNum,
                    webSite = site.webSite
                };
                
                SitesDTO newSite = _mapper.Map<SitesDTO>(site);
                siteList.Add(newSite);
            }
            
            return siteList;
        }
        */

        public List<SitesDTO> getSuggestedSites_v4()
        {
            List<SitesDTO> sites = new List<SitesDTO>();

            var result = _travelDbContext.sites.GroupJoin
                (
                    _travelDbContext.visits,
                    site => site.siteId,
                    visit => visit.siteIdFK,
                    (site, visit) => new { site, visit }
                ).ToList();

            foreach(var item in result)
            {
                SitesDTO newSite = _mapper.Map<SitesDTO>(item.site);
                sites.Add(newSite);
            }
            return sites;
        }

        public List<SitesDTO> getSuggestedSites_v5()
        {
            List<SitesDTO> sites = new List<SitesDTO>();

            /*
            var result1 = from visit in _travelDbContext.visits
                         join site in _travelDbContext.sites
                         on visit.siteIdFK equals site.siteId
                         group visit by visit.siteIdFK into groupedVisit
                         orderby (groupedVisit.Average(g => g.rating) * groupedVisit.Count()) descending
                         select new
                         {
                             groupedVisit
                         };
            

            var result2 = from site in _travelDbContext.sites
                         join visit in _travelDbContext.visits
                         on site.siteId equals visit.siteIdFK
                         group visit by visit.siteIdFK into groupedVisit
                         orderby (groupedVisit.Average(g => g.rating) * groupedVisit.Count()) descending
                         select new
                         {
                             
                         };
            

            var result3 = from site in _travelDbContext.sites
                          join visit in _travelDbContext.visits
                          on site.siteId equals visit.siteIdFK into groupedVisit
                          select new SitesDTO() 
                          {
                              imageSource = site.imageSource,
                              mainCategoryFK = site.mainCategoryFK,
                              managerEmail = site.managerEmailFK,
                              siteAverageRating = site.averageRating,
                              siteDescription = site.siteDescription,
                              siteId = site.siteId,
                              siteName = site.siteName,
                              visitsNum = site.visitsNum,
                              webSite = site.webSite
                          };
            */

            //IEnumerable<SitesDTO>
            var result4 = (from site in _travelDbContext.sites.AsEnumerable()
                           join visit in _travelDbContext.visits
                           on site.siteId equals visit.siteIdFK into groupedVisit
                           select new SitesDTO()
                           {
                               imageSource = site.imageSource,
                               mainCategoryFK = site.mainCategoryFK,
                               managerEmail = site.managerEmailFK,
                               //siteAverageRating = site.averageRating,
                               siteDescription = site.siteDescription,
                               siteId = site.siteId,
                               siteName = site.siteName,
                               //visitsNum = site.visitsNum,
                               webSite = site.webSite
                           }).ToList();

            foreach (var item in result4)
            {
                //SitesDTO newSite = _mapper.Map<SitesDTO>((SitesDTO)item);
                //sites.Add(newSite);
                sites.Add(item);
            }

            return sites;
        }

        public List<SitesDTO> getManagerSites(string email)
        {
            List<SitesDTO> managerSites = _travelDbContext.sites.
                Where(s => s.managerEmailFK == email).Select(s => new SitesDTO() {
                    siteId = s.siteId,
                    imageSource = s.imageSource,
                    managerEmail = s.managerEmailFK,
                    siteName = s.siteName,
                    webSite = s.webSite,
                    //visitsNum = s.visitsNum,
                    siteDescription = s.siteDescription,
                    //siteAverageRating = s.averageRating,
                    mainCategoryFK = s.mainCategoryFK
                }).ToList();
            return managerSites;
        }

        public int updateSite_v1(SitesDTO upSite)
        {
            SitesEO site = _travelDbContext.sites.FirstOrDefault(s => s.siteId == upSite.siteId);
            if(site == null)//not found
            {
                return 2;
            }
            PropertyInfo[] properties = typeof(SitesDTO).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if(property.GetValue(upSite) != null)// && property.GetValue(upSite) != property.GetValue(site))
                {
                    property.SetValue(site, property.GetValue(upSite));
                }
                /*
                if(upSite.GetType().GetProperty(property.Name).GetValue(upSite, null) != null &&
                    site.GetType().GetProperty(property.Name).GetValue(site, null) != upSite.GetType().GetProperty(property.Name).GetValue(upSite, null))
                {
                    site.GetType().GetProperty(property.Name).SetValue(upSite, upSite.GetType().GetProperty(property.Name).GetValue(upSite, null));
                    //site.GetType().GetProperty(property.Name) = upSite.GetType().GetProperty(property.Name);
                    
                }
                */
            }
            int check = _travelDbContext.SaveChanges();
            return check;
        }

        public int updateSite(SitesDTO upSite)
        {
            SitesEO site = _travelDbContext.sites.FirstOrDefault(s => s.siteId == upSite.siteId);
            if (site == null)//not found
            {
                return 2;
            }
            //site = _mapper.Map<SitesEO>(upSite);

            site.siteName = upSite.siteName;
            site.siteDescription = upSite.siteDescription;
            site.webSite = upSite.webSite;
            site.managerEmailFK = upSite.managerEmail;
            site.mainCategoryFK = upSite.mainCategoryFK;
            site.imageSource = upSite.imageSource;

            int check = _travelDbContext.SaveChanges();
            return check;
        }

        public int deleteSite(int siteId)
        {
            SitesEO site = _travelDbContext.sites.FirstOrDefault(s => s.siteId == siteId);
            if (site == null)//site not found
            {
                return 2;
            }
            _travelDbContext.sites.Remove(site);

            int check = _travelDbContext.SaveChanges();
            return check;
        }

        public List<SiteCategoriesDTO> getCategories()
        {
            List<SiteCategoriesDTO> siteCategories = _travelDbContext.categories.Select(c => new SiteCategoriesDTO()
            {
                categoryId = c.categoryId,
                categoryName = c.categoryName
            }).ToList();
            return siteCategories;
        }

        /*
        public List<SitesDTO> DemoGetSites()
        {
            List<SitesDTO> sites = new List<SitesDTO>();

            SitesDTO site = new SitesDTO()
            {
                siteId = 1,
                imageSource = "https://cdn1.parksmedia.wdprapps.disney.com/resize/mwImage/1/433/433/75/vision-dam/digital/parks-platform/parks-global-assets/disney-world/0526ZP_1270MS_xak_R2-1x1.jpg?2021-08-05T12:44:38+00:00",
                siteAverageRating = 4.1,
                siteDescription = "some description..........................",
                siteName = "disney world",
                webSite = "https://disneyworld.disney.go.com/"
            };
                
            for(int i = 0; i < 12; i++)
            {
                sites.Add(site);
            }

            return sites;
        }
        */

        /*
        public List<SitesDTO> getUserSites(string userEmail)//demo function for now
        {
            

            List<SitesDTO> sites = new List<SitesDTO>();

            SitesDTO site = new SitesDTO()
            {
                siteId = 1,
                imageSource = "https://cdn1.parksmedia.wdprapps.disney.com/resize/mwImage/1/433/433/75/vision-dam/digital/parks-platform/parks-global-assets/disney-world/0526ZP_1270MS_xak_R2-1x1.jpg?2021-08-05T12:44:38+00:00",
                siteAverageRating = 4.1,
                siteDescription = "some description..........................",
                siteName = "disney world",
                webSite = "https://disneyworld.disney.go.com/"
            };

            for (int i = 0; i < 7; i++)
            {
                sites.Add(site);
            }

            return sites;
        }
        */

        /*
        public List<SiteCategoriesDTO> getCategories()
        {
            //demo
            List<SiteCategoriesDTO> siteCategories = new List<SiteCategoriesDTO>()
            {
                new SiteCategoriesDTO() { categoryId = 1, categoryName = "nature"},
                new SiteCategoriesDTO() { categoryId = 2, categoryName = "family"},
                new SiteCategoriesDTO() { categoryId = 3, categoryName = "museum"},
                new SiteCategoriesDTO() { categoryId = 4, categoryName = "history"}
            };
            return siteCategories;
        }
        */

        /*
        public SitesDTO DemoGetSiteById(int siteId)
        {
            SitesDTO site = new SitesDTO()
            {
                siteId = 1,
                imageSource = "https://cdn1.parksmedia.wdprapps.disney.com/resize/mwImage/1/433/433/75/vision-dam/digital/parks-platform/parks-global-assets/disney-world/0526ZP_1270MS_xak_R2-1x1.jpg?2021-08-05T12:44:38+00:00",
                siteAverageRating = 4.5,
                siteDescription = "some description..........................",
                siteName = "disney world",
                webSite = "https://disneyworld.disney.go.com/"
            };
            return site;
        }
        */

        /*
        private Dictionary<int, int> getCategoryRating(string userEmail)
        {
            Dictionary<int, int> categoryRating = new Dictionary<int, int>();

            List<UserPreferencesEO> preferences = _travelDbContext.preferences.Where(u => u.userEmail == userEmail).ToList();
            foreach(UserPreferencesEO preference in preferences)
            {
                categoryRating.Add(preference.categoryId, preference.categoryRating);
            }
            
            List<CategoriesEO> categories = _travelDbContext.categories.ToList();
            foreach (CategoriesEO category in categories)
            {
                if (! categoryRating.ContainsKey(category.categoryId))
                {
                    categoryRating.Add(category.categoryId, setCategoryRating(category.categoryId));  
                }
            }
            
            return categoryRating;
        }*/
    }
}
