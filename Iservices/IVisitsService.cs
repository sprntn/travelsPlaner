﻿using System;
using System.Collections.Generic;
using travels_server_side.Models;

namespace travels_server_side.Iservices
{
    public interface IVisitsService
    {
        //public List<object> DemoGetVisitedSites(string userEmail);
        //public bool demoUpdateVisit(VisitsDTO updatedVisit);
        
        //public List<object> getUserVisitedSites(string userEmail);
        public List<VisitedSite> getUserVisitedSites(string userEmail);
        List<VisitsDTO> getVisits();
        List<VisitsDTO> getTravelVisits(int travelId);
        int addVisit(VisitsDTO visit);
        int updateVisit(VisitsDTO visit);
        int deleteVisit(VisitKeys keys);
        VisitsDTO getVisit(VisitKeys keys);
        int updateRating(string userEmail, int siteId, int newRating);
        int updateVisitDatetime(VisitsDTO visit);
        VisitsDTO getVisitsById(int visitId);
        int deleteVisitById(int visitId);
    }
}
