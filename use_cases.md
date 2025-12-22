# Use Cases for Cooking Recipes Web Application

## Functional Use Cases

| ID | Description | Related Requirements |
|----|-------------|----------------------|
| UC-001 | Allows new users to create an account by providing first name, last name, email, and password. The system validates the input, creates a user profile in the database, and logs the user in automatically. | User authentication, data validation |
| UC-002 | Enables existing users to log in using email and password. The system authenticates credentials, sets session data, and redirects to the home page. | User authentication, session management |
| UC-003 | Displays a list of recipes, either random or filtered by category. Users can view initial recipes and load more as needed. | Recipe display, pagination |
| UC-004 | Shows detailed information about a specific recipe, including ingredients, instructions, and category. | Recipe information display |
| UC-005 | Allows users to search for recipes by name or keywords, displaying matching results. | Search functionality |
| UC-006 | Lists all available recipe categories for browsing. | Category display |
| UC-007 | Enables logged-in users to add or remove recipes from their favorites list. | User personalization, favorites management |
| UC-008 | Displays the user's favorite recipes in a dedicated page. | User personalization, favorites display |
| UC-009 | Loads additional recipes for pagination in category views. | Pagination, performance |
| UC-010 | Redirects to a random recipe's detail page. | Random selection |

### Detailed Explanations

**UC-001: User Registration**  
This use case covers the initial user onboarding process. Users fill out a registration form with personal details. The system performs validation (e.g., email uniqueness, password strength). Upon successful registration, a user record is inserted into the Supabase database, and the user is automatically authenticated with a session created. This enables immediate access to personalized features like favorites.

**UC-002: User Login**  
Existing users access the login form to enter their credentials. The system verifies the email and password against the Supabase database. If valid, a session is established with user details stored, allowing access to protected features. Invalid attempts display error messages without revealing sensitive information.

**UC-003: Browse Recipes**  
The home page shows recipes based on user selection. Random recipes are fetched initially, or category-specific recipes if a category is chosen. Pagination allows loading additional recipes via AJAX to improve performance. Caching is used to store results temporarily.

**UC-004: View Recipe Details**  
Users click on a recipe to view its full details. The system fetches the recipe data from the MealDB API using the recipe ID. Information includes ingredients list, step-by-step instructions, preparation time, servings, and category. This page also includes favorite toggle for logged-in users.

**UC-005: Search Recipes**  
Users enter search terms in the search bar. The system queries the MealDB API for recipes matching the search criteria. Results are displayed in a list format similar to the browse view. If no results are found, a message is shown.

**UC-006: View Categories**  
The categories page displays all meal categories from the MealDB API. Each category is shown as a clickable item that filters recipes when selected. This provides an organized way for users to explore recipes by type (e.g., Italian, Chinese, Dessert).

**UC-007: Add/Remove Favorite**  
Authenticated users can mark recipes as favorites from the recipe details page. The system checks if the recipe is already favorited and toggles the state. Favorites are stored in the Supabase database linked to the user ID. AJAX is used for seamless interaction without page reload.

**UC-008: View My Favorites**  
Logged-in users access their personalized favorites page. The system retrieves all favorited recipes for the user from the database, fetches full recipe details from the API, and displays them in a list. If no favorites exist, an appropriate message is shown.

**UC-009: Load More Recipes**  
When viewing recipes by category, users can load more recipes beyond the initial set. This AJAX endpoint fetches the next batch of recipes for the specified category, maintaining performance by loading data incrementally.

**UC-010: Get Random Dish**  
Users can discover new recipes by clicking a "Random Dish" button. The system fetches a random recipe from the MealDB API and redirects to its detail page, providing an element of surprise and exploration.

## Non-Functional Use Cases

| ID | Description | Related Requirements |
|----|-------------|----------------------|
| NF-001 | The application should respond to user requests within 2 seconds for most operations, with caching implemented for frequently accessed data. | Response time, caching |
| NF-002 | User authentication and data protection using Supabase, with secure password handling and session management. | Data protection, authentication |
| NF-003 | Responsive design optimized for desktop and mobile, with intuitive navigation and clear error messages. | Responsive UI, user experience |
| NF-004 | Robust error handling for API failures, database issues, and invalid inputs, with graceful degradation. | Error handling, system stability |

### Detailed Explanations

**NF-001: Performance**  
Performance is critical for user satisfaction. The system implements in-memory caching for recipes and categories to reduce API calls. Response times are monitored, with most operations completing within 2 seconds. This includes page loads, searches, and data retrieval.

**NF-002: Security**  
Security ensures user data protection and system integrity. Supabase handles authentication with secure password hashing. Sessions are managed server-side with proper expiration. All data transmission uses HTTPS, and sensitive operations require authentication.

**NF-003: Usability**  
The UI adapts to different screen sizes using Bootstrap. Navigation is intuitive with clear menus and breadcrumbs. Error messages are user-friendly and provide guidance. The design follows accessibility principles for broader usability.

**NF-004: Reliability**  
The system handles failures gracefully without crashing. API timeouts and database connection issues are caught and user-friendly messages displayed. Invalid inputs are validated client and server-side. Logging helps with troubleshooting.
