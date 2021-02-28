using System;

public class EncodingInvalidoException : Exception
{
    public EncodingInvalidoException() : base() {}
    public EncodingInvalidoException(string message) : base(message) {}   
  
}