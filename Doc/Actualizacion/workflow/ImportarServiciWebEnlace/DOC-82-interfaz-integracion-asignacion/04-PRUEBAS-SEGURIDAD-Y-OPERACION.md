# Pruebas, seguridad y operación

## Pruebas deterministas

- Suites `Tests/importar-servicio-web-enlase-*.test.cjs`: composición, capacidad/contexto, 0/1/N, selección total, preview, tipología, intención única, cierre, reconciliación, asignación y accesibilidad.
- `Tests/doc82-technical-documentation.test.cjs`: inventario obligatorio, PlantUML, AST JavaScript y firmas declaradas.
- Roslyn valida las firmas VB de `diagram-contract.json` en CI.

## E2E reutilizada

`import-sii-enlase-ui` (DOC-82) es de lectura y requiere `environment,gate`. Reutiliza login, selección oficial ENLASE, gate temporal, controles SELECT, evidencia saneada y restauración. Exige al menos dos filas cuando `sampleSize >= 2`; recorre selección total, preparación múltiple, tipología, preview, responsive, foco y confirma que no hubo `CreateImportIntent` ni `ExecuteImportIntent`.

La mutación permanece en `import-sii-enlase-execution` (DOC-81): requiere además ejecución y recurso descartable; prueba una intención y una ejecución para N anexos. La ejecución real no se afirma como realizada por DOC-82 hasta contar con autorización vigente y evidencia saneada.

Comandos deterministas:

```powershell
node --test Tests/importar-servicio-web-enlase-*.test.cjs Tests/doc82-technical-documentation.test.cjs
node --test tools/e2e/tests/doc82-import-sii-enlase-ui.test.cjs
dotnet run --project tools/validation/Doc72SourceValidator/Doc72SourceValidator.csproj -- Doc/Actualizacion/workflow/ImportarServiciWebEnlace/DOC-82-interfaz-integracion-asignacion/diagram-contract.json .
```

Comando real, únicamente después de las autorizaciones interactivas:

```powershell
npm.cmd --prefix tools/e2e run test:workflow:platform -- --scenario import-sii-enlase-ui --profile <perfil-runtime-saneado.json> --authorize environment,gate,local-tls
```

## Seguridad y rollback

- La UI no recibe credenciales, no consulta SQL y no persiste directamente.
- Preview usa descriptor temporal; no interpreta URL del proveedor como autoridad.
- El guard evita escribir/proyectar sobre otra tarea.
- Resultados y evidencia no exponen secretos.
- El gate debe restaurarse a `false`, con usuarios/grupos vacíos, incluso ante error.
- Rollback: desactivar el gate conserva disparadores y flujo legacy; DOC-82 no modifica contratos ni almacenamiento.

La prueba automática comprueba existencia y correspondencia estructural. No demuestra por sí sola fidelidad conductual completa, calidad visual ni disponibilidad del SII; esos aspectos requieren revisión visual y E2E autorizada.

## Resultado de validación de esta entrega

| Control | Resultado |
|---|---|
| Suites focales, regresión UI y plataforma E2E | PASS: 103/103 |
| Contrato documental JavaScript por AST | PASS: 13 firmas |
| Contrato documental VB por Roslyn | PASS: 2 firmas |
| `opsxj:refine DOC-82 --sync` | PASS |
| OpenSpec 1.7 estricto sobre todo el repositorio | PASS: 61/61 |
| E2E real autenticada DOC-82 | PASS, tarea autorizada 220587: 9 anexos observados (6 disponibles, 3 importados), preparación individual/múltiple, preview, foco y tabla responsive confirmados; 7 controles sin cambios; gate restaurado |
