# CookingFinal_Remake

A web application for browsing, managing, and favoriting cooking recipes. Built with ASP.NET Core MVC, this app allows users to explore recipes by categories, view detailed instructions, and save their favorite recipes.

## Features

- **Recipe Browsing**: View a collection of recipes with details including ingredients, instructions, and categories.
- **User Authentication**: Register and login to access personalized features.
- **Favorites Management**: Save and manage favorite recipes.
- **Category Filtering**: Browse recipes by different categories.
- **Responsive Design**: Optimized for desktop and mobile devices.

## Technologies Used

- **Backend**:
  - .NET 10.0
  - ASP.NET Core MVC
  - C#

- **Database & Authentication**:
  - Supabase (PostgreSQL database, authentication, and real-time features)

- **Frontend**:
  - HTML5
  - CSS3
  - JavaScript
  - Bootstrap 5 (for responsive UI)
  - jQuery (for DOM manipulation and AJAX)

- **Other Libraries**:
  - DotNetEnv (for environment variable management)
  - Microsoft.AspNetCore (for web framework components)

## Prerequisites

- .NET 10.0 SDK
- Supabase account and project setup
- Environment variables configured (SUPABASE_URL and SUPABASE_KEY)

## Setup Instructions

1. Clone the repository:
   ```
   git clone <repository-url>
   cd CookingFinal_Remake
   ```

2. Set up environment variables:
   - Create a `.env` file in the root directory
   - Add your Supabase credentials:
     ```
     SUPABASE_URL=your_supabase_url
     SUPABASE_KEY=your_supabase_anon_key
     ```

3. Restore dependencies:
   ```
   dotnet restore
   ```

4. Run the application:
   ```
   dotnet run
   ```

5. Open your browser and navigate to `https://localhost:5001` (or the port specified in launchSettings.json)

## Project Structure

- **Controllers/**: Contains MVC controllers for handling requests
- **Models/**: Data models and view models
- **Views/**: Razor views for the UI
- **Services/**: Business logic services (RecipeService, UserService)
- **wwwroot/**: Static assets (CSS, JS, images)
- **supabase_tables.sql**: Database schema for Supabase

## Database Setup

Run the SQL scripts in `supabase_tables.sql` to set up the required tables in your Supabase project.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## License

This project is licensed under the MIT License.
