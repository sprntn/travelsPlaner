namespace travels_server_side.Models
{
    public class UsersDTO
    {
        public string email { get; set; }//key
        public string firstName { get; set; }
        public string lastName { get; set; }
        //public DateTime birthDate { get; set; }
        public string address { get; set; }
        public string password { get; set; }
        public string phone { get; set; }
    }
}
