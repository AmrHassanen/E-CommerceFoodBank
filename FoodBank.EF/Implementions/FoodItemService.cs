using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FoodBank.CORE.Dtos;
using FoodBank.CORE.Interfaces;
using CaptionGenerator.EF.Data;
using AutoMapper;

public class FoodItemService : IFoodItemService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public FoodItemService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<FoodItem> CreateFoodItemAsync(FoodItemDto foodItemDto)
    {
        var foodItem = _mapper.Map<FoodItem>(foodItemDto);
        _context.FoodItems.Add(foodItem);
        await _context.SaveChangesAsync();
        return foodItem;
    }

    public async Task<IEnumerable<FoodItem>> GetAllFoodItemsAsync()
    {
        return await _context.FoodItems.Include(fi => fi.Category).ToListAsync();
    }

    public async Task<FoodItem> GetFoodItemByIdAsync(int id)
    {
        return await _context.FoodItems.Include(fi => fi.Category).FirstOrDefaultAsync(fi => fi.Id == id);
    }

    public async Task<FoodItem> UpdateFoodItemAsync(int id, FoodItemDto foodItemDto)
    {
        var existingFoodItem = await _context.FoodItems.FindAsync(id);
        if (existingFoodItem == null)
        {
            return null;
        }

        _mapper.Map(foodItemDto, existingFoodItem);
        existingFoodItem.UpdatedAt = DateTime.UtcNow;

        _context.FoodItems.Update(existingFoodItem);
        await _context.SaveChangesAsync();

        return existingFoodItem;
    }

    public async Task<bool> DeleteFoodItemAsync(int id)
    {
        var foodItem = await _context.FoodItems.FindAsync(id);
        if (foodItem == null)
        {
            return false;
        }

        _context.FoodItems.Remove(foodItem);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<FoodItem>> GetFoodItemsByCategoryIdAsync(int categoryId)
    {
        return await _context.FoodItems.Where(fi => fi.CategoryId == categoryId).Include(fi => fi.Category).ToListAsync();
    }

    public async Task<IEnumerable<FoodItem>> GetAvailableFoodItemsAsync()
    {
        return await _context.FoodItems.Where(fi => fi.IsAvailable).Include(fi => fi.Category).ToListAsync();
    }
}


