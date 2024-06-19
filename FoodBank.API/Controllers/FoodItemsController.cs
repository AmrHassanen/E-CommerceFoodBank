using Microsoft.AspNetCore.Mvc;
using FoodBank.CORE.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace FoodBank.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FoodItemsController : ControllerBase
    {
        private readonly IFoodItemService _foodItemService;
        private readonly IMapper _mapper;

        public FoodItemsController(IFoodItemService foodItemService, IMapper mapper)
        {
            _foodItemService = foodItemService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodItemDto>>> GetFoodItems()
        {
            var foodItems = await _foodItemService.GetAllFoodItemsAsync();
            var foodItemDtos = foodItems.Select(fi => _mapper.Map<FoodItemDto>(fi)).ToList();
            return Ok(foodItemDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FoodItemDto>> GetFoodItem(int id)
        {
            var foodItem = await _foodItemService.GetFoodItemByIdAsync(id);
            if (foodItem == null)
            {
                return NotFound();
            }
            var foodItemDto = _mapper.Map<FoodItemDto>(foodItem);
            return Ok(foodItemDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<FoodItemDto>> PostFoodItem(FoodItemDto foodItemDto)
        {
            // Create the food item using the service
            var foodItem = await _foodItemService.CreateFoodItemAsync(foodItemDto);

            // Generate the URI for the newly created food item
            var createdFoodItemDto = _mapper.Map<FoodItemDto>(foodItem);
            var uri = Url.Action(nameof(GetFoodItem), new { id = foodItem.Id });

            // Return the URI of the newly created resource in the Location header
            return Created(uri, createdFoodItemDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutFoodItem(int id, FoodItemDto foodItemDto)
        {
            var updatedFoodItem = await _foodItemService.UpdateFoodItemAsync(id, foodItemDto);
            if (updatedFoodItem == null)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFoodItem(int id)
        {
            var success = await _foodItemService.DeleteFoodItemAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<FoodItemDto>>> GetFoodItemsByCategoryId(int categoryId)
        {
            var foodItems = await _foodItemService.GetFoodItemsByCategoryIdAsync(categoryId);
            var foodItemDtos = foodItems.Select(fi => _mapper.Map<FoodItemDto>(fi)).ToList();
            return Ok(foodItemDtos);
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<FoodItemDto>>> GetAvailableFoodItems()
        {
            var foodItems = await _foodItemService.GetAvailableFoodItemsAsync();
            var foodItemDtos = foodItems.Select(fi => _mapper.Map<FoodItemDto>(fi)).ToList();
            return Ok(foodItemDtos);
        }
    }
}
