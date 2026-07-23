using Domain.Stores;
using Domain.CartItems;
using Domain.Users;
using Domain.OrderItems;
namespace Domain.Products
{
    public class Product
    {
        public Ulid Id { get; set; }

        // references
        public Ulid StoreId { get; private set; }
        public Store Store { get; set; } = null!;

        public List<CartItem> CartItems { get; private set; } = new();
        public List<OrderItem> OrderItems { get; private set; } = new();
    
        public enum ProductCategory
        {
            None = 0,                    // Не указано / Системное

            // --- 1-19: ЭЛЕКТРОНИКА И ЦИФРОВАЯ ТЕХНИКА ---
            Electronics = 1,             // Электроника (общее)
            Smartphones = 2,             // Смартфоны и гаджеты
            Laptops = 3,                 // Ноутбуки
            Computers = 4,               // Настольные ПК и моноблоки
            ComputerComponents = 5,      // Комплектующие для ПК
            AudioVideo = 6,              // Аудио и видеотехника
            PhotoVideoCameras = 7,       // Фото- и видеокамеры
            SmartHome = 8,               // Умный дом и безопасность
            OfficeEquipment = 9,         // Оргтехника и периферия
            GamingConsoles = 10,         // Игровые приставки и аксессуары

            // --- 20-39: ОДЕЖДА, ОБУВЬ И АКСЕССУАРЫ ---
            Clothing = 20,               // Одежда общего назначения
            MensClothing = 21,           // Мужская одежда
            WomensClothing = 22,         // Женская одежда
            Shoes = 23,                  // Ообувь общего назначения
            MensShoes = 24,              // Мужская обувь
            WomensShoes = 25,            // Женская обувь
            Accessories = 26,            // Аксессуары (сумки, ремни, кошельки)
            Jewelry = 27,                // Ювелирные изделия и бижутерия
            Watches = 28,                // Часы (наручные и умные)

            // --- 40-59: ДОМ, КУХНЯ И РЕМОНТ ---
            HomeAndKitchen = 40,         // Дом и кухня (общее)
            Furniture = 41,              // Мебель (диваны, шкафы, столы)
            LargeAppliances = 42,        // Крупная бытовая техника (холодильники, стиральные машины)
            SmallAppliances = 43,        // Мелкая бытовая техника (чайники, блендеры)
            HomeTextiles = 44,           // Домашний текстиль (постельное белье, шторы)
            Tableware = 45,              // Посуда и кухонная утварь
            Tools = 46,                  // Инструменты и садовая техника
            BuildingMaterials = 47,      // Строительные и отделочные материалы
            Lighting = 48,               // Освещение (люстры, светильники)

            // --- 60-69: КРАСОТА, ЗДОРОВЬЕ И УХОД ---
            BeautyAndHealth = 60,        // Красота и здоровье (общее)
            Cosmetics = 61,              // Декоративная и уходовая косметика
            Perfume = 62,                // Парфюмерия
            PersonalCareAppliances = 63, // Приборы для ухода (фены, бритвы)
            MedicalSupplies = 64,        // Медицинские товары и аптека
            VitaminsAndSupplements = 65, // Витамины и БАДы

            // --- 70-79: ДЕТСКИЕ ТОВАРЫ ---
            ChildrenProducts = 70,       // Детские товары (общее)
            Toys = 71,                   // Игрушки и настольные игры
            BabyClothing = 72,           // Одежда и обувь для малышей
            StrollersAndCarSeats = 73,   // Коляски и автокресла
            BabyFeeding = 74,            // Детское питание и кормление
            SchoolSupplies = 75,         // Школьные товары и канцелярия

            // --- 80-89: СПОРТ, ТУРИЗМ И ОТДЫХ ---
            SportsAndOutdoors = 80,      // Спорт и отдых (общее)
            FitnessEquipment = 81,       // Тренажеры и фитнес
            SportsClothingAndShoes = 82, // Спортивная одежда и обувь
            CampingGear = 83,            // Туризм, кемпинг, охота и рыбалка
            BicyclesAndTransport = 84,  // Велосипеды, самокаты, гироскутеры

            // --- 90-99: АВТОТОВАРЫ ---
            AutoSupplies = 90,           // Автотовары (общее)
            CarParts = 91,               // Автозапчасти и расходники
            CarElectronics = 92,         // Автоэлектроника (навигаторы, регистраторы)
            CarChemicalsAndOils = 93,    // Автохимия и масла
            TiresAndWheels = 94,         // Шины и диски

            // --- 100-119: КНИГИ, ХОББИ, КАНЦЕЛЯРИЯ ---
            Books = 100,                 // Печатные книги
            Stationery = 101,            // Канцелярия для офиса и дома
            ArtAndCrafts = 102,          // Товары для творчества и рукоделия
            MusicalInstruments = 103,    // Музыкальные инструменты

            // --- 120-129: ТОВАРЫ ДЛЯ ЖИВОТНЫХ ---
            PetSupplies = 120,           // Зоотовары (общее)
            PetFood = 121,               // Корм для животных
            PetAccessories = 122,        // Игрушки, лежанки, ошейники

            // --- 130-139: ПРОДУКТЫ ПИТАНИЯ ---
            Grocery = 130,               // Бакалея (чай, кофе, макароны)
            HealthyFood = 131,           // Диетическое и здоровое питание

            // --- 140-149: ЦИФРОВЫЕ ТОВАРЫ ---
            DigitalSoftware = 140,       // Программное обеспечение и ключи
            EBooks = 141,                // Электронные и аудиокниги
            DigitalCourses = 142,        // Онлайн-курсы и обучение

            // --- СЕРВИСНЫЕ И ПРОЧИЕ ---
            Services = 500,              // Услуги (доставка, сборка, настройка)
            Other = 999                  // Другое / Прочее
        }

        public ProductCategory Category { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ShortDescription { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public long Article { get; set; }
        
        // preview
        public string? PreviewUrl { get; set; }
        public bool IsDeleted { get; set; }
        private Product() { }
        internal Product(Ulid storeId, int categoryId, 
            string name, string? description, string? shortDescription,
            decimal price, int quantity,
            string? previewUrl)
        {
            Id = Ulid.NewUlid();
            StoreId = storeId;
            Category = (ProductCategory)categoryId;
            Name = name;
            ShortDescription = shortDescription;
            Description = description;
            Price = price;
            Quantity = quantity;
            Article = 0;
            PreviewUrl = previewUrl;
            IsDeleted = false;
        }
    }
}