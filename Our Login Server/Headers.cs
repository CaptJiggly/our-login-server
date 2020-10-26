using System;



public enum Headers : ushort
{
    Login,
    Register
}

public enum ErrorCodes : ushort
{
    Success,
    Exists,
    InvalidLogin,
    Error
}