namespace travels_server_side.Iservices
{
    public interface IAuthService
    {
        //demos
        public string demoToken(string userEmail);

        //reals
        public string authenticate(string userEmai);
    }
}
