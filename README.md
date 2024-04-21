# SoulsFormats
A .NET library for reading and writing various FromSoftware file formats, targeting .NET 6 and 7.  
Elden Ring, Dark Souls, Demon's Souls, Bloodborne, and Sekiro are the main focus, but other From games may be supported to varying degrees.  
A brief description of each supported format can be found in FORMATS.md, with further documentation for some formats.

Forked from JKAnderson's repository, but also contains all the new changes from [DS Map Studio](https://github.com/soulsmods/DSMapStudio/tree/master/SoulsFormats) (where it's currently maintained officially?). Sadly I have no option but to merge the changes manually — all credits go to the contributors there.

Additionally, I implement my own changes as well, mainly to the Headerizer class, which is used for deswizzling console textures (the most popular being PS4).

## Usage
Objects for most formats can be created with the static method Read, which accepts either a Memory\<byte>, or a file path. A very new addition to the library was to use Memory Mapped files, which reduces memory consumption, and is much faster due to unnecessary allocations.
```cs
BND4 bnd = BND4.Read(@"C:\your\path\here.chrbnd");

// or

var file = MemoryMappedFile.CreateFromFile(@"C:\your\path\here.chrbnd", FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
var accessor = file.CreateMemoryAccessor(0, 0, MemoryMappedFileAccess.Read);
BND4 bnd = BND4.Read(accessor.Memory);
```

The Write method can be used to create a new file from an object. If given a path it will be written to that location with a stream, otherwise a byte array will be returned.
```cs
bnd.Write(@"C:\your\path\here.chrbnd");

// or

byte[] bytes = bnd.Write();
using FileStream stream = File.Create(@"C:\your\path\here.chrbnd");
stream.Write(bytes, 0, bytes.Length);
stream.Flush();
```

DCX (compressed files) will be decompressed automatically and the compression type will be remembered and reapplied when writing the file.
```cs
BND4 bnd = BND4.Read(@"C:\your\path\here.chrbnd.dcx");
bnd.Write(@"C:\your\path\here.chrbnd.dcx");
```

The compression type can be changed by either setting the Compression field of the object, or specifying one when calling Write.
```cs
BND4 bnd = BND4.Read(@"C:\your\path\here.chrbnd.dcx");
bnd.Write(@"C:\your\path\here.chrbnd", DCX.Type.None);

// or

BND4 bnd = BND4.Read(@"C:\your\path\here.chrbnd.dcx");
bnd.Compression = DCX.Type.None;
bnd.Write(@"C:\your\path\here.chrbnd");
```

Finally, DCX files can be generically read and written with static methods if necessary. DCX holds no important metadata so they read/write directly to/from byte arrays instead of creating an object.
```cs
Memory<byte> bndBytes = DCX.Decompress(@"C:\your\path\here.chrbnd.dcx");
BND4 bnd = BND4.Read(bndBytes);

// or

var file = MemoryMappedFile.CreateFromFile(@"C:\your\path\here.chrbnd.dcx", FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
var accessor = file.CreateMemoryAccessor(0, 0, MemoryMappedFileAccess.Read);
Memory<byte> bndBytes = DCX.Decompress(accessor.Memory);
BND4 bnd = BND4.Read(bndBytes);
```

Writing a new DCX requires a DCX.Type parameter indicating which game it is for. DCX.Decompress has an optional out parameter indicating the detected type which should usually be used instead of specifying your own.
```cs
Memory<byte> bndBytes = DCX.Decompress(@"C:\your\path\here.chrbnd.dcx", out DCX.Type type);
DCX.Compress(bndBytes, type, @"C:\your\path\here.chrbnd.dcx");

// or

Memory<byte> bndBytes = DCX.Decompress(@"C:\your\path\here.chrbnd.dcx", out DCX.Type type);
Memory<byte> dcxBytes = DCX.Compress(bndBytes.Span, type);
using FileStream stream = File.Create(@"C:\your\path\here.chrbnd.dcx");
stream.Write(dcxBytes.Span);
stream.Flush();
```

## Special Thanks
* albeartron
* Atvaark
* B3LYP
* horkrux
* HotPocketRemix
* katalash
* Lance
* Meowmaritus
* Nyxojaele
* Pav
* SeanP
* thefifthmatt
* Wulf2k
* Yoshimitsu
* [DS Map Studio](https://github.com/soulsmods/DSMapStudio/tree/master/SoulsFormats)
* [DS Anim Studio](https://github.com/Meowmaritus/DSAnimStudio)
