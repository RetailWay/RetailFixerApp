using System;

namespace RetailFixer.Exceptions;

public class AllOptionalArgumentsNullException(params string[] paramsName)
    : ArgumentException($"Необходимо заполнить хотя бы одно из следующих аргументов: {string.Join(", ", paramsName)}");