namespace Template.CommandServer.Jobs;

using BunnyTail.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    [ComponentRegistration(Lifetime.Transient, "Job$", Namespace = "Template.CommandServer.Jobs")]
    public static partial IServiceCollection AddJobs(this IServiceCollection services);
}
