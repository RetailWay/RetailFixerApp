using System;

namespace RetailFixer.Exceptions;

public class DbFactoryNotFoundException(string providerName) : Exception($"Не найден провайдер {providerName}");