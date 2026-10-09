# Validación, evidencia y pendientes

## Resultados locales

| Comando | Resultado | Alcance |
| --- | --- | --- |
| `msbuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m:1 /v:minimal` | Código 0 | Compilación .NET Framework 4.6.1; advertencias preexistentes del proyecto. |
| `node --test tests/login-second-factor-foundation.test.cjs tests/login-second-factor-persistence.test.cjs tests/login-second-factor-smtp.test.cjs` | 8/8 PASS | Regresiones DOC-91/DOC-92 y DOC-93 sin red. |
| CI: `node --test tests/login-second-factor-smtp.test.cjs` | 2/2 PASS esperado en checkout limpio | Compila con Roslyn únicamente el corte DOC-93 y sus contratos; `tests/MySqlParameterTestDouble.vb` sustituye el tipo de parámetro, sin conexión MySQL. |
| `node --test tests/doc93-technical-documentation.test.cjs` | 4/4 PASS | Inventario obligatorio y Mermaid. |
| `dotnet run --project ./tools/validation/Doc72SourceValidator/Doc72SourceValidator.csproj -- ./Doc/Actualizacion/Login/Implementacion/DOC-93/diagram-contract.json .` | PASS: 16 declaraciones, 5 relaciones, 1 enum, 13 firmas y 5 tipos | Declaraciones, implementaciones, firmas, sobrecargas y tipos por Roslyn. |
| `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-93 --sync` | PASS | Decisiones y requisitos trazables en diseño, especificación y tareas. |
| `openspec.cmd validate doc-93-segundo-factor-smtp --type change --strict --json` | PASS | Cambio OpenSpec válido sin incidencias. |
| `git diff` focal sobre `ClassCorreo.vb` y `ClassRaEnvioCorrespondencia.vb` | Sin archivos | Correo legacy fuera del diff funcional. |

La prueba conductual usa `DataTable`, conexiones, repositorios, parámetro MySQL, fábrica y cliente SMTP dobles. No usa MySQL, sockets ni credenciales reales. El build completo registrado corresponde al entorno de desarrollo: un checkout limpio del repositorio no puede reproducirlo porque el `.vbproj` contiene referencias históricas a fuentes no versionadas; por ello CI compila de forma aislada y estructuralmente trazable el corte DOC-93.

## Inconsistencias corregidas

- Se eliminó la selección implícita de la primera fila para el recorrido 2FA; múltiples filas ahora fallan cerradamente.
- La consulta nueva enumera columnas y parametriza `ESTADO_ENVIO`.
- El timeout legacy se conserva pero se protege contra valor no positivo, overflow y espera superior a 120 segundos.
- Los errores ya no retornan mensajes técnicos en el contrato 2FA.
- El cliente y el mensaje se liberan tanto en éxito como en fallo.

## Elementos no verificados

- Entrega contra un servidor SMTP real: requiere autorización explícita vigente de ambiente, cuenta y buzón descartable.
- Composición desde login/UI/ASMX: no está implementada en DOC-93.
- Fidelidad de entrega, reputación, spam, TLS/certificado y recepción: los dobles no pueden acreditarlos.
- Datos actuales de producción: no se consultaron durante esta implementación.
- Build completo desde checkout limpio: bloqueado por referencias históricas del `.vbproj` a archivos no versionados, ajenos al alcance de DOC-93.

La validación documental prueba existencia y correspondencia estructural; no demuestra por sí sola la fidelidad completa del comportamiento ni cobertura de todos los futuros casos de uso. La cobertura declarada se limita a los cinco casos implementados de DOC-93.
