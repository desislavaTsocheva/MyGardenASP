using MyGardenWEB.Data;
using MyGardenWEB.Models;

namespace MyGardenWEB.Services
{
    public class ImagesBuilder
    {
        private readonly MyGardenDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private string wwwroot;

        public ImagesBuilder(MyGardenDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            wwwroot = $"{this._hostEnvironment.WebRootPath}";
        }

        ///
        public async Task CreateImages(ProductsVM model)
        {
            Product productToDb = new Product()
            {
                BulgarianName = model.BulgarianName,
                LatinName = model.LatinName,    
                Size = model.Size,  
                Price = model.Price,
                Description = model.Description
            };
            await _context.Products.AddAsync(productToDb);
            await this._context.SaveChangesAsync();

            //var wwwroot = $"{this._hostEnvironment.WebRootPath}";
            //създаваме папката images, ако не съществува
            Directory.CreateDirectory($"{wwwroot}/ProductImages/");
            var imagePath = Path.Combine(wwwroot, "ProductImages");
            string uniqueFileName = null;
            if (model.ImagePath.Count > 0)
            {
                for (int i = 0; i < model.ImagePath.Count; i++)
                {
                    Photo dbImage = new Photo()
                    {
                        ProductsId = productToDb.Id,
                        Products = productToDb
                    };//id се създава автоматично при създаване на обект
                    if (model.ImagePath[i] != null)
                    {
                        uniqueFileName = dbImage.Id + "_" + model.ImagePath[i].FileName;
                        string filePath = Path.Combine(imagePath, uniqueFileName);
                        using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.ImagePath[i].CopyToAsync(fileStream);
                        }

                        dbImage.Url = uniqueFileName;
                        await _context.Photos.AddAsync(dbImage);
                        await this._context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
