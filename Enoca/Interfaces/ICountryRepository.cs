using Enoca.Models;

namespace Enoca.Interfaces
{
    public interface ICountryRepository
    {
        ICollection<Country> GetCountries();
        // Tüm ülkeleri (Country) veritabanından liste olarak getirir.
        Country GetCountry(int id);
        // Belirli bir ID'ye sahip ülkeyi getirir.
        Country GetCountryByOwner(int ownerId);
        // Belirli bir owner (sahip) ID'sine sahip olan ülkeyi getirir.
        // (Yani, bir Pokemon sahibi hangi ülkedense, o ülkeyi bulmak için kullanılır.)
        ICollection<Owner>GetOwnersFromCountry(int countryId);
        // Belirli bir ülkeye (countryId) ait tüm sahipleri (Owner) getirir.
        bool CountryExists(int Id);
        // Veritabanında, verilen ID'ye sahip bir ülke olup olmadığını kontrol eder.
    }
}
