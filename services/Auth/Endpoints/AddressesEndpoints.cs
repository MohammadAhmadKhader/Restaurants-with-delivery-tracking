
using System.Security.Claims;
using Auth.Dtos.Address;
using Auth.Services.IServices;
using Auth.Utils;
using Shared.Utils;
using Auth.Mappers;
using Shared.Filters;

namespace Auth.Endpoints;

public static class AddressesEndpoints
{
    public static void MapAddressesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users/addresses").RequireAuthorization();

        group.MapGet("", async (int? page, int? size, IAddressesService addressesService, ClaimsPrincipal principal) =>
        {
            PaginationUtils.Normalize(ref page, ref size);
            var userId = SecurityUtils.ExtractUserId(principal);
            var (addresses, count) = await addressesService.FindAllByUserIdAsync(userId, page!.Value, size!.Value);
            var addressesView = addresses.Select(a => a.ToViewDto()).ToList();

            return Results.Ok(PaginationUtils.ResultOf(addressesView, count, page, size));
        });

        group.MapPost("", async (IAddressesService addressesService, AddressCreateDto dto, ClaimsPrincipal principal) =>
        {
            var userId = SecurityUtils.ExtractUserId(principal);
            var address = await addressesService.CreateAsync(userId, dto);
            var addressView = address.ToViewDto();

            return Results.Ok(new { address = addressView });
        }).AddEndpointFilter<ValidationFilter<AddressCreateDto>>();

        group.MapPut("/{id}", async (Guid id, IAddressesService addressesService, AddressUpdateDto dto, ClaimsPrincipal principal) =>
        {
            var userId = SecurityUtils.ExtractUserId(principal);
            var address = await addressesService.UpdateAsync(id, userId, dto);

            return Results.NoContent();
        }).AddEndpointFilter<ValidationFilter<AddressUpdateDto>>();
        
        group.MapDelete("/{id}", async (Guid id, IAddressesService addressesService, ClaimsPrincipal principal) =>
        {
            var userId = SecurityUtils.ExtractUserId(principal);
            await addressesService.DeleteByIdAsync(id, userId);

            return Results.NoContent();
        });
    }
}