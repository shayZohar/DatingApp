using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipleExtentions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value; //return the UniqueName property inside the token service
        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value); //return the NameId (id) property inside the token service
        }
    }
}