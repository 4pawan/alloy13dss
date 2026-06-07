using Microsoft.Extensions.DependencyInjection;

namespace alloy13dss.Business.Forms;

public static class DummyServiceCollectionExtensions
{
    public static IServiceCollection AddDummyForms(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<IDummyFormJourneyStateRepository, DummyFormJourneyStateRepository>();
        services.AddSingleton<IDummyFormBranchEvaluator, DummyFormBranchEvaluator>();
        services.AddTransient<IDummyFormSubmissionAnswerStore, DummyFormSubmissionAnswerStore>();
        services.AddTransient<IDummyFormJourneyService, DummyFormJourneyService>();
        services.AddTransient<IDummyToolSettingsResolver, DummyToolSettingsResolver>();
        services.AddTransient<IDummyFormToolResolver, DummyFormToolResolver>();
        services.AddHttpClient<IRecaptchaV3Verifier, RecaptchaV3Verifier>();

        return services;
    }
}
