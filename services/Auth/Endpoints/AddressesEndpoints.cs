
using System.Security.Claims;
using Auth.Contracts.Dtos.Address;
using Auth.Services.IServices;
using Auth.Utils;
using Shared.Utils;
using Auth.Mappers;
using Shared.Filters;
using Shared.Contracts.Dtos;

namespace Auth.Endpoints;

public static class AddressesEndpoints
{
    public static void MapAddressesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users/addresses").RequireAuthorization();

        group.MapGet("", async ([AsParameters] PagedRequest pagedReq, IAddressesService addressesService, ClaimsPrincipal principal) =>
        {
            PaginationUtils.Normalize(pagedReq);
            var page = pagedReq.Page!.Value;
            var size = pagedReq.Size!.Value;

            var userId = SecurityUtils.ExtractUserId(principal);
            
            var (addresses, count) = await addressesService.FindAllByUserIdAsync(userId, page, size);
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