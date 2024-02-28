using Microsoft.AspNetCore.Mvc.Rendering;
using MyGardenWEB.Data;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace MyGardenWEB.Models
{
    public class PhotosVM
    {
        public PhotosVM()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        [Required]
        public int ProductsId { get; set; }
        public List<SelectListItem> Products { get; set; }
        [Required]
        public IFormFile Url { get; set; }
    }
}
