
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // Add this to use TextMeshPro

public class FoodStackManager : MonoBehaviour
{
    public GameObject pizzaPrefab;
    public GameObject friedChickenPrefab;
    public GameObject sushiPrefab;
    public GameObject hamburgerPrefab;
    public GameObject donutPrefab;
    public Transform stackPoint;
    public int maxStackSize = 20;

    private List<GameObject> foodStack = new List<GameObject>();
    private bool hasCollectedFood = false;
    private float foodPrefabHeight;

    private enum FoodType { None, Pizza, FriedChicken, Sushi,Hamburger,Donut };
    private FoodType currentFoodType = FoodType.None;

    public TextMeshProUGUI dollarText;  // TextMeshPro field for the dollar count
    private int dollarCount = 0;  // Variable to track the current dollar count

    private TimerBarController timerBarController; // Reference to TimerBarController

    void Start()
    {
        UpdateFoodPrefabHeight(pizzaPrefab);
        UpdateDollarText();  // Initialize dollar count on the UI
        timerBarController = FindObjectOfType<TimerBarController>(); // Get TimerBarController instance
    }

    void UpdateFoodPrefabHeight(GameObject foodPrefab)
    {
        Renderer foodRenderer = foodPrefab.GetComponent<Renderer>();
        if (foodRenderer != null)
        {
            foodPrefabHeight = foodRenderer.bounds.size.y;
        }
        else
        {
            Debug.LogWarning("The foodPrefab does not have a Renderer component.");
            foodPrefabHeight = 0.2f;  // Default height
        }
    }

    public void CollectFood(string restaurantTag)
    {
        if (!hasCollectedFood && foodStack.Count == 0)
        {
            GameObject foodPrefab = null;

            switch (restaurantTag)
            {
                case "PizzaRestaurant":
                    foodPrefab = pizzaPrefab;
                    currentFoodType = FoodType.Pizza;
                    break;
                case "FriedChickenRestaurant":
                    foodPrefab = friedChickenPrefab;
                    currentFoodType = FoodType.FriedChicken;
                    break;
                case "SushiRestaurant":
                    foodPrefab = sushiPrefab;
                    currentFoodType = FoodType.Sushi;
                    break;
                case "Bakery":
                    foodPrefab = hamburgerPrefab;
                    currentFoodType = FoodType.Hamburger;
                    break;
                case "FastFood":
                    foodPrefab = donutPrefab;
                    currentFoodType = FoodType.Donut;
                    break;
                default:
                    Debug.LogWarning("Unknown restaurant tag: " + restaurantTag);
                    return;
            }

            UpdateFoodPrefabHeight(foodPrefab);

            for (int i = 0; i < maxStackSize; i++)
            {
                GameObject newFood = Instantiate(foodPrefab, stackPoint);
                Vector3 newPosition = new Vector3(0, i * foodPrefabHeight, 0);
                newFood.transform.localPosition = newPosition;
                foodStack.Add(newFood);
            }

            hasCollectedFood = true;
            timerBarController.StartTimer(); // Start the timer when food is collected
        }
    }

    public void StartFoodDelivery()
    {
        if (hasCollectedFood && foodStack.Count > 0)
        {
            StartCoroutine(DeliverFood());
        }
    }

    IEnumerator DeliverFood()
    {
        if (foodStack.Count > 0)
        {
            GameObject topFood = foodStack[foodStack.Count - 1];
            foodStack.RemoveAt(foodStack.Count - 1);
            yield return StartCoroutine(AnimateFoodFall(topFood));
            Destroy(topFood);

            // Increase the dollar count by 5 each time food is delivered
            dollarCount += 5;
            UpdateDollarText();  // Update the UI text with the new dollar count

            if (foodStack.Count == 0)
            {
                hasCollectedFood = false;
                currentFoodType = FoodType.None;
                timerBarController.StopTimer(); // Stop the timer if all food is delivered
            }

            Debug.Log("Delivered a " + currentFoodType.ToString());
        }
    }

    void UpdateDollarText()
    {
        dollarText.text = "$" + dollarCount.ToString();  // Update the TextMeshPro text
    }

    IEnumerator AnimateFoodFall(GameObject food)
    {
        Vector3 startPosition = food.transform.localPosition;
        Vector3 endPosition = new Vector3(startPosition.x, 0, startPosition.z + 1f);
        float jumpHeight = 2f;
        float fallDuration = 0.5f;

        float elapsedTime = 0f;

        while (elapsedTime < fallDuration)
        {
            float t = elapsedTime / fallDuration;
            float yPosition = Mathf.Lerp(startPosition.y, endPosition.y, t) + Mathf.Sin(t * Mathf.PI) * jumpHeight;
            food.transform.localPosition = new Vector3(
                Mathf.Lerp(startPosition.x, endPosition.x, t),
                yPosition,
                Mathf.Lerp(startPosition.z, endPosition.z, t)
            );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        food.transform.localPosition = endPosition;
        yield return new WaitForSeconds(0.1f);
    }

    public bool IsCarryingStack()
    {
        return foodStack.Count > 0;
    }

    public void ApplyWobbleEffect(float wobbleAmount, float wobbleSpeed, float wobbleTimeOffset)
    {
        if (foodStack.Count > 0)
        {
            for (int i = 0; i < foodStack.Count; i++)
            {
                float wobbleAngle = Mathf.Sin(Time.time * wobbleSpeed + wobbleTimeOffset + i * 0.1f) * wobbleAmount;
                foodStack[i].transform.localRotation = Quaternion.Euler(wobbleAngle, 0, wobbleAngle);
            }
        }
    }
}
