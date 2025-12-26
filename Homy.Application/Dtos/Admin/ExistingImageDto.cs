namespace Homy.Application.Dtos.Admin
{
    public class ExistingImageDto
    {
        public long Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }
    }
}
