using System.Threading.Tasks;

namespace Exas
{
    public interface IExasClient
    {
        string ExasBaseUrl { get; }

        Task<string> AddAssessmentProviderConfiguration(string tenantId);
        Task<string> CreateAndSendInvitation(string tenantId, string assessmentTestId, string sendInvitation);
        Task<string> ResultsPostCallback(string invitationId);
        Task<string> SaveAsessmentTestConfiguration(string tenantId, string id);
        Task<string> SaveAssessmentType();
        Task<string> SaveCustomer(string tenantId);
        Task<string> GetAllAssessmentTypeIdNames();
        Task<string> GetAllAssessmentTypes();
        Task<string> GetAssessmentConfigurations(string tenantId);
        Task<string> GetAssessmentProviderConfiguration(string tenantId, string assessmentTypeId);
        Task<string> GetAssessmentTest(string tenantId, string id);
        Task<string> GetAssessmentType(string id);
        Task<string> GetAssessmentTypeIdNames(string tenantId);
        Task<string> GetAssessmentTypes(string tenantId);
        Task<string> GetCustomer(string tenantId);
        Task<string> GetLatestStatusInvitationSentToCandidate(string tenantId, string invitationId);
        Task<string> GetResults(string invitationId);
        Task<string> ListAllAssessmentTests(string tenantId);
        Task<string> ListAllAssessmentTestsByTenantId(string tenantId, string assessmentTypeId);
        Task<string> ListAssessmentTests(string tenantId, string assessmentTypeId);
    }
}