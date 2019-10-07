namespace Junjuria.Services.Services
{
    public static class ConcurencyMaster
    {
        public static object LockProductsObj = new object();
        public static object LockManufacturersObj = new object();
    }
}