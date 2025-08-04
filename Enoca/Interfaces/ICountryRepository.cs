using Enoca.Models;

namespace Enoca.Interfaces
{
    public interface ICountryRepository
    {
        ICollection<Country> GetCountries(); // Tüm ülkeleri getirir
        Country GetCountry(int id); // ID'ye göre ülke getirir
        Country GetCountryByOwner(int ownerId); // Owner'a göre ülke getirir
        ICollection<Owner> GetOwnersFromCountry(int countryId); // Ülkeye ait sahipleri getirir
        bool CountryExists(int Id); // Ülke var mı kontrol eder
        void AddCountry(Country country); // Yeni ülke ekler

        bool CountryExistsByName(string Name);

        bool UpdateCountry(Country country);

        bool DeleteCountry(Country country);

        bool Save();
    }
}
