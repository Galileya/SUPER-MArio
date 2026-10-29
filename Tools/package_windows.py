"""Package the complete Windows player, excluding Unity debug-only outputs."""
from pathlib import Path
from zipfile import ZIP_DEFLATED, ZipFile

root = Path(__file__).resolve().parents[1]
build = root / "Builds/Windows"
output = root / "Builds/Entrega/SuperMario-Galileya-Windows.zip"
required = ("Mario.exe", "UnityPlayer.dll", "Mario_Data/globalgamemanagers")
for name in required:
    if not (build / name).is_file():
        raise SystemExit(f"Missing build file: {name}")
output.parent.mkdir(parents=True, exist_ok=True)
with ZipFile(output, "w", ZIP_DEFLATED, compresslevel=6) as archive:
    for path in sorted(build.rglob("*")):
        relative = path.relative_to(build)
        if not path.is_file() or any("DoNotShip" in part for part in relative.parts) or path.suffix == ".pdb":
            continue
        archive.write(path, Path("SuperMario") / relative)
    archive.writestr("SuperMario/LEEME.txt", """SUPER MARIO - ENTREGA WINDOWS

1. Extraer TODO el archivo ZIP en una carpeta.
2. Abrir Mario.exe. Conservar sus carpetas y DLL junto al ejecutable.
3. Elegir el nivel en el menu con las flechas y Enter o con el mouse.

Controles: A/D o flechas para mover; Espacio para saltar o nadar;
Escape para pausar; flechas arriba/abajo y Enter para el menu.

Niveles: 1-1 superficie; 1-2 subterraneo; 1-3 acuatico;
1-4 castillo de Bowser con escenario propio.

Repositorio: https://github.com/Galileya/SUPER-MArio
""")
with ZipFile(output) as archive:
    bad = archive.testzip()
    if bad:
        raise SystemExit(f"Corrupt ZIP entry: {bad}")
    for name in required:
        archive.getinfo("SuperMario/" + name)
    print(f"Verified: {len(archive.namelist())} entries")
print(output)
print(f"Size: {output.stat().st_size / (1024 * 1024):.1f} MiB")
