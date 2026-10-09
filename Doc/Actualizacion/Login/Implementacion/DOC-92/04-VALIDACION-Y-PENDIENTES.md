# Validación automática y pendientes

## Prueba documental

`tests/doc92-technical-documentation.test.cjs` usa el inventario explícito `diagram-contract.json` para:

1. exigir todos los documentos y diagramas declarados;
2. analizar cada diagrama con Mermaid real;
3. comparar las referencias `CODE:` del archivo con el manifiesto;
4. comprobar que las fuentes trazadas existen;
5. restringir exclusiones a actores `EXT:` y conceptos `CONCEPT:`;
6. verificar que el inventario técnico mencione todos los tipos y firmas contratados;
7. confirmar la integración de los validadores en CI.

`tools/validation/Doc72SourceValidator` analiza sintaxis Visual Basic mediante Roslyn. Para DOC-92 valida archivos y tipos, relaciones `Implements`, pertenencia de cada método a su clase/interfaz, parámetros, retornos y propiedades. Las firmas completas distinguen métodos homónimos por tipo y lista de parámetros, evitando coincidencias textuales ambiguas.

Los errores indican el identificador del manifiesto, archivo o tipo esperado y el motivo: declaración ausente/ambigua, relación inexistente, firma no coincidente o propiedad inconsistente.

## Comandos

- `node --test tests/doc92-technical-documentation.test.cjs`
- `dotnet run --project ./tools/validation/Doc72SourceValidator/Doc72SourceValidator.csproj -- ./Doc/Actualizacion/Login/Implementacion/DOC-92/diagram-contract.json .`
- `npx --yes @fission-ai/openspec@1.7.0 validate doc-92-segundo-factor-persistencia --strict`

## Resultados de esta revisión

Fecha: 2026-10-09.

| Comando | Código | Resultado |
| --- | ---: | --- |
| `node --test tests/doc92-technical-documentation.test.cjs` | 0 | 4/4: inventario requerido, Mermaid, referencias, contenido e integración CI. |
| `dotnet run --project ./tools/validation/Doc72SourceValidator/Doc72SourceValidator.csproj -- ./Doc/Actualizacion/Login/Implementacion/DOC-92/diagram-contract.json .` | 0 | 15 declaraciones, 5 relaciones, 39 firmas y 7 tipos con propiedades. |
| `node --test tests/doc91-technical-documentation.test.cjs tests/doc92-technical-documentation.test.cjs tests/login-second-factor-foundation.test.cjs tests/login-second-factor-persistence.test.cjs` | 0 | 14/14; incluye regresión documental y funcional DOC-91/DOC-92. |
| Validador Roslyn con manifiesto DOC-91 | 0 | 32 declaraciones, 2 enums, 34 firmas y 4 tipos; sin regresión. |
| `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-92 --sync` | 0 | D-01 a D-10 trazables a diseño, especificación y tareas. |
| `openspec.cmd validate doc-92-segundo-factor-persistencia --strict` | 0 | Cambio OpenSpec válido. |

La primera ejecución Roslyn dentro del sandbox no pudo escribir en `obj`; la ejecución autorizada fuera del sandbox completó correctamente. No hubo acceso a base de datos, red de aplicación o credenciales.

## Límite de la prueba

La prueba valida existencia, sintaxis y correspondencia estructural. No demuestra por sí sola la fidelidad completa del comportamiento, la corrección del SQL en MySQL real, la concurrencia efectiva ni la cobertura exhaustiva de todos los casos de uso. Las pruebas conductuales DOC-92 aportan evidencia separada, y la integración MySQL continúa sujeta a autorización vigente.

## Pendientes de validación

- La implementación externa de `DocuArchiCore` no se compila ni resuelve desde el CI de `DocuArchiNet`; su compatibilidad se basa en la exploración previa y debe revisarse nuevamente si cambia ese repositorio.
- No se ejecuta integración MySQL real en esta revisión documental.
- No existe Controller, Service, endpoint, UI ni consumidor productivo que permita una prueba E2E del login.
- `Complete` y `FailFinalization` siguen siendo indistinguibles con el esquema existente.
- `sessionBindingHash` no se compara en el repositorio; su garantía depende de la verificación HMAC del consumidor futuro.
