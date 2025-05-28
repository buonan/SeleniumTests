
# SeleniumTests

A simple collection of UI tests using Selenium WebDriver and NUnit in C#. This project serves as a starting point for writing and running automated browser tests.

## Features

- Browser automation using Selenium WebDriver  
- Test framework powered by NUnit  
- Easily customizable for different test cases and web applications  

## Requirements

- [.NET SDK 6.0+](https://dotnet.microsoft.com/download)  
- [Chrome](https://www.google.com/chrome/) browser  
- [ChromeDriver](https://sites.google.com/chromium.org/driver/) installed and in your system `PATH`  

## Getting Started

1. **Clone the repository:**

```bash
git clone https://github.com/buonan/SeleniumTests.git
cd SeleniumTests
```

2. **Restore dependencies and build the project:**

```bash
dotnet restore
dotnet build
```

3. **Run the tests:**

```bash
dotnet test
```

> Make sure ChromeDriver is compatible with your Chrome browser version.

## Project Structure

- `Tests/`: Contains test files using Selenium and NUnit  
- `SeleniumTests.csproj`: Project configuration and dependencies  

## License

This project is licensed under the [MIT License](LICENSE).
