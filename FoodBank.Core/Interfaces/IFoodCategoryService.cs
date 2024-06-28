using FoodBank.CORE.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IFoodCategoryService
{
    // Create a new FoodCategory
    Task<FoodCategory> CreateFoodCategoryAsync(FoodCategoryDto foodCategoryDto);

    // Get all FoodCategories
    Task<IEnumerable<FoodCategory>> GetAllFoodCategoriesAsync();

    // Get a FoodCategory by ID
    Task<FoodCategory> GetFoodCategoryByIdAsync(int id);

    // Update an existing FoodCategory
    Task<FoodCategory> UpdateFoodCategoryAsync(int id, FoodCategoryDto foodCategoryDto);

    // Delete a FoodCategory by ID
    Task<bool> DeleteFoodCategoryAsync(int id);

    // Get all FoodItems for a specific FoodCategory
    Task<IEnumerable<FoodItem>> GetFoodItemsByCategoryIdAsync(int categoryId);
}
