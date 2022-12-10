using System.Collections.Generic;
using System.Linq;
using travels_server_side.DBcontext;
using travels_server_side.Entities;
using travels_server_side.Iservices;
using travels_server_side.Models;

namespace travels_server_side.Services
{
    public class TravelsService: ITravelsService
    {
        private readonly TravelsDbContext _travelDbContext;

        public TravelsService(TravelsDbContext travelsDbContext)
        {
            _travelDbContext = travelsDbContext;
        }

        public TravelsDTO getTravel(int travelId)
        {
            TravelsDTO travel = _travelDbContext.travels.Where(t => t.travelId == travelId).Select(t => new TravelsDTO() {
                //adultsNum = t.adultsNum,
                //childrenNum = t.childrenNum,
                participantsNum = t.participantsNum,
                beginDate = t.beginDate,
                endDate = t.endDate,
                userEmail = t.userEmailFK,
                travelId = t.travelId,
                //travelPlan = getTravelVisits(t.travelId)
            }).FirstOrDefault();
            travel.travelPlan = getTravelVisits(travel.travelId);

            return travel;
        }

        private List<VisitsDTO> getTravelVisits(int travelId)
        {
            /*
            List<VisitsDTO> visits = _travelDbContext.visits.Where(v => v.travelIdFK == travelId)
                .Select(v => new VisitsDTO() {
                    userEmail = v.userEmailFK,
                    siteId = v.siteIdFK,
                    rating = v.rating,
                    date = v.date,
                    executed = v.executed
                }).ToList();
            return visits;
            */
            List<VisitsDTO> visits = _travelDbContext.visits.Where(v => v.travelIdFK == travelId).Select(v => new VisitsDTO() {
                //date = v.date,
                visitId = v.visitId,
                executed = v.executed,
                //rating = (int)v.rating,
                rating = v.rating,
                siteId = v.siteIdFK
                //userEmail = v.userEmailFK
            }).ToList();
            return visits;
        }

        public List<TravelsDTO> getUserTravels_v5(string userEmail)
        {
            var userTravels = from travel in _travelDbContext.travels.Where(t => t.userEmailFK == userEmail)
                              join visit in _travelDbContext.visits
                              on travel.travelId equals visit.travelIdFK
                              into travelVisits
                              select new
                              {
                                  travel = travel,
                                  visits = travelVisits
                                  //travel,
                                  //travelVisits
                              };//).ToList();
            List<TravelsDTO> travels = new List<TravelsDTO>();
            foreach (var row in userTravels)
            {
                TravelsDTO newTravel = new TravelsDTO()
                {
                    userEmail = userEmail,
                    beginDate = row.travel.beginDate,
                    endDate = row.travel.endDate,
                    participantsNum = row.travel.participantsNum,
                    travelId = row.travel.travelId
                };
                foreach (VisitsEO visits in row.visits)
                {
                    VisitsDTO newVisit = new VisitsDTO()
                    {
                        //date = visits.date,
                        executed = visits.executed,
                        rating = visits.rating,
                        siteId = visits.siteIdFK,
                        travelId = visits.travelIdFK,
                        visitId = visits.visitId
                    };
                    newTravel.travelPlan.Add(newVisit);
                }
            }
            return travels;
        }

        public List<TravelsDTO> getUserTravels_v4(string userEmail)
        {
            //where...
            List<TravelsDTO> userTravels = _travelDbContext.travels.GroupJoin(
                _travelDbContext.visits,
                t => t.travelId,
                v => v.travelIdFK,
                (t, v) => new
                {
                    travel = t,
                    visits = v
                }).Select(r => new TravelsDTO()
                {
                    travelId = r.travel.travelId,
                    beginDate = r.travel.beginDate,
                    endDate = r.travel.endDate,
                    participantsNum = r.travel.participantsNum,
                    userEmail = userEmail,
                    travelPlan = r.visits.Select(v => new VisitsDTO() 
                    {
                        //date = v.date,
                        executed = v.executed,
                        rating = v.rating,
                        siteId = v.siteIdFK,
                        travelId = v.travelIdFK,
                        visitId = v.visitId
                    }).ToList()
                }).ToList();
            return userTravels;
        }

        public List<TravelsDTO> getUserTravels_v2(string userEmail)
        //public List<TravelsDTO> getUserTravels(string userEmail)
        {
            List<TravelsDTO> userTravels = _travelDbContext.travels.Where(t => t.userEmailFK == userEmail)
                .Select(t => new TravelsDTO() 
                {
                    userEmail = userEmail,
                    beginDate = t.beginDate,
                    endDate = t.endDate,
                    participantsNum = t.participantsNum,
                    travelId = t.travelId,
                    travelPlan = _travelDbContext.visits.Where(v => v.travelIdFK == t.travelId).Select(v => new VisitsDTO() 
                    {
                        travelId = v.travelIdFK,
                        //date = v.date,
                        executed = v.executed,
                        rating = v.rating,
                        siteId = v.siteIdFK,
                        visitId = v.visitId
                    }).ToList()
                }).ToList();
            return userTravels;
        }

        public List<TravelsDTO> getUserTravels_v1(string userEmail)
        //public List<TravelsDTO> getUserTravels(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                return null;
            }
            List<TravelsDTO> travels = _travelDbContext.travels
                //.AsEnumerable()
                .Where(t => t.userEmailFK == userEmail)
                .GroupJoin(_travelDbContext.visits,
                t => t.travelId,
                v => v.travelIdFK,
                (travel, visits) => new TravelsDTO() 
                {
                    travelId = travel.travelId,
                    beginDate = travel.beginDate,
                    endDate = travel.endDate,
                    participantsNum = travel.participantsNum,
                    userEmail = userEmail,
                    
                    travelPlan = visits.Select(v => new VisitsDTO() 
                    {
                        //date = v.date,
                        rating = v.rating,
                        siteId = v.siteIdFK,
                        travelId = v.travelIdFK,
                        visitId = v.visitId,
                        executed = v.executed
                    }).ToList()
                    
                }).ToList();
            return travels;
        }

        public List<TravelsDTO> getUserTravels_v3(string userEmail) 
        //public List<TravelsDTO> getUserTravels(string userEmail) 
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                return null;
            }
            var userTravels = (from travel in _travelDbContext.travels
                               where travel.userEmailFK == userEmail
                               join visit in _travelDbContext.visits//.DefaultIfEmpty()
                               on travel.travelId equals visit.travelIdFK into gt
                               where travel.userEmailFK == userEmail
                               select new
                               {
                                   travel = travel,
                                   visits = gt
                               }).ToList();
            List<TravelsDTO> travels = new List<TravelsDTO>();
            foreach (var row in userTravels)
            {
                TravelsDTO newTravel = new TravelsDTO()
                {
                    userEmail = userEmail,
                    beginDate = row.travel.beginDate,
                    endDate = row.travel.endDate,
                    participantsNum = row.travel.participantsNum,
                    travelId = row.travel.travelId
                };
                foreach(VisitsEO visits in row.visits)
                {
                    VisitsDTO newVisit = new VisitsDTO()
                    {
                        //date = visits.date,
                        executed = visits.executed,
                        rating = visits.rating,
                        siteId = visits.siteIdFK,
                        travelId = visits.travelIdFK,
                        visitId = visits.visitId
                    };
                    newTravel.travelPlan.Add(newVisit);
                }
            }
            return travels;
        }

        public List<TravelsDTO> getUserTravels_v6(string userEmail) 
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                return null;
            }
            var userTravels = from travel in _travelDbContext.travels.Where(t => t.userEmailFK == userEmail)
                              let visits = _travelDbContext.visits.Where(v => v.travelIdFK == travel.travelId)
                              select new { travel, visits };
                               //).ToList();
            List<TravelsDTO> travels = new List<TravelsDTO>();
            foreach(var row in userTravels)
            {
                TravelsDTO newTravel = new TravelsDTO()
                {
                    userEmail = userEmail,
                    beginDate = row.travel.beginDate,
                    endDate = row.travel.endDate,
                    participantsNum = row.travel.participantsNum,
                    travelId = row.travel.travelId
                };
                foreach (VisitsEO visits in row.visits)
                {
                    VisitsDTO newVisit = new VisitsDTO()
                    {
                        //date = visits.date,
                        executed = visits.executed,
                        rating = visits.rating,
                        siteId = visits.siteIdFK,
                        travelId = visits.travelIdFK,
                        visitId = visits.visitId
                    };
                    newTravel.travelPlan.Add(newVisit);
                }
                travels.Add(newTravel);
            }
            return travels;
        }

        public List<TravelsDTO> getUserTravels_v7(string userEmail) 
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                return null;
            }
            List<TravelsDTO> userTravels = (from travel in _travelDbContext.travels.Where(t => t.userEmailFK == userEmail)
                               join visit in _travelDbContext.visits
                               on travel.travelId equals visit.travelIdFK into gVisits
                               //from v in gVisits
                               select new TravelsDTO()
                               {
                                   beginDate = travel.beginDate,
                                   endDate = travel.endDate,
                                   participantsNum = travel.participantsNum,
                                   travelId = travel.travelId,
                                   userEmail = travel.userEmailFK,
                                   travelPlan = gVisits.Select(v => new VisitsDTO() 
                                   {
                                       travelId = v.travelIdFK,
                                       executed = v.executed,
                                       rating = v.rating,
                                       siteId = v.siteIdFK,
                                       visitId = v.visitId
                                   }).ToList()
                               }).ToList();

            return userTravels;
        }

        public List<TravelsDTO> getUserTravels(string userEmail)
        {
            /*
            var userTravels = (from travel in _travelDbContext.travels.Where(t => t.userEmailFK == userEmail)
                              join visit in _travelDbContext.visits
                              on travel.travelId equals visit.travelIdFK
                              orderby travel.travelId
                              select new { travel, visit }).ToList();*/

            var userTravels = _travelDbContext.travels.Where(t => t.userEmailFK == userEmail).Join(
                _travelDbContext.visits,
                t => t.travelId,
                v => v.travelIdFK,
                (t, v) => new { travel = t, visit = v }).OrderBy(r => r.travel.travelId).ToList();

            List<TravelsDTO> travelsList = new List<TravelsDTO>();

            
            int length = userTravels.Count();
            for (int i = 0; i < length; i++)
            {
                TravelsDTO travel = new TravelsDTO()
                {
                    beginDate = userTravels[i].travel.beginDate,
                    endDate = userTravels[i].travel.endDate,
                    participantsNum = userTravels[i].travel.participantsNum,
                    travelId = userTravels[i].travel.travelId,
                    userEmail = userTravels[i].travel.userEmailFK,
                    travelPlan = new List<VisitsDTO>()
                };
                while(i < length && userTravels[i].travel.travelId == travel.travelId)
                {
                    travel.travelPlan.Add(new VisitsDTO() 
                    {
                        //date = userTravels[i].visit.date,
                        executed = userTravels[i].visit.executed,
                        rating = userTravels[i].visit.rating,
                        travelId = userTravels[i].visit.travelIdFK,
                        siteId = userTravels[i].visit.siteIdFK,
                        visitId = userTravels[i].visit.visitId
                    });
                    i++;
                }
                travelsList.Add(travel);
                i--;
            }
            
            return travelsList;
        }

        public List<SelectedSite> getTravelSites(int travelId)
        {
            List<SelectedSite> sites = _travelDbContext.visits.Where(v => v.travelIdFK == travelId).Join(
                _travelDbContext.sites,
                v => v.siteIdFK,
                s => s.siteId,
                (v, s) => new SelectedSite()
                {
                    siteId = s.siteId,
                    imageSource = s.imageSource,
                    siteDescription = s.siteDescription,
                    siteName = s.siteName,
                    webSite = s.webSite,
                    datetime = v.date,
                    visitId = v.visitId
                }).ToList();

            return sites;
        }

        public List<TravelsDTO> getTravels()
        {
            List<TravelsDTO> travels = _travelDbContext.travels.Select(t => new TravelsDTO()
            {
                travelId = t.travelId,
                //adultsNum = t.adultsNum,
                //childrenNum = t.childrenNum,
                participantsNum = t.participantsNum,
                beginDate = t.beginDate,
                endDate = t.endDate,
                userEmail = t.userEmailFK,
                //travelPlan = getTravelVisits(t.travelId)
            }).ToList();
            foreach(TravelsDTO travel in travels)
            {
                travel.travelPlan = getTravelVisits(travel.travelId);
            }
            return travels;
        }


        public int addEmptyTravel(TravelsDTO travel)
        {
            TravelsEO newTravel = new TravelsEO()
            {
                beginDate = travel.beginDate,
                endDate = travel.endDate,
                participantsNum = travel.participantsNum,
                userEmailFK = travel.userEmail,
            };
            _travelDbContext.travels.Add(newTravel);
            _travelDbContext.SaveChanges();
            int id = newTravel.travelId;
            return id;
        }

        public int addTravel(TravelsDTO travel)
        {
            TravelsEO newTravel = new TravelsEO()
            {
                //travelId = travel.travelId,
                //adultsNum = travel.adultsNum,
                beginDate = travel.beginDate,
                endDate = travel.endDate,
                //childrenNum = travel.childrenNum,
                participantsNum = travel.participantsNum,
                userEmailFK = travel.userEmail,
            };
            _travelDbContext.travels.Add(newTravel);
            if (newTravel == null)
            {
                return 2;
            }

            /*
            foreach (VisitsDTO v in travel.travelPlan)
            {
                VisitsEO newVisit = new VisitsEO()
                {
                    date = v.date,
                    executed = v.executed,
                    rating = v.rating,
                    siteIdFK = v.siteId,
                    userEmailFK = v.userEmail
                };
                _travelDbContext.visits.Add(newVisit);
            }
            */

            int changeTravel = _travelDbContext.SaveChanges();
            return changeTravel;
        }

        public int updateTravel(TravelsDTO upTravel)
        {
            TravelsEO travel = _travelDbContext.travels.FirstOrDefault(t => t.travelId == upTravel.travelId);
            if(travel == null)
            {
                return 2;
            }

            //travel.childrenNum = upTravel.childrenNum;
            //travel.adultsNum = upTravel.childrenNum;
            travel.participantsNum = upTravel.participantsNum;
            travel.beginDate = upTravel.beginDate;
            travel.endDate = upTravel.endDate;
            travel.userEmailFK = upTravel.userEmail;

            int changeTravel = _travelDbContext.SaveChanges();
            return changeTravel;
        }

        public int executeTravel(int travelId)
        {
            List<VisitsEO> visits = _travelDbContext.visits.Where(v => v.travelIdFK == travelId).ToList();
            if(visits == null || visits.Count == 0)
            {
                return 2;
            }
            foreach(VisitsEO vis in visits)
            {
                vis.executed = true;
            }

            int changeTravel = _travelDbContext.SaveChanges();
            return changeTravel;
        }

        public int deleteTravel(int travelId)
        {
            TravelsEO travel = _travelDbContext.travels.FirstOrDefault(t => t.travelId == travelId);
            if (travel == null)//site not found
            {
                return -1;
            }
            //if travel.userEmail !=  userEmail return -2

            List<VisitsEO> travelVisits = _travelDbContext.visits.Where(v => v.travelIdFK.Equals(travelId)).ToList();
            foreach (VisitsEO visit in travelVisits)
            {
                _travelDbContext.Remove(visit);
            }
            _travelDbContext.travels.Remove(travel);

            int check = _travelDbContext.SaveChanges();
            return check;
        }
    }
}
