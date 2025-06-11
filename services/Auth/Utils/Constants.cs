namespace Auth.Utils;

static class Constants
{
    public const int MaxPasswordLength = 36;
    public const int MinPasswordLength = 6;
    public const int MaxFirstNameLength = 36;
    public const int MinFirstNameLength = 3;
    public const int MaxRoleNameLength = 36;
    public const int MinRoleNameLength = 3;
    public const int MaxLastNameLength = 36;
    public const int MinLastNameLength = 3;
    public const int MaxEmailLength = 64;

    // address
    public const int MinCityLength = 2;
    public const int MaxCityLength = 100;
    public const int MinCountryLength = 2;
    public const int MaxCountryLength = 64;
    public const int MinStateLength = 2;
    public const int MaxStateLength = 64;
    public const int MinPostalCodeLength = 3;
    public const int MaxPostalCodeLength = 20;
    public const int MinAddressLineLength = 5;
    public const int MaxAddressLineLength = 255;

    public const int MinLongitude = -180;
    public const int MaxLongitude = 180;

    public const int MinLatitude = -90;
    public const int MaxLatitude = 90;
}