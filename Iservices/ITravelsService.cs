using System.Collections.Generic;
using travels_server_side.Models;

namespace travels_server_side.Iservices
{
    public interface ITravelsService
    {
        int addTravel(TravelsDTO travel);
        TravelsDTO getTravel(int travelId);
        List <TravelsDTO> getTravels();
        int deleteTravel(int travelId);
        int updateTravel(TravelsDTO travel);
        int executeTravel(int travelId);
        int addEmptyTravel(TravelsDTO travel);
        List<TravelsDTO> getUserTravels(string userEmail);
        List<SelectedSite> getTravelSites(int travelId);
    }
}
