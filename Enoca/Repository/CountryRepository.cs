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

        public bool CountryExistsByName(string Name)
        {
            return _context.Countries.Any(c => c.Name.ToLower().Trim() == Name.ToLower().Trim()); // Ülke adı var mı kontrol eder
        }

        public bool UpdateCountry(Country country)
        {
            //_context.Update() metodu entity fraemwork core da bir nesnenin durumunun güncellendi olarak işaretlemesini söyler.
            _context.Update(country);

            return Save();

        }

        public bool Save()
        {
            //_context.SaveChanges() metodu,etkilenen satır sayısını döndürür.
            //Eğer en az bir satır eklendiyse (Yani değişiklik başarılıysa) true döner.
            var saved = _context.SaveChanges();
            return saved > 0;
        }

        public bool DeleteCountry(Country country)
        {
            //_context.Remove(), Entity Fraemwork e bu nesnenin durumunun 
            //silindi olarak işaretlenmesini sağlar.
            _context.Remove(country);
            return Save();
        }
    }
}
