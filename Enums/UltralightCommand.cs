using System;

namespace DimensionalTag.Enums
{
    //
    // Summary:
    //     List of commands available for the Mifare cards
    public enum UltralightCommand
    {
        //
        // Summary:
        //     Get the NTAG version
        GetVersion = 96,
        //
        // Summary:
        //     Read 16 Bytes
        Read16Bytes = 48,
        //
        // Summary:
        //     Read multiple pages at once
        ReadFast = 58,
        //
        // Summary:
        //     Write 16 Bytes but only last significant 4 bytes are written
        WriteCompatible = 160,
        //
        // Summary:
        //     Write 4 Bytes
        Write4Bytes = 162,
        //
        // Summary:
        //     Read the current value of the NFC one way counter
        ReadCounter = 57,
        //
        // Summary:
        //     Increase he 24 bit counter
        IncreaseCounter = 165,
        //
        // Summary:
        //     Password authentication with 4 bytes
        PasswordAuthentication = 27,
        //
        // Summary:
        //     For Ultralight C 3DS authentication
        ThreeDsAuthenticationPart1 = 26,
        //
        // Summary:
        //     For Ultralight C 3DS authentication
        ThreeDsAuthenticationPart2 = 175,
        //
        // Summary:
        //     Read the ECC specific 32 byte signature
        ReadSignature = 60,
    }
}