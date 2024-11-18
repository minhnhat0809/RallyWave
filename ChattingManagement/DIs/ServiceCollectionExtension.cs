    using ChattingManagement.Repository;
    using ChattingManagement.Repository.Impl;
    using ChattingManagement.Service;
    using ChattingManagement.Ultility;

    namespace ChattingManagement.DIs;

    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            
            // Services
            services.AddScoped<IConservationService, ConservationService>();
            services.AddScoped<IMessageService, MessageService>();
            
            // Repositories
            services.AddScoped<IConservationRepo, ConservationRepo>();
            services.AddScoped<IMessageRepo, MessageRepo>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            

            //utilities
            services.AddScoped(typeof(Validate));
            services.AddScoped(typeof(ListExtensions));
            
            
            return services;
        }
    }