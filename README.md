Provides interop between Onigmo and CLR/Mono managed code.

There are two pieces that make up the package:

* `onigwrap` - A C library that wraps Onigmo, greatly simplifying the interface for which we need to provide interop. This also greatly limits the flexibility of Onig, but for our use of this library, we didn't need any of that flexibility.
* `OnigRegex` - A C# class library that implements interop to onigwrap, and exposes a tidier interface to consumers.

Building
========

First, get a copy of Onigmo or Oniguruma.

* https://github.com/k-takata/Onigmo
* https://github.com/kkos/oniguruma (We don't actively test against oniguruma, but it should work.)

Copy `oniguruma.h` into the `onigwrap` folder (or copy `onigmo.h` and rename it to `oniguruma.h` if using Onigmo), alongside `onigwrap.c` and `onigwrap.h`.

NOTE: PInvoke can be used without an extension cross platform if the library filename (without extension) is consistent - `[DllImport("libonigwrap")]` https://github.com/dotnet/corefx/issues/24444#issuecomment-334550197

From here, the build steps diverge for each platform:

Mac
---

Configure and build onig. The defaults should work, but Mono on Mac is usually 32 bit by default, so we'll add in the `-m32` flag.

`./configure "CFLAGS=-m32"`

`make`

Copy `.libs/libonig.a` to the `onigwrap` folder.

Now we build onigwrap:

`clang -m32 -dynamiclib -L. -lonig -o libonigwrap.dylib onigwrap.c`

Take the dylib and put it alongside your binary.

Windows
-------

Build and configure onig. Copy the `win32/Makefile` and `win32/config.h` to onig's root directory and run `nmake`. If you're building Onig as 64 bit, you'll need to edit the Makefile and add `/MACHINE:X64` to the LINKFLAGS

Copy `onig_s.lib` to the `onigwrap` folder.

With the `onigwrap` folder as your working dir, build onigwrap:

`cl.exe /DONIG_EXTERN=extern /D_USRDLL /D_WINDLL onigwrap.c /link /LTCG onig_s.lib /DLL /OUT:onigwrap.dll`

Copy `onigwrap.dll` to the folder with your binary. (For example, `OnigRegexTests/bin/Debug` and `OnigWrapConsoleTest/bin/Debug`.)


```powershell
(New-Object System.Net.WebClient).DownloadFile("https://github.com/k-takata/Onigmo/archive/master.zip", (Join-Path $pwd "onigmo-master.zip"))
Expand-Archive onigmo-master.zip -DestinationPath .
cd Onigmo-master
cp win32/Makefile
cp win32/config.h
(Get-Content Makefile) -creplace '^(LINKFLAGS\s*=\s*.*)$', '$1 /MACHINE:X64' | Set-Content Makefile
#%comspec% /k "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Auxiliary\Build\vcvars64.bat"
# https://stackoverflow.com/a/2124759/4473405
& %comspec% /c '"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Auxiliary\Build\vcvars64.bat"&set' |
    foreach {
        if ($_ -match "(.*?)=(.*)") {
            Set-Item -force -path "ENV:\$($matches[1])" -value "$($matches[2])"
        }
    }
nmake
cp build_x64/onigmo_s.lib ../onigwrap/onig_s.lib
cp onigmo.h ../onigwrap/oniguruma.h
cd ../onigwrap/
cl.exe /DONIG_EXTERN=extern /D_USRDLL /D_WINDLL onigwrap.c /link /LTCG onig_s.lib /DLL /OUT:libonigwrap.dll
```

Linux
-----

Build and configure onig. We need to prepare onig for static linking though, so add `-fPIC` to the CFLAGS. If your Mono version is 32bit, make sure to add -m32 to the CFLAGS too. (You may need to install a package like `gcc-multilib` to make the build work with -m32.)

`./configure "CFLAGS=-fPIC"`

Copy `.libs/libonig.a` to the `onigwrap` folder.

Build onigwrap:

`gcc -shared -fPIC onigwrap.c libonig.a -o libonigwrap.so`

```sh
wget -O onigmo-master.zip https://github.com/k-takata/Onigmo/archive/master.zip
unzip onigmo-master.zip
cd Onigmo-master
./configure "CFLAGS=-fPIC"
make
cp .libs/libonigmo.a ../onigwrap/libonig.a
cp onigmo.h ../onigwrap/oniguruma.h
cd ../onigwrap/
gcc -shared -fPIC onigwrap.c libonig.a -o libonigwrap.so
```

Copy `libonigwrap.so` alongside your binary.
