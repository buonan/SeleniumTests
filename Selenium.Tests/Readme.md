# Author: Brian Ku
# File Overview: Readme on how to run Selenium tests.

## Firefox dotnet bug
```
$export MONO_IOMAP=all 
```

## How to run tests
```
$mono --runtime=v4.0 --debug /opt/mono/NUnit-2.6.3/bin/nunit-console.exe Selenium.Tests.dll
$mono --runtime=v4.0 --debug /opt/mono/NUnit-2.6.3/bin/nunit-console.exe -labels -fixture:Selenium.Tests.BrowserStackTestSuite --include:BrowserStack Selenium.Tests.dll
$mono --runtime=v4.0 --debug /opt/mono/NUnit-2.6.3/bin/nunit-console.exe -labels -run:Selenium.Tests.BrowserStackTestSuite.BrowserStack_LoginTest Selenium.Tests.dll
$mono --runtime=v4.0 --debug /opt/mono/NUnit-2.6.3/bin/nunit-console.exe -labels -include:Integration Selenium.Tests.dll
$mono --runtime=v4.0 --debug /opt/mono/NUnit-2.6.3/bin/nunit-console.exe -labels -include:VideoCarouselTests+P0 Selenium.Tests.dll
$mono --runtime=v4.0 --debug /opt/mono/NUnit-2.6.3/bin/nunit-console.exe -labels -exclude:BrowserStack,ProductionF,BVT,Integration,HomeStreamUrls,CssTests Selenium.Tests.dll
```
## Code Coverage
```
istanbul report -v --include json/**/*.json  html  
```
## BrowserStackLocal
```
$cd ~/Downloads
$./BrowserStackLocal [key] localhost,3000,0
$mono --runtime=v4.0 --debug /opt/mono/NUnit-2.6.3/bin/nunit-console.exe --run:Selenium.Tests.BrowserStackTestSuite.LocalStack_Test Selenium.Tests.dll
```