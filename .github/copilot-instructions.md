---
description: 'Blazor component and application patterns'
applyTo: '**/*.razor, **/*.razor.cs, **/*.razor.css, **/*.cs'
---

# Copilot instructions for MyHomeCatalogus

This repository enforces project coding, formatting, and architecture preferences. When suggesting code, tests, or edits, follow these rules exactly.

## Project Context
- **Target Framework:** .NET 8 (Default for current project). Use the latest stable .NET/C# features only for new solutions.
- **Architecture:** Blazor (Server interactive components) and WebAssembly projects.
- **Patterns:** Prefer idiomatic Blazor patterns (Components, Dependency Injection, AuthenticationState, RenderModes).

## Formatting & Editor Settings (Authoritative)
- **Indentation:** Use **Tabs** for indentation.
  - Tab width: `4` for code (C#, Razor, HTML, CSS).
  - Tab width: `2` for project/manifest files (.csproj, .json, .xml).
- **Line Endings:** CRLF. Ensure files end with a single final newline.
- **Razor Files:** Use tabs. Single-line attributes per tag are allowed. `razor_indent_webgrid_col_definitions = true`.

## C# Style & Naming
- **Naming:** - Interfaces must begin with `I` (e.g., `IUserService`) and use PascalCase.
  - Types (classes, structs, enums) use PascalCase.
  - Private fields and local variables use camelCase.
- **Logic & Syntax:**
  - **Expression-bodied members:** Disabled. Use full block bodies for methods and constructors.
  - **Method Parameters:** Keep to a minimum (max 4). If more are needed, encapsulate them in a class/record.
  - **Using Directives:** Place outside the namespace. Use simple `using var` statements where applicable.
  - **Var Guideline:** Prefer `var` for built-in types and when the type is apparent; otherwise, be explicit.
  - **Modern Features:** Prefer null-propagation, coalesce expressions, and object/collection initializers.

## Blazor & Component Guidelines
- **Rendering:** Be explicit with `@rendermode` for server interactive components (e.g., `@rendermode InteractiveServer`).
- **Lifecycle:** Utilize built-in features (`OnInitializedAsync`, `OnParametersSetAsync`) effectively.
- **Performance:** - Optimize via `StateHasChanged()` efficiency and `ShouldRender()` where appropriate.
  - Use `EventCallbacks` for interactions, passing minimal data.
  - Minimize the render tree to avoid unnecessary re-renders.
- **Concurrency:** Always prefer fully async flows. **Never** block the UI thread with `.Result` or `.GetAwaiter().GetResult()`.

## State Management & Caching
- **State:** Use Cascading Parameters/EventCallbacks for simple state. For complex logic, use the StateContainer pattern or libraries like `Fluxor`.
- **Caching:** - **Server:** Use `IMemoryCache` for lightweight data.
  - **WASM:** Use `localStorage` or `sessionStorage` (e.g., via `Blazored.LocalStorage`).
  - **API:** Cache responses to avoid redundant calls.

## Error Handling & Validation
- **Validation:** Use `FluentValidation` or `DataAnnotations` in forms.
- **UI Safety:** Implement `ErrorBoundary` to capture UI-level errors.
- **Backend:** Ensure proper try-catch blocks for API/Database calls with logging.

## Security & Authorization
- **Roles:** Centralize role names in `RoleConstants`.
- **Protection:** Protect Admin UI using `@attribute [Authorize(Roles = "...")]`.
- **Communication:** Ensure HTTPS for all web communication and implement proper CORS policies.

## Tests & CI
- **Framework:** Use **xUnit** for unit/integration testing.
- **Mocking:** Use **Moq** for mocking dependencies.
- **Workflow:** Ensure `dotnet format` respects `.editorconfig` before committing. Keep changes minimal and focused.

## When in Doubt
- Follow `.editorconfig` strictly.
- Ask for clarification if changes affect architecture, authentication, or cross-cutting concerns.

## AI Interaction & Documentation
- **Error Handling:** If a suggested solution fails, do not iterate on the failed code. Search for a fresh, better solution from the original requirement.
- **Clarification:** Ask for clarification if a request is ambiguous rather than guessing.

## Package & Dependency Management
- **Library Choice:** Use currently installed libraries first. Only suggest new ones if necessary.
- **Open Source:** If suggesting an open-source library, ensure it is free for personal/home project use.
- **Validation:** Double-check all code for compiler errors or non-existent methods before suggesting.

## Advanced Coding & Refactoring
- **Reusability:** Prioritize one-time, reusable solutions (e.g., generic Blazor components or service patterns).
- **Clean Methods:** Keep method parameters to a minimum (strictly **maximum of 5**). 
- **Parameter Objects:** If more than 5 parameters are needed, refactor them into a dedicated class or record to hold the state.
- **Best Practices:** All solutions must align with industry-standard "Best Practices" for .NET 8 (or haigher) and Blazor.
- **UI:** If the project uses **Bootstrap** for styling; ensure component markup reflects this.