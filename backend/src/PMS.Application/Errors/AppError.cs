using FluentResults;

namespace PMS.Domain.Errors;

public class AppError : Error
{
    public string Code { get; }

    public AppError(string code, string message) : base(message)
    {
        Code = code;
        WithMetadata("Code", code);
    }

    // Common errors
    public static AppError ValidationError(string message) =>
        new("VALIDATION_ERROR", message);

    public static AppError NotFound(string entity) =>
        new($"{entity.ToUpper()}_NOT_FOUND", $"{entity} не найден");

    // Employee errors
    public static AppError EmployeeNotFound =>
        new("EMPLOYEE_NOT_FOUND", "Сотрудник не найден");

    public static AppError EmployeeEmailExists =>
        new("EMPLOYEE_EMAIL_ALREADY_EXISTS", "Email уже используется другим сотрудником");

    public static AppError EmployeeIsManager =>
        new("EMPLOYEE_IS_MANAGER", "Сотрудник является менеджером проектов");

    public static AppError ManagerNotFound =>
        new("MANAGER_NOT_FOUND", "Менеджер не найден");

    // Project errors
    public static AppError ProjectNotFound =>
        new("PROJECT_NOT_FOUND", "Проект не найден");

    public static AppError InvalidDates =>
        new("INVALID_DATES", "Дата начала должна быть раньше даты окончания");

    public static AppError ProjectHasDocuments =>
        new("PROJECT_HAS_DOCUMENTS", "Нельзя удалить проект с документами");

    public static AppError ManagerInEmployees =>
        new("MANAGER_IN_EMPLOYEES", "Менеджер не может быть в списке исполнителей");

    public static AppError EmployeeNotFoundById(Guid id) =>
        new("EMPLOYEE_NOT_FOUND", $"Сотрудник с ID {id} не найден");

    // Document errors
    public static AppError DocumentNotFound =>
        new("DOCUMENT_NOT_FOUND", "Документ не найден");

    public static AppError FileTooLarge =>
        new("FILE_TOO_LARGE", "Файл слишком большой (макс. 50MB)");

    public static AppError InvalidExtension =>
        new("INVALID_EXTENSION", "Недопустимое расширение файла");

    public static AppError FileMissing =>
        new("FILE_MISSING", "Файл отсутствует на сервере");

    public static AppError ProjectNotFoundForDocument =>
        new("PROJECT_NOT_FOUND", "Проект не найден");
}