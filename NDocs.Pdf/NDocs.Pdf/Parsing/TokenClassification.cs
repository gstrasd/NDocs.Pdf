using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Parsing
{
    internal enum TokenClassification : byte
    {
        Undefined,
        HeaderMajorVersion,
        HeaderMinorVersion,
        HeaderMagicNumber,
        Comment,
        BooleanTrue,
        BooleanFalse,
        Integer,
        ObjectNumber,
        GenerationNumber,
        Real,
        LiteralString,
        HexadecimalString,
        Name,
        BeginArray,
        EndArray,
        BeginDictionary,
        EndDictionary,
        BeginObject,
        EndObject,
        BeginStream,
        Stream,
        EndStream,
        FreeEntry,
        NewEntry,
        Null,
        ObjectReference,
        StartCrossReference,
        CrossReference,
        Trailer,
        EndOfFile
    }
}