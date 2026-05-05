# Project Context
Backend for a medical/psychological platform (PsychoSupCenterBackend)[cite: 1].
Stack: C#, .NET 10.0, EF Core 10.0.6, MediatR 14.1.0, FluentValidation, ASP.NET Core SignalR[cite: 1].
Architecture: Clean Architecture (API, Application, Domain, Persistence/Infrastructure)[cite: 1].

# AI Coding Guidelines

## Architecture & CQRS
1. **Strict Layer Separation:** The Application layer must not depend on `DbContext`. Data access is strictly via `IUnitOfWork` and `IRepository<T>`[cite: 1].
2. **CQRS Pattern:** Operations are strictly separated into Commands (state changes) and Queries (reads)[cite: 1]. 
3. **Encapsulation:** Each feature is a single static class with nested types[cite: 1]:
   * `Command` or `Query` (`sealed record`) implementing `ICommand<Result<T>>` or `IQuery<Result<T>>`[cite: 1].
   * `Validator` inheriting `AbstractValidator<Command>`[cite: 1].
   * `Handler` implementing `IRequestHandler<Command, Result<T>>` using primary constructors for dependencies[cite: 1].

## Data Access (IUnitOfWork)
* Never inject `DbContext` directly[cite: 1].
* Use repositories: `await unitOfWork.Appointments.GetByIdAsync(id, cancellationToken);`[cite: 1].
* For querying lists, use `unitOfWork.EntityName.Query().Include(...)` or `unitOfWork.EntityName.FindAsync(...)`[cite: 1].

## Responses & DTOs
* **Result Pattern:** All handlers must return `Result<T>` (`Result<T>.Success(...)` or `Result<T>.Failure(...)`)[cite: 1].
* **DTOs:** Declare all DTOs as `sealed record`[cite: 1] and place them in the `DTOs` subfolder of the respective feature domain[cite: 1].

## Validation
* Keep validation logic out of the Handler. Use the nested `Validator` class for data validation rules[cite: 1]. It will be processed automatically via MediatR's `ValidationBehavior`[cite: 1].

## SignalR
* Chat functionality uses `ChatHub.cs`[cite: 1]. SignalR actions must invoke MediatR Commands first to persist state in the DB, then broadcast the result to clients via `Clients.Group(...)`[cite: 1].

## Code Style
* Write clean, organic code utilizing modern C# features (primary constructors, collection expressions `[]`, target-typed `new()`).
* Code must be self-documenting. Avoid redundant comments.
* Always use file-scoped namespaces[cite: 1].