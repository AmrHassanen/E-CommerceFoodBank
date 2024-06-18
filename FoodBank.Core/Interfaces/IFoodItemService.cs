using FoodBank.CORE.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IFoodItemService
{
    // Create a new FoodItem
    Task<FoodItem> CreateFoodItemAsync(FoodItemDto foodItem);

    // Get all FoodItems
    Task<IEnumerable<FoodItem>> GetAllFoodItemsAsync();

    // Get a FoodItem by ID
    Task<FoodItem> GetFoodItemByIdAsync(int id);

    // Update an existing FoodItem
    Task<FoodItem> UpdateFoodItemAsync(int id, FoodItemDto foodItem);

    // Delete a FoodItem by ID
    Task<bool> DeleteFoodItemAsync(int id);

    // Get FoodItems by Category ID
    Task<IEnumerable<FoodItem>> GetFoodItemsByCategoryIdAsync(int categoryId);

    // Get available FoodItems
    Task<IEnumerable<FoodItem>> GetAvailableFoodItemsAsync();
}
