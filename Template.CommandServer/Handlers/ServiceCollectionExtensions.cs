namespace Template.CommandServer.Handlers;

using Template.CommandServer.Handlers.Commands;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        services.AddSingleton<ICommand, ExitCommand>();
        services.AddSingleton<ICommand, HealthCommand>();
        services.AddSingleton<ICommand, SetCommand>();
        services.AddSingleton<ICommand, GetCommand>();
        return services;
    }
}
