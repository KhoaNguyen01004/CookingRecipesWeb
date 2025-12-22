# Data Flow Diagrams for Cooking Recipes Web Application

| ID | Description | Related Requirements |
|----|-------------|----------------------|
| DFD-001 | The context diagram shows the overall system boundaries, with external entities (User, MealDB API, Supabase Database) interacting with the Cooking Recipes Web Application. Data flows include user requests, API responses, and database operations. | System integration, external API connectivity, database interactions |
| DFD-002 | This diagram details the authentication subsystem, showing data flows for login and registration. It includes user input, validation, Supabase authentication, and session creation. | User authentication, session management |
| DFD-003 | Illustrates the recipe management subsystem, including browsing, searching, and viewing details. Data flows from user requests through the RecipeService to the MealDB API and back to the user interface. | Recipe display, search functionality, recipe information display |
| DFD-004 | Shows the favorites subsystem, detailing how users add/remove favorites and view their list. Data flows between the user interface, HomeController, UserService, and Supabase database. | User personalization, favorites management, favorites display |
| DFD-005 | Depicts the category browsing subsystem, showing how categories are fetched and displayed, with navigation to filtered recipes. | Category display, recipe display |
| DFD-006 | Illustrates the caching mechanism for improved performance, showing how frequently accessed data (recipes, categories) is cached using IMemoryCache. | Response time, caching |
| DFD-007 | Breaks down the RecipeService into detailed data flows, showing interactions with the MealDB API for different operations (search, random, categories, filter). | Recipe display, search functionality, category display, random selection |
| DFD-008 | Details the UserService interactions with Supabase database for user management and favorites operations. | User authentication, user personalization, favorites management |
| DFD-009 | Shows error handling flows throughout the application, including API failures, database errors, and user input validation. | Error handling, system stability |
| DFD-010 | Illustrates session management and security measures, including authentication checks and secure data handling. | Data protection, authentication |

### Detailed Explanations

**DFD-001: Context Diagram**  
This diagram provides an overview of the entire system, showing how the Cooking Recipes Web Application interacts with external entities. Users send requests through the web interface, which processes them via controllers and services. Data flows to the MealDB API for recipe information and to Supabase for user data and favorites. Responses flow back through the same channels to display results to users.

**DFD-002: Level 0 DFD - User Authentication Process**  
Focuses on the authentication flow at a high level. User input (login/registration forms) is processed by AccountController, which validates data and interacts with Supabase for authentication. Successful authentication creates sessions, while failures trigger error responses. This diagram shows data transformation from user input to secure session data.

**DFD-003: Level 0 DFD - Recipe Browsing and Search**  
Details how recipe data flows through the system. User requests for browsing or searching are handled by HomeController, which calls RecipeService. The service queries MealDB API, processes responses, and returns formatted recipe data. Caching is shown as an intermediate store to improve performance for repeated requests.

**DFD-004: Level 0 DFD - Favorites Management**  
Illustrates the favorites functionality data flow. Authenticated users' favorite actions (add/remove/view) are processed by HomeController and UserService. Data is stored/retrieved from Supabase database, with validation checks for user sessions and recipe existence. AJAX responses provide immediate feedback without full page reloads.

**DFD-005: Level 0 DFD - Category Management**  
Shows category-related data flows. Categories are fetched from MealDB API via RecipeService and cached for performance. User selections trigger filtered recipe requests, demonstrating how category data influences recipe display logic.

**DFD-006: Level 0 DFD - Caching and Performance**  
Highlights the caching subsystem. Frequently accessed data (recipes, categories) is stored in IMemoryCache after initial API calls. Subsequent requests check cache first, reducing API load and improving response times. Cache expiration and refresh mechanisms are indicated.

**DFD-007: Level 1 DFD - Recipe Service Interactions**  
Breaks down RecipeService operations. Shows detailed API interactions for different endpoints (search, random, categories, filter). Data parsing, error handling, and response formatting are depicted, including how API responses are transformed into application models.

**DFD-008: Level 1 DFD - User Service Database Operations**  
Details UserService database interactions. Shows CRUD operations for users and favorites in Supabase. Query construction, execution, and result processing are illustrated, including error handling for database connection issues.

**DFD-009: Level 1 DFD - Error Handling and Logging**  
Demonstrates error flow throughout the system. API failures, database errors, and validation issues are caught at various points (controllers, services). Error data is logged and user-friendly messages are generated, with graceful degradation to prevent system crashes.

**DFD-010: Level 1 DFD - Session and Security Management**  
Illustrates security-related data flows. Session creation/validation, authentication checks, and secure data transmission are shown. Supabase authentication integration and HTTPS usage are highlighted, with data protection measures for sensitive operations.
