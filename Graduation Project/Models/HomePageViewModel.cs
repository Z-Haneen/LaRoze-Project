namespace Graduation_Project.Models
{
    public class HomePageViewModel
    {
        public List<Category> Categories { get; set; }
        public List<Product> FeaturedProducts { get; set; }
        public List<Product> BestSellingProducts { get; set; }
        public List<Promotion> Promotions { get; set; }
    }
}