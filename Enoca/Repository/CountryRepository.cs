using AutoMapper;
using Enoca.Data;
using Enoca.Interfaces;
using Enoca.Models;

namespace Enoca.Repository
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DataContext _context; // Veritabanı bağlantısı
        private readonly IMapper _mapper;

        public CountryRepository(DataContext context,IMapper mapper)
        {
            _context = context; // Context'i constructor ile alır
            _mapper = mapper;
        }

        public ICollection<Country> GetCountries()
        {
            return _context.Countries.ToList(); // Tüm ülkeleri getirir
        }

        public Country GetCountry(int id)
        {
            return _context.Countries.FirstOrDefault(c => c.Id == id); // ID'ye göre ülke getirir
        }

        public Country GetCountryByOwner(int ownerId)
        {
            return _context.Owners.Where(o => o.Id == ownerId).Select(o => o.Country).FirstOrDefault(); // Owner'a göre ülke getirir
        }

        public ICollection<Owner> GetOwnersFromCountry(int countryId)
        {
            return _context.Owners.Where(o => o.Country.Id == countryId).ToList(); // Ülkeye ait sahipleri getirir
        }

        public bool CountryExists(int Id)
        {
            return _context.Countries.Any(c => c.Id == Id); // Ülke var mı kontrol eder
        }
        public void AddCountry(Country country)
        {
            _context.Countries.Add(country);
            _context.SaveChanges();
        }
    }
}
