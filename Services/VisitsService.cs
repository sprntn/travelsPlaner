using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using travels_server_side.DBcontext;
using travels_server_side.Entities;
using travels_server_side.Iservices;
using travels_server_side.Models;

namespace travels_server_side.Services
{
    public class VisitsService : IVisitsService
    {
        private readonly TravelsDbContext _travelDbContext;
        private readonly IMapper _mapper;

        public VisitsService(TravelsDbContext travelsDbContext, IMapper mapper)
        {
            _travelDbContext = travelsDbContext;
            _mapper = mapper;
        }
        
        /*
        public bool updateVisitRating(double rating, int siteId, string userEmail)
        {
            VisitsDTO visit = demoVisits.FirstOrDefault(v => v.userEmail == userEmail && v.siteId == siteId);
            if(visit == null)
            {
                return false;
            }
            visit.rating = rating;
            //changed saved check
            return true;//demo
        }
        */

        //public List<object> getUserVisitedSites(string userEmail)
        public List<VisitedSite> getUserVisitedSites(string userEmail)
        {
            /*
            var visitedSites = from visit in _travelDbContext.visits
                               join site in _travelDbContext.sites
                               on visit.siteIdFK equals site.siteId
                               where visit.userEmailFK == userEmail
                               select new
                               {
                                   siteId = site.siteId,
                                   userEmail = userEmail,
                                   rating = visit.rating,
                                   imageSrc = site.imageSource,
                                   //date = visit.date,
                                   siteName = site.siteName,
                                   siteDescription = site.siteDescription
                               };*/
            
            var visitGroupResult = from visit in _travelDbContext.visits
                                   //group visit by new { visit.siteIdFK, visit.travelIdFK, visit.rating } into gv
                                   group visit by new { visit.siteIdFK, visit.travelIdFK} into gv
                                   select new
                                   {
                                       siteId = gv.Key.siteIdFK,
                                       travelId = gv.Key.travelIdFK,
                                       //rating = gv.Key.rating
                                       rating = gv.Max(g => g.rating)
                                   };

            /*
            string query = "SELECT v.siteIdFK AS siteId, MAX(v.rating) AS max_rating, travelIdFK AS travelId FROM visits_tbl v INNER JOIN sites_tbl s " +
                            "ON s.siteId = v.siteIdFK GROUP BY v.siteIdFK, v.travelIdFK";

            var visitGroupResult = _travelDbContext.visits.FromSqlRaw(query).ToList();
            */

            var result = from site in _travelDbContext.sites
                         join visit in visitGroupResult
                         //on site.siteId equals visit.siteIdFK
                         on site.siteId equals visit.siteId
                         join travel in _travelDbContext.travels
                         //on visit.travelIdFK equals travel.travelId
                         on visit.travelId equals travel.travelId
                         where travel.userEmailFK == userEmail

                         select new
                         {
                             site,
                             visit,
                             userEmail = travel.userEmailFK
                         };

            List<VisitedSite> visitedSites = new List<VisitedSite>();
            foreach (var item in result)
            {
                VisitedSite site = new VisitedSite()
                {
                    imageSrc = item.site.imageSource,
                    siteDescription = item.site.siteDescription,
                    siteName = item.site.siteName,
                    siteId = item.visit.siteId,
                    //siteId = item.visit.siteIdFK,
                    rating = (int)item.visit.rating,
                    userEmail = item.userEmail
                };
                visitedSites.Add(site);
            }
            return visitedSites;
            //https://cdn1.parksmedia.wdprapps.disney.com/resize/mwImage/1/433/433/75/vision-dam/digital/parks-platform/parks-global-assets/disney-world/0526ZP_1270MS_xak_R2-1x1.jpg?2021-08-05T12:44:38+00:00",
        }

        /*
        public bool updateVisit(VisitsDTO updatedVisit)
        {
            VisitsEO visit = _travelDbContext.visits.FirstOrDefault(
                v => v.siteIdFK == updatedVisit.siteId && v.userEmailFK == updatedVisit.userEmail);
            if(visit != null)
            {
                //update
                visit.rating = updatedVisit.rating;
            }
            else
            {
                return false;
            }
            int c = _travelDbContext.SaveChanges();
            return c > 0;
        }
        */

        public List<VisitsDTO> getVisits()
        {
            //List<VisitsDTO> visits = _travelDbContext.visits.Select(visit => _mapper.Map<VisitsDTO>(visit)).ToList();
            List<VisitsDTO> visits = _travelDbContext.visits.Select(visit => new VisitsDTO()
            {
                visitId = visit.visitId,
                //date = visit.date,
                travelId = visit.travelIdFK,
                executed = visit.executed,
                rating = (int)visit.rating,
                siteId = visit.siteIdFK,
                //userEmail = visit.userEmailFK
                datetime = visit.date//.ToString()
            }).ToList();
            return visits;
        }

        public List<VisitsDTO> getTravelVisits(int travelId)
        {
            List<VisitsDTO> visits = _travelDbContext.visits
                .Where(v => v.travelIdFK == travelId).Select(visit => _mapper.Map<VisitsDTO>(visit)).ToList();
            return visits;
        }

        public int addVisit_v1(VisitsDTO visit)
        {
            VisitsEO newVisit = _mapper.Map<VisitsEO>(visit);
            if (newVisit == null)
            {
                return 2;
            }

            _travelDbContext.visits.Add(newVisit);

            int check = _travelDbContext.SaveChanges();
            return check;
        }

        public int addVisit(VisitsDTO visit)
        {
            VisitsEO newVisit = _mapper.Map<VisitsEO>(visit);
            if (newVisit == null)
            {
                return -1;
            }

            _travelDbContext.visits.Add(newVisit);

            int check = _travelDbContext.SaveChanges();
            if(check == 0)
            {
                return 0;
            }
            return newVisit.visitId;
        }

        public int updateVisit(VisitsDTO visit)
        {
            VisitsEO upVisit = _travelDbContext.visits.FirstOrDefault(v => v.visitId == visit.visitId);
            if (upVisit == null)
            {
                return 2;
            }
            upVisit.date = DateTime.Parse(visit.datetime + ":000");
            upVisit.rating = visit.rating;
            upVisit.executed = visit.executed;
            upVisit.siteIdFK = visit.siteId;
            upVisit.travelIdFK = visit.travelId;

            int changeVisit = _travelDbContext.SaveChanges();
            return changeVisit;
        }


        public int updateVisitDatetime(VisitsDTO visit)
        {
            VisitsEO upVisit = _travelDbContext.visits.FirstOrDefault(v => v.visitId == visit.visitId);
            if (upVisit == null)
            {
                return 2;
            }
            //upVisit.date = DateTime.Parse(visit.datetime + ":000");// visit.date;
            upVisit.date = visit.datetime;
            int changeVisit = _travelDbContext.SaveChanges();
            return changeVisit;
        }


        public int updateRating(string userEmail, int siteId, int newRating)
        {
            var userVisitsList = (from visit in _travelDbContext.visits
                              join travel in _travelDbContext.travels
                              on visit.travelIdFK equals travel.travelId
                              where travel.userEmailFK == userEmail && visit.siteIdFK == siteId
                              select visit).ToList();

            if(userVisitsList == null)
            {
                return -1;
            }
            foreach(var visit in userVisitsList)
            {
                VisitsEO upVisit = _travelDbContext.visits.FirstOrDefault(v => v.visitId == visit.visitId);
                upVisit.rating = newRating;
            }
            //foreach(VisitsEO visit in upVisits)
            //{
            //    visit.rating = newRating;
            //}
            int check = _travelDbContext.SaveChanges();
            return check;
        }

        public int deleteVisit(VisitKeys keys)
        {
            throw new System.NotImplementedException();
        }

        public int deleteVisitById(int visitId)
        {
            VisitsEO visit = _travelDbContext.visits.FirstOrDefault(v => v.visitId.Equals(visitId));
            if(visit == null)
            {
                return 2;
            }

            _travelDbContext.visits.Remove(visit);
            int check = _travelDbContext.SaveChanges();
            return check;
        }

        public VisitsDTO getVisit(VisitKeys keys)
        {
            throw new System.NotImplementedException();
        }

        public VisitsDTO getVisitsById(int visitId)
        {
            VisitsDTO visit = _travelDbContext.visits.Select(v => new VisitsDTO()
            {
                visitId = v.visitId,
                //datetime = v.date.ToString(),
                datetime = v.date,
                executed = v.executed,
                rating = v.rating,
                siteId = v.siteIdFK,
                travelId = v.travelIdFK
            }).FirstOrDefault(v => v.visitId == visitId);
            return visit;
        }
    }
}
