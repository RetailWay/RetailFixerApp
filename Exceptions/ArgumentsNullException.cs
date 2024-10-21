using System;

namespace RetailFixer.Exceptions;

public class ArgumentsNullException(params string[] paramsName)
    : ArgumentException($"Необходимо заполнить следующие аргументы: {string.Join(", ", paramsName)}");