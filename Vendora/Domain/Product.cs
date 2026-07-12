namespace _product
{
    public class Product
    {
        public Ulid Id { get; set; }
        public Ulid StoreId { get; private set; }
        public Ulid SellerId { get; private set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public long Article { get; set; }
        
        // preview
        public string? PreviewUrl { get; set; }
        public bool IsDeleted { get; set; }
        public Product(Ulid storeId, Ulid sellerId, int categoryId, 
            string name, string? description, 
            decimal price, int quantity,
            string? previewUrl)
        {
            Id = Ulid.NewUlid();
            StoreId = storeId;
            SellerId = sellerId;
            CategoryId = categoryId;
            Name = name;
            Description = description;
            Price = price;
            Quantity = quantity;
            Article = 0;
            PreviewUrl = previewUrl;
            IsDeleted = false;
        }
    }
}