# PriceMonitor

### Goal:
This application is composed by:
1. Bot than search products in the web;
2. API to listen and to receive the produts data of the bot and to send messages to Telegram bot; 

### Requirements:
1. ##### Install dotnet-sdk (version 8 or higher);
2. ##### Install Microsoft SQLServer;
3. ##### Install Microsoft Entity Framework:</br>
    *dotnet add package Microsoft.EntityFrameworkCore*</br>
    *dotnet add package Microsoft.EntityFrameworkCore.Design*</br>
    *dotnet add package Microsoft.EntityFrameworkCore.SqlServer*</br>
    *dotnet tool install --global dotnet-ef*</br>
4. ##### Set appsettings.json with your own data;</br>
5. ##### Create a first Migration and update the database:</br>
    *dotnet ef migrations add MigrationsName*</br>
    *dotnet ef database update*</br>
6. ##### Install ChromeDriver, Robot Framework, Selenium Library and other dependecies:</br>
    *wget https://storage.googleapis.com/chrome-for-testing-public/136.0.7052.2/linux64/chromedriver-linux64.zip*</br>
    *cd chromedriver-linux64*</br>
    *cp -f chromedriver /usr/local/bin*</br>
    *chmod +x /usr/local/bin/chromedriver*</br>
    *python3 -m pipx ensurepath*</br>
    *python3.11 -m pipx ensurepath*</br>
    *pipx install robotframework*</br>
    *pipx inject robotframework robotframework-seleniumlibrary*</br>
    *pipx inject robotframework robotframework-seleniumlibrary*</br>
    *pipx inject robotframework robotframework-requests*</br>
    *pipx inject robotframework robotframework-jsonlibrary*</br>
    *pipx inject robotframework webdrivermanager*</br>
    *webdrivermanager chrome --linkpath /usr/local/bin*</br>

> Author: Francisco Capitan
