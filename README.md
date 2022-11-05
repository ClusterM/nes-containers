# NesContainers
A simple .NET Standard 2.0 library for reading and modifying NES/Famicom ROM containers: .nes (iNES, NES 2.0), .unf (UNIF), and .fds (Famicom Disk System images).

Full documentation: https://clusterm.github.io/nes-containers/

## Usage

There are three classes for different type of containers.

### .nes (iNES, NES 2.0)

Most popular ROM container. Most older ROMs are stored in the iNES format, but most modern dumps usually use the newer version: NES 2.0. This class supports both.

Load ROM: `var nesfile = new NesFile(filename)` or `var nesfile = NesFile.FromFile(filename)`

Access fields:

Set mapper: `nesfile.Mapper = 4;`

Set version to NES 2.0: `nes.Version = NesFile.iNesVersion.NES20;`

Set PRG data: `nesfile.PRG = new byte[32768] { ... };`

Set CHR data: `nesfile.CHR = new byte[8192] { ... };`

Enable battery saves: `nesfile.Battery = true;`

Save ROM as .nes file: `nesfile.Save(filename);`

Check [documentation](https://clusterm.github.io/nes-containers/classcom_1_1clusterrr_1_1_famicom_1_1_containers_1_1_nes_file.html) for all available properties.

Full format specifications: https://www.nesdev.org/wiki/INES

### .unf (UNIF)

UNIF (Universal NES Image Format) is an alternative format for holding NES and Famicom ROM images. Its motivation was to offer more description of the mapper board than the popular iNES format, but it suffered from other limiting constraints and a lack of popularity. The format is considered deprecated, replaced by the NES 2.0 revision of the iNES format, which better addresses the issues it had hoped to solve. There are a small number of game rips that currently only exist as UNIF. UNIF is currently considered a deprecated standard.

UNIF uses key-value format fields. Key is four character string and value is binary data. In theory you can save data as any field but there are several standard fields.

Load ROM: `var uniffile = new UnifFile(filename)` or `var uniffile = UnifFile.FromFile(filename)`

You can assess fields like dictionary:

Set mapper name: `uniffile["MAPR"] = "COOLGIRL";`

Set PRG data: `uniffile["PRG0"] = new byte[...] {...};`

But all standatd fields also available as properties:

Set mapper name: `uniffile.Mapper = "COOLGIRL";`

Set mirroring: `uniffile.Mirroring = MirroringType.MapperControlled;`

Save ROM as .unf file: `unif.Save(filename);`

Check [documentation](https://clusterm.github.io/nes-containers/classcom_1_1clusterrr_1_1_famicom_1_1_containers_1_1_unif_file.html) for all available properties.

Full format specifications: https://www.nesdev.org/wiki/UNIF

### .fds (Famicom Disk System images)

The FDS format is a way to store Famicom Disk System disk data. It's much more complex format as it can contain multiple disks/sides and each disk/side contains a disk header and files.
                                                     
```
                                                                         /-- File header block
                                /-- Disk header block       /- File #1 --
            /- Disk 1, side A ----- File amount block      /             \-- File data block
           /                    \-- File blocks------------
FDS file ----- Disk 1, side B                              \             /-- File header block
           \                                                \- File #2 --
            \- Disk 2, side A                                            \-- File data block
```

Load ROM: `var fdsfile = new FdsFile(filename)` or `var fdsfile = FdsFile.FromFile(filename)`

Get disk(s) sides: `IList<FdsDiskSide> sides = fdsfile.Sides;`

Get game name code: `var name = fdsfile.sides[0].GameName;`

Get file name: `var filename = fdsfile.sides[0].Files[0].FileName;`

Get file data: `var filedata = fdsfile.sides[0].Files[0].Data;`

Save ROM as .fds file: `fdsfile.Save(filename);`

Check [documentation](https://clusterm.github.io/nes-containers/classcom_1_1clusterrr_1_1_famicom_1_1_containers_1_1_fds_file.html) for all available classes and properties.

Full format specifications: https://www.nesdev.org/wiki/FDS_file_format

## Get it on NuGet
https://www.nuget.org/packages/NesContainers/

## Donate
https://www.donationalerts.com/r/clustermeerkat

https://boosty.to/cluster
