using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CaptionGenerator.EF.Data;
using FoodBank.CORE.Dtos;

public class FoodCategoryService : IFoodCategoryService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public FoodCategoryService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<FoodCategory> CreateFoodCategoryAsync(FoodCategoryDto foodCategoryDto)
    {
        var foodCategory = _mapper.Map<FoodCategory>(foodCategoryDto);
        _context.FoodCategories.Add(foodCategory);
        await _context.SaveChangesAsync();
        return foodCategory;
    }

    public async Task<IEnumerable<FoodCategory>> GetAllFoodCategoriesAsync()
    {
        return await _context.FoodCategories.Include(fc => fc.FoodItems).ToListAsync();
    }

    public async Task<FoodCategory> GetFoodCategoryByIdAsync(int id)
    {
        return await _context.FoodCategories.Include(fc => fc.FoodItems).FirstOrDefaultAsync(fc => fc.Id == id);
    }

    public async Task<FoodCategory> UpdateFoodCategoryAsync(int id, FoodCategoryDto foodCategoryDto)
    {
        var existingFoodCategory = await _context.FoodCategories.FindAsync(id);
        if (existingFoodCategory == null)
        {
            return null;
        }

        _mapper.Map(foodCategoryDto, existingFoodCategory);
        existingFoodCategory.UpdatedAt = DateTime.UtcNow;

        _context.FoodCategories.Update(existingFoodCategory);
        await _context.SaveChangesAsync();

        return existingFoodCategory;
    }

    public async Task<bool> DeleteFoodCategoryAsync(int id)
    {
        var foodCategory = await _context.FoodCategories.FindAsync(id);
        if (foodCategory == null)
        {
            return false;
        }

        _context.FoodCategories.Remove(foodCategory);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<FoodItem>> GetFoodItemsByCategoryIdAsync(int categoryId)
    {
        return await _context.FoodItems.Where(fi => fi.CategoryId == categoryId).ToListAsync();
    }
}
