namespace Junjuria.Services.InitialSeed
{
    using Junjuria.Infrastructure.Data;
    using Junjuria.Infrastructure.Models;
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class Constants
    {
        public static string UserAddress { get; } = "Ulica sezam N=";
        public static string UserPassword { get; } = "12A3bc5";
        public static IDictionary<string, int> RolesUsersCount { get; } = new Dictionary<string, int> { ["Admin"] = 1, ["User"] = 5 };

        public static readonly int NumberOfProductsToSeed = 11;
        public static string[] ProductNames = { "test chetka za zubi", "test magnitofon", "test mishka", "test Компот", "test Pensil" };
        public static string[] ProductDescription = { "light and fast", "heavy and expensive", "with cool design", "for every pocket", "best purchase of year 2001" };
        public static string ProductReviewURL = "https://youtu.be/GRxofEmo3HA";
        public static string ProductMainPicUrl = "https://www.buildabear.co.uk/dw/image/v2/BBNG_PRD/on/demandware.static/-/Sites-buildabear-master/default/dw5430adaa/26613x.jpg?sw=600&sh=600&sm=fit&q=70";

        public static Dictionary<string, string> HeadCategories = new Dictionary<string, string> { ["Za doma"] = "vseki dom trqbva da gi ima", ["Za kolata"] = "da vurvi po burzo", ["Za jenata"] = "da ne q boli glavata", ["Za tushtata"] = "description" };
        public static Dictionary<string, string> MiddleCategories = new Dictionary<string, string> { ["Za banqta"] = "kupete se po chesto", ["Za kuhnqta"] = "mekici i palachinki", ["Amortisiori"] = "leko i meko", ["Auspusi"] = "nai dobriq zvuk", ["Grim"] = "Juzepe Nacapoti", ["Bijuteria"] = "Ne e ot kitai", ["Nishto"] = "kot takoa" };
        public static Dictionary<string, string> BottomCategories = new Dictionary<string, string> { ["Mivki"] = "za miene", ["Zakachalki"] = "za zakachane" };

        public static string[] ManufacturerNames = { "Mazniq", "Kitaeca", "Beliq vojd", "Mazuta", "Marko Polo" };
        public static string ManufacturerEmail = "testov{0}Mail@mail.com";
        public static string ManufacturerPhoneNumber = "0883387849";
        public static string ManufacturerWebAddress = "empornium.ru";

        public static string[] Colors = { "Black", "Blue", "White", "Metalic" };

        public static string[] ProductContentPictureURLs = { "http://i.imgur.com/nif7ztU.jpg", "http://rockinsider.com/wp-content/uploads/2011/11/BadPanda105-Visiol-EP-500x500.jpg"
                                         ,"https://getsocial.nz/wp-content/uploads/2014/12/Evolution_by_will_yen-500x500.png"
                                         ,"https://forums.crackberry.com/attachments/blackberry-q10-f272/171886d1371181552t-bbm-display-picture-limitations-423341110329_qy737vid_l.jpg"
                                         ,"https://www.newcastlewildflower.com.au/wp-content/uploads/2013/05/The-catalyst-single-cover-500x500.png"
                                         ,"https://data.whicdn.com/images/1484820/original.jpg",
                                         "http://welde-lessocenter.com/wp-content/uploads/2017/07/welde-lessocenter-3-500x500.jpg"};
    }

    // public int ProductId { get; set; }
    // [ForeignKey(nameof(ProductId))]
    // public Product Product { get; set; }
    //
    // public string Title { get; set; }
    //
    // [MaxLength(64)]
    // public string TextValue { get; set; }
    //
    // public double? NumericValue { get; set; }

    public class DataBaseSeeder
    {
        private readonly ApplicationDbContext db;
        private readonly Random random;
        private readonly UserManager<AppUser> _UserManager;
        private readonly RoleManager<IdentityRole> _RoleManager;

        public DataBaseSeeder(Random random,
            UserManager<AppUser> _userManager,
            RoleManager<IdentityRole> _roleManager,
            ApplicationDbContext dbContext
            )
        {
            this._RoleManager = _roleManager;
            this.random = random;
            this._UserManager = _userManager;
            this.db = dbContext;
        }

        public async Task SeedData()
        {
            //  await db.Database.EnsureDeletedAsync();
            //  await db.Database.EnsureCreatedAsync();
            //  await SeedRoles(Constants.RolesUsersCount.Keys);
            //  await SeedUsers(Constants.RolesUsersCount);
            //  await SeedManufacturers();
            //  await SeedProductCategories();
            //  await SeedProducts(Constants.NumberOfProductsToSeed);
            //  await SeedProductCharacteristics();
            //  await SeedProductPicture();
        }

        private async Task SeedProductPicture()
        {
            var products = db.Products.ToArray();
            for (int i = 0; i < products.Length; i++)
            {
                if (i == 4) continue;
                var product = products[i];
                List<int> pictureIndexes = new List<int>();

                while (pictureIndexes.Count < 2)
                {
                    pictureIndexes.Add(random.Next(0, Constants.ProductContentPictureURLs.Length));
                }

                foreach (var index in pictureIndexes)
                {
                    product.ProductPictures.Add(new ProductPicture
                    {
                        PictureURL = Constants.ProductContentPictureURLs[index],
                    });
                }
            }

            await db.SaveChangesAsync();
        }

        private async Task SeedRoles(IEnumerable<string> roles)
        {
            foreach (var role in roles)
            {
                await _RoleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private async Task SeedUsers(IDictionary<string, int> rolesUsersCount)
        {
            foreach (var kvp in rolesUsersCount)
            {
                for (int i = 1; i <= kvp.Value; i++)
                {
                    string userName = kvp.Key + i;
                    var user = new AppUser()
                    {
                        FirstName = "TestUserFirstName",
                        LastName = "TestUserLastName",
                        UserName = userName,
                        Adress = Constants.UserAddress + i,
                        Email = userName + "@gmail.com",
                        Town = userName + "'s HomeTown",
                        SecurityStamp = Guid.NewGuid().ToString()
                    };
                    var res = await _UserManager.CreateAsync(user, Constants.UserPassword);
                    await _UserManager.AddToRoleAsync(user, kvp.Key);
                }
            }
        }

        private async Task SeedProductCategories()
        {
            List<Category> categories = new List<Category>();

            foreach (var kvp in Constants.HeadCategories)
            {
                categories.Add(new Category
                {
                    Title = kvp.Key,
                    Description = kvp.Value,
                });
            }
            int i = 0;
            foreach (var kvp in Constants.MiddleCategories)
            {
                Category headcategory;
                try
                {
                    headcategory = categories[(i++) / 2];
                }
                catch
                {
                    headcategory = categories.First();
                }
                categories.Add(new Category
                {
                    Title = kvp.Key,
                    Description = kvp.Value,
                    OuterCategory = headcategory
                });
            }
            i = 0;
            foreach (var kvp in Constants.BottomCategories)
            {
                Category middleCategory;
                try
                {
                    middleCategory = categories.Where(x => x.OuterCategory != null).ToArray()[(i++) / 2];
                }
                catch
                {
                    middleCategory = categories.Where(x => x.OuterCategory != null).First();
                }

                categories.Add(new Category
                {
                    Title = kvp.Key,
                    Description = kvp.Value,
                    OuterCategory = middleCategory
                });
            }
            await db.AddRangeAsync(categories);
            await db.SaveChangesAsync();
        }

        private async Task SeedManufacturers()
        {
            for (int i = 0; i < Constants.ManufacturerNames.Length; i++)
            {
                string manufacturerName = Constants.ManufacturerNames[i];
                await db.Manufacturers.AddAsync(new Manufacturer
                {
                    Name = manufacturerName,
                    Email = string.Format(Constants.ManufacturerEmail, i + 1),
                    PhoneNumber = Constants.ManufacturerPhoneNumber,
                    WebAddress = Constants.ManufacturerWebAddress
                });
            }
            await db.SaveChangesAsync();
        }

        private async Task SeedProducts(int productsCount)
        {
            var categoryIds = db.Categories.Select(x => x.Id).ToArray();
            var manufacturerIds = db.Manufacturers.Select(x => x.Id).ToArray();
            for (int i = 1; i <= productsCount; i++)
            {
                decimal price = random.Next(1, 100) + random.Next(1, 100) / 100m;
                double discount = random.Next(0, 4) <= 2 ? 0d : random.Next(1, 50) / 100d;
                double weight = random.Next(1, 50) + random.Next(1, 100) / 100d; ;
                uint quantity = (uint)(random.Next(0, 4) == 1 ? 0 : random.Next(1, 101));

                uint? warranty = random.Next(0, 4) == 2 ? null : (uint?)random.Next(6, 25);

                var newProduct = new Product
                {
                    Name = GetRandom(Constants.ProductNames),
                    Description = GetRandom(Constants.ProductDescription),
                    Price = price,
                    Discount = discount,
                    Quantity = quantity,
                    ReviewURL = Constants.ProductReviewURL,
                    MainPicURL = Constants.ProductMainPicUrl,
                    MonthsWarranty = warranty,
                    Weight = weight,
                    ManufacturerId = GetRandom(manufacturerIds),
                    CategoryId = GetRandom(categoryIds)
                };
                await db.Products.AddAsync(newProduct);
            }
            await db.SaveChangesAsync();
        }

        private async Task SeedProductCharacteristics()
        {
            Product[] products = db.Products.ToArray();

            for (int i = 0; i < products.Length; i++)
            {
                if (i == 5) continue;
                var product = products[i];
                Stack<ProductCharacteristic> characteristics = new Stack<ProductCharacteristic>();
                characteristics.Push(new ProductCharacteristic
                {
                    Title = "Material",
                    TextValue = GetRandom(Constants.Colors),
                    NumericValue = null
                });
                if (i % 3 != 0 && i != 0)
                {
                    characteristics.Push(new ProductCharacteristic
                    {
                        Title = "Width",
                        NumericValue = random.Next(12, 45),
                    });
                    characteristics.Push(new ProductCharacteristic
                    {
                        Title = "Length",
                        NumericValue = random.Next(15, 75),
                    });
                }
                product.Characteristics = new HashSet<ProductCharacteristic>(characteristics);
            }
            await db.SaveChangesAsync();
        }

        private T GetRandom<T>(IEnumerable<T> collection)
        {
            int index = this.random.Next(0, collection.Count());
            return collection.ToArray()[index];
        }

    }
}