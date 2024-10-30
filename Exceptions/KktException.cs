using System;

namespace RetailFixer.Exceptions;

public class KktException(string msg): Exception(msg);