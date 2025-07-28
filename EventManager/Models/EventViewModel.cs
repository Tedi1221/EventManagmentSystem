public class EventViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; }
    public decimal Price { get; set; }
    public int MaxParticipants { get; set; }
    public int CategoryId { get; set; }
    public IFormFile? ImageFile { get; set; }
    public string? ImageUrl { get; set; }
}
