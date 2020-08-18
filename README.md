# Задание
Разработать веб-приложение для ведения списка дел. Список дел имеет название и описание. Элемент списка дел имеет название, описание и дату выполнения, а также отметку выполнено/не выполнено. 

Приложение должно обеспечивать: 
- Хранение произвольного числа списков дел.
- Хранение произвольного числа дел в списке.
- Добавление, редактирование, удаление списка.
- Добавление, удаление, редактирование дела.

Приветствуются дополнительный функционал (поиск с фильтрацией, пагинация, собственные идеи).

### Требования
Бэкенд реализовать на ASP.NET Core 3.1+. Реализовывать фронтенд не обязательно, но является плюсом. Можно использовать ASP.NET MVC c Razor/Blazor либо любой SPA фреймворк. Авторизация не требуется. 

Обязательно:
- Использовать [SwaggerUI](https://swagger.io/tools/swagger-ui/ "REST API Documentation Tool") для документирования API. Реализовать с помощью библиотеки [Swashbuckle](https://www.nuget.org/packages/Swashbuckle.AspNetCore.Swagger/ "NuGet Gallery") по [инструкции](https://docs.microsoft.com/ru-ru/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-3.1&tabs=visual-studio "Начало работы с Swashbuckle и ASP.NET Core").
- Использовать EntityFramework Core для работы с БД.
- Использовать в качестве БД localdb или sqlite.
- Обеспечить разделение приложения на проекты в соответствии с трехслойной архитектурой. 
- Работу вести в репозитории на github по модели [Gitflow](https://www.atlassian.com/ru/git/tutorials/comparing-workflows/gitflow-workflow "Рабочий процесс Gitflow Workflow").

Дополнительно: 
- Покрыть код юнит-тестами. Рекомендуется использовать [xUnit](https://xunit.net/ "xUnit.net") и [FluentAssertions](https://fluentassertions.com/ "Fluent Assertions").
- Реализовать поиск по названию списка или дела.