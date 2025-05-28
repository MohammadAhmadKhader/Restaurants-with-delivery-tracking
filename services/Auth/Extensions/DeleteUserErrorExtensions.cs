using Auth.Models;

namespace Auth.Extensions;

public static class DeleteUserErrorExtensions
{
    public static string GetMessage(this DeleteUserError error) =>
        error switch
        {
            DeleteUserError.None            => "",
            DeleteUserError.NotFound        => "User was not found",
            DeleteUserError.ForbiddenOwner  => "Cannot delete the owner account",
            DeleteUserError.ForbiddenAdmin  => "Cannot delete an Admin, you must remove their Admin role first",
            DeleteUserError.Unexpected      => "An unexpected error occurred while deleting the user",
            _                               => "An unknown error occurred"
        };

    public static int GetStatusCode(this DeleteUserError error) =>
        error switch
        {
            DeleteUserError.None            => 204,
            DeleteUserError.NotFound        => 404,
            DeleteUserError.ForbiddenOwner  => 403,
            DeleteUserError.ForbiddenAdmin  => 403,
            DeleteUserError.Unexpected      => 500,
            _                                => 500
        };
}