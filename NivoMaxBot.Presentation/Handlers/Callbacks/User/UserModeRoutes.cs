namespace NivoMaxBot.Presentation.Handlers.Callbacks.User
{
    public static class UserModeRoutes
    {
        // Cart
        public const string CartClear = "user:cart:clear";

        public const string CartAdd = "user:cart:add";
        public const string CartRemove = "user:cart:remove";
        public const string CartUpdate = "user:cart:update";
        public const string CartPage = "user:cart:page";
        public const string CartView = "user:cart:view";

        // Order
        public const string OrderCreate = "user:order:create";

        // Consultation
        public const string ConsultationCreate = "user:consultation:create";

        // Service section
        public const string ServiceSection = "user:service_section";
        public const string ServiceDepartment = "user:service_department";

        // Warranty
        public const string WarrantyCreate = "user:warranty_request:create";

        // Profile
        public const string Profile = "profile:main";

        public const string OrdersPage = "user:orders:page";
        public const string OrderView = "user:order:view";
        public const string Orders = "profile:orders";

        public const string Warranty = "profile:warranty";
        public const string WarrantyPage = "warranty:page";
        public const string WarrantyView = "warranty:view";
    }
}
