using Restaurants.Contracts.Dtos.Menu;
using Restaurants.Contracts.Dtos.MenuItems;
using Restaurants.Data;
using Restaurants.Mappers;
using Restaurants.Models;
using Restaurants.Repositories.IRepositories;
using Restaurants.Services.IServices;
using Shared.Data.Patterns.UnitOfWork;
using Shared.Exceptions;

namespace Restaurants.Services;

public class MenusService(
    IUnitOfWork<AppDbContext> unitOfWork,
    IMenusRepository menusRepository,
    IMenuItemsRepository menuItemsRepository
    ) : IMenusService
{
    private readonly IUnitOfWork<AppDbContext> _unitOfWork = unitOfWork;
    private readonly IMenusRepository _menusRepository = menusRepository;
    private readonly IMenuItemsRepository _menuItemsRepository = menuItemsRepository;
    private const string _menuResourceName = "menu";
    private const string _itemResourceName = "item";

    public async Task<(List<Menu> menus, int count)> FindAllAsync(int page, int size)
    {
        return await _menusRepository.FindAllOrderedDescAtAsync(page, size);
    }

    public async Task<Menu?> FindByIdWithItemsAsync(int menuId)
    {
        return await _menusRepository.FindByIdWithItemsAsync(menuId);
    }

    public async Task<Menu> AddItemsToMenuAsync(int menuId, MenuAddItemsDto dto)
    {
        var menu = await _menusRepository.FindByIdWithItemsAsync(menuId);
        ResourceNotFoundException.ThrowIfNull(menu, _menuResourceName);

        var existingItemIds = menu.Items.Select(item => item.Id).ToHashSet();
        foreach (var newItemId in dto.MenuItemsIds)
        {
            if (existingItemIds.Contains(newItemId))
            {
                var conflictingItem = menu.Items.First(item => item.Id == newItemId);
                throw new ConflictException($"Item '{conflictingItem.Name}' is already added", ConflictType.Duplicate);
            }
        }

        var items = await _menuItemsRepository.FindManyByIdsAsync(dto.MenuItemsIds);
        if (items.Count != dto.MenuItemsIds.Count)
        {
            var foundItemIds = items.Select(item => item.Id).ToHashSet();
            var notFoundItemIds = dto.MenuItemsIds.Where(id => !foundItemIds.Contains(id)).ToList();
            throw new ResourceNotFoundException("menu", notFoundItemIds.FirstOrDefault());
        }

        foreach (var item in items)
        {
            menu.Items.Add(item);
        }

        await _unitOfWork.SaveChangesAsync();

        return menu;
    }

    public async Task RemoveItemFromMenuAsync(int menuId, int menuItemId)
    {
        var menu = await _menusRepository.FindByIdWithItemsAsync(menuId);
        ResourceNotFoundException.ThrowIfNull(menu, _menuResourceName);

        var itemToRemove = menu.Items.Where(x => x.Id == menuItemId).FirstOrDefault();
        if (itemToRemove == null)
        {
            throw new ConflictException("item", $"id: {menuItemId}", ConflictType.NotAssigned);
        }

        menu.Items.Remove(itemToRemove);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<Menu> CreateAsync(MenuCreateDto dto)
    {
        var duplicatedMenu = await _menusRepository.FindByNameAsync(dto.Name);
        if (duplicatedMenu != null)
        {
            throw new ConflictException($"menu with name: '{dto.Name}' already exists", ConflictType.Duplicate);
        }

        var newMenu = await _menusRepository.CreateAsync(dto.ToModel());
        await _unitOfWork.SaveChangesAsync();

        return newMenu;
    }

    public async Task<Menu> UpdateAsync(int id, MenuUpdateDto dto)
    {
        var menu = await _menusRepository.FindByIdAsync(id);
        ResourceNotFoundException.ThrowIfNull(menu, _menuResourceName);

        dto.PatchModel(menu);
        await _unitOfWork.SaveChangesAsync();

        return menu;
    }

    public async Task DeleteAsync(int id)
    {
        var isFound = await _menusRepository.DeleteAsync(id);
        ResourceNotFoundException.ThrowIfTrue(!isFound, _menuResourceName);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<MenuItem> CreateItemAsync(MenuItemCreateDto dto)
    {
        var item = await _menuItemsRepository.CreateAsync(dto.ToModel());
        await _unitOfWork.SaveChangesAsync();

        return item;
    }

    public async Task<MenuItem> UpdateItemAsync(int itemId, MenuItemUpdateDto dto)
    {
        var menuItem = await _menuItemsRepository.FindByIdAsync(itemId);
        ResourceNotFoundException.ThrowIfNull(menuItem, _itemResourceName);
        
        dto.PatchModel(menuItem);
        await _unitOfWork.SaveChangesAsync();
    
        return menuItem;
    }

    public async Task DeleteItemAsync(int itemId)
    {
        await _menuItemsRepository.DeleteAsync(itemId);
        await _unitOfWork.SaveChangesAsync();
    }
}