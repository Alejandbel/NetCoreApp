namespace WebLab.Domain.Entities
{
	public class BeerType
	{
		public int Id { get; set; }

		public string Name { get; set; }
		public string NormalizedName { get; set; }

		public ICollection<Beer>? Beers { get; set; }
	}
}
