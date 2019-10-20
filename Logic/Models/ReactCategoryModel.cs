namespace Logic.Models
{
    public class ReactCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> Roles { get; set; }
    }
}