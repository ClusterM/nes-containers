# NesContainers
A simple .NET library for working with NES/Famicom ROM containers: .nes (iNES, NES 2.0), .unf (UNIF) and .fds (Famicom Disk System images).

Full documentation: https://clusterm.github.io/nes-containers/

## Usage

There are three classes for different type of containers.

### .nes (iNES, NES 2.0)

Most popular ROM container.

Load ROM: `var nesfile = new NesFile(filename)` or `var nesfile = NesFile.FromFile(filename)`

Access different fields:

`nesfile.Mapper = 4;` // Set mapper

`nesfile.PRG = new byte[32768]; // Set PRG data

`nesfile.CHR = new byte[8192];  // Set CHR data

`nesfile.Battery = true;        // Enable battery saves

