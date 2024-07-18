# IMDB REST API in ASP.NET Core

This project is an IMDB REST API built using ASP.NET Core and abiding by Clean Code, SOLID, KISS, YAGNI.

## Table of Contents

- [Introduction](#introduction)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Getting Started](#getting-started)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Introduction

This project adapts the best practices used in building a RESTful API using ASP.NET Core. The API provides CRUD operations for managing movies and includes features like authentication, authorization, pagination, filtering, and more.

## Features

- Minimal API version of Web API
- CRUD operations for manipulating movies, ratings and genres
- JWT authentication and authorization
- Rating system for movies
- Pagination, filtering, and sorting
- API versioning (not used right now)
- Swagger integration
- Caching mechanisms (current: Response, future: Output)
- HATEOAS support (not used right now)
- SDK creation for the API

## Technologies Used

- ASP.NET Core
- PostgreSQL database
- Dapper
- Npgsql
- AutoMapper
- FluentValidation
- Swashbuckle
- Asp.Versioning
- JWT, x-api-key for authentication
- Swagger for API documentation
- Refit for creating the SDK
- Docker for containerization

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (version 6.0 or higher)
- [C# 12](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-12) (version 12 or higher)
- [PostgreSQL](https://www.postgresql.org/)

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/Margo17/IMDB-be.git
   cd IMDB-be
   ```

2. Install the dependencies:

   ```bash
   dotnet restore
   ```

3. Update the database connection string in `appsettings.json`:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=your_host;Database=your_db;Username=your_user;Password=your_password"
   }
   ```

4. Start the API:

   ```bash
   dotnet run
   ```

### Running Tests

To run the tests, use the following command:

```bash
dotnet test
```

## Usage

### Endpoints

_Movies resource_
- **GET /api/movies**: Retrieve all movies with pagination, filtering, and sorting.
- **GET /api/movies/{idOrSlug}**: Retrieve a movie by ID or Slug.
- **POST /api/movies**: Create a new movie.
- **PUT /api/movies/{id}**: Update a movie by ID.
- **DELETE /api/movies/{id}**: Delete a movie by ID.

_Ratings resource_
- **GET /api/ratings/me**: Retrieve user ratings.
- **PUT /api/movies/{id}/ratings**: Update a movie rating.
- **DELETE /api/movies/{id}/ratings**: Delete a movie by ID.

_Health check_
- **GET /_health**: Check api status

### Authentication

Use the `/api/auth/login` endpoint to authenticate and receive a JWT token. Include this token in the `Authorization` header as `Bearer {token}` for authenticated endpoints.

### Swagger

Access the Swagger UI at `https://localhost:5001/swagger` to explore and test the API endpoints.

## Contributing

Contributions are not welcome at this time, sorry! Still ensuring that the codebase adheres to the project's coding standards and includes relevant tests.

## License

This project is licensed under the MIT License. See the [LICENSE](https://opensource.org/license/mit) file for details.
