using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Kentico.Xperience.Intercom
{
    /// <summary>
    /// Application startup extension methods.
    /// </summary>
    public static class StartupExtensions
    {
        /// <summary>
        /// Maps Intercom routes into the system.
        /// </summary>
        /// <param name="endpoints">Endpoints route builder.</param>
        public static void MapKenticoIntercomRoutes(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                name: "Kentico.Xperience.Intercom.UpdateContact",
                pattern: "kentico.xperience.intercom/updatecontact",
                defaults: new
                {
                    controller = "KenticoIntercom",
                    action = nameof(KenticoIntercomController.UpdateContact)
                });

            endpoints.MapControllerRoute(
                name: "Kentico.Xperience.Intercom.LogActivity",
                pattern: "kentico.xperience.intercom/logactivity",
                defaults: new
                {
                    controller = "KenticoIntercom",
                    action = nameof(KenticoIntercomController.LogActivity)
                });
        }
    }
}
