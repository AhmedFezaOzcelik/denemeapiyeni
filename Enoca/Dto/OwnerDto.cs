using Enoca.Models;

namespace Enoca.Dto
{
    public class OwnerDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }



        public string Gym { get; set; }

        //Birebir ilişki:Her Owner (sahip) in bir Country si (ülkesi) olur.
        public Country Country { get; set; }
    }
}
