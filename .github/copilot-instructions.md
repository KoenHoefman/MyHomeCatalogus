# Copilot instructions for MyHomeCatalogus

This repository enforces project coding, formatting and architecture preferences. When suggesting code, tests, or edits, follow these rules exactly.

## Project context
- Targets: .NET 8
- Contains: Blazor (server interactive components) and WebAssembly projects
- Prefer solutions and patterns matching Blazor (components, DI, AuthenticationState, RenderModes) unless user asks otherwise.

## Formatting & editor settings (authoritative: `.editorconfig`)
- Indentation
  - Use tabs for indentation in source files.
  - Tab width: `4` for code files (C#, Razor, HTML, CSS, etc.).
  - Tab width: `2` for project/manifest files (`.csproj`, `.json`, `.xml`, etc.).
  - End-of-line: CRLF.
  - Ensure files end with a single final newline.

- C# style & naming
  - Interfaces must begin with `I` and use PascalCase.
  - Types (classes, structs, enums, delegates) use PascalCase.
  - Prefer null-propagation and coalesce expressions where appropriate.
  - Prefer auto-properties and use object/collection initializers where sensible.
  - Place `using` directives outside the namespace.
  - Prefer simple `using` statements (e.g., `using var`) when applicable.
  - Use block-scoped namespace declarations.
  - Expression-bodied methods and constructors: disabled in this project.
  - `var` guideline: prefer `var` for built-in types and when the type is apparent; otherwise be explicit.
  - Avoid implicit object creation when type is not apparent.

- Razor / Blazor
  - Use tabs for Razor files. Single-line attributes per tag are allowed.
  - `razor_indent_webgrid_col_definitions = true`.
  - Be explicit with `@rendermode` for server interactive components (e.g., `@rendermode InteractiveServer`) to match app behavior.
  - Avoid blocking calls on the UI thread (never use `.GetAwaiter().GetResult()` or `.Result` within component rendering). Use `async/await`.

## Concurrency & async guidance
- Always prefer fully async flows in Blazor Server components. Blocking the sync context can deadlock the circuit.
- Use `async Task` component lifecycle methods and `await` for I/O/EF/Identity calls.

## Authorization & Identity
- Centralize role names (use `RoleConstants`) and seed roles on startup.
- Use policies where appropriate and protect admin UI with role-based `@attribute [Authorize(Roles = "...")]`.

## Tests & CI
- Ensure `dotnet format` (or IDE Format Document) respects `.editorconfig` before committing.
- Keep changes minimal and focused. Prefer adding small, testable units and update tests accordingly.

## When in doubt
- Follow `.editorconfig` strictly.
- Prefer idiomatic Blazor code.
- Ask for clarification if the change affects architecture, authentication, or cross-cutting concerns.
