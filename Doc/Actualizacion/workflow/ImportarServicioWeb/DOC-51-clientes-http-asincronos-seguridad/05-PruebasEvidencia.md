# Pruebas y evidencia

- Ticket: DOC-51
- Cambio OpenSpec: doc-51-servicio-proveedor
- Clasificacion: cross_cutting

Fecha: 2026-09-06.

| Evidencia | Resultado |
|---|---|
| `node --test Tests/importar-servicio-web-http-contract.test.cjs Tests/importar-servicio-web-http-security.test.cjs Tests/importar-servicio-web-http-cancellation.test.cjs Tests/importar-servicio-web-contracts.test.cjs Tests/importar-servicio-web-context.test.cjs Tests/importar-servicio-web-provider-registry.test.cjs` | PASS, 22/22, incluido servidor loopback explícito |
| `msbuild GestionDocumental-Docuarchi.net.vbproj /t:Compile /p:Configuration=Debug /m /v:minimal` | PASS; advertencias preexistentes del proyecto legacy |
| `openspec.cmd validate doc-51-servicio-proveedor --strict` | PASS |
| `opsxj:refine DOC-51 -NonInteractive` | PASS |

Las pruebas usan archivos saneados y `127.0.0.1`. No se hicieron llamadas SII, E2E autenticado, carga, cambios de base de datos ni activación del gate. La cobertura de integración del adaptador SII se difiere a Backend 06.
