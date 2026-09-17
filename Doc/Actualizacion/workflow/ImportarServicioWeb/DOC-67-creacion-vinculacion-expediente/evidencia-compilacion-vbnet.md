# Evidencia de compilación VB.NET DOC-67

Fecha: 2026-09-15. Ambiente local; sin E2E autenticada, carga ni activación del gate.

- Proyecto: `GestionDocumental-Docuarchi.net.vbproj`.
- Framework objetivo confirmado: `.NET Framework 4.6.1` (`TargetFrameworkVersion=v4.6.1`).
- MSBuild: `18.7.8.30822` (`18.7.8+1ac568fee`).
- Configuración: `Debug`.

Comando:

```text
"C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\MSBuild.exe" GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m /nologo
```

Resultado real: código de salida `0`; `0` errores y `309` advertencias preexistentes del proyecto.

La compilación usó el proyecto Web Forms/VB.NET afectado; no fue sustituida por una compilación TypeScript o npm. Las superficies legacy protegidas no se modificaron para obtener este resultado.
