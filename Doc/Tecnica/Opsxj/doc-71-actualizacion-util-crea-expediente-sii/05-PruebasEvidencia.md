# ACTUALIZACION-UTIL-CREA-EXPEDIENTE-SII

- Ticket: DOC-71
- Cambio OpenSpec: doc-71-actualizacion-util-crea-expediente-sii
- Clasificacion: cross_cutting (Transversal)
## Evidencia requerida

- [x] unit: `node --test tests/importar-servicio-web-*.test.cjs`; 370 aprobadas, 0 fallidas,
  2026-09-20. `msbuild GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m`;
  0 errores, con advertencias históricas.
- [x] policy: `npm.cmd --prefix tools/e2e run test:doc56:policy`; 21 aprobadas, 0 fallidas,
  incluida repetición de `CreateImportIntent` con la misma clave e identidad estable.
- [x] manual_qa: 2026-09-20; E2E autorizadas de rama 0 (`220584`) y rama 1 (`220585`),
  idempotencia, reconciliación y restauración del gate confirmadas; inspección física de todos los
  documentos `ENLASE=S002194109` confirmó `MATRICULA`, `NITCEDULA` y `RAZONSOCIAL`.

## QA/E2E WebForms

DOC-71 reutiliza exclusivamente `tools/e2e`. Cada ejecución real requiere autorización expresa para
ambiente, cuentas, TLS, gate y recursos descartables. Al cierre debe conservarse evidencia saneada,
controles exclusivamente `SELECT` y restauración de `WorkflowCentroTrabajoModernActive=false`,
usuarios/grupos vacíos.

La primera corrida real autorizada de la rama 0 completó almacenamiento, índices documentales,
reconciliación e idempotencia. La evidencia saneada mostró seis controles con cambio y caché de vínculo
sin cambio. El veredicto global fue `E2E_PLATFORM_MUTATION_EXPECTATION_FAILED` por una expectativa
histórica del arnés que exigía cambio de caché también sin expediente. Se corrigió la política de forma
sensible al modo y pasó 21/21, junto con 22/22 pruebas del runner transversal. El recurso ya mutado no se
reutilizará. `manual_qa` permanece pendiente hasta completar la rama 1 y cerrar la matriz E2E.

La revisión física posterior invalidó la confirmación de índices de esa primera corrida: los metadatos
descargados del sello no se propagaban a la inscripción usada para actualizar documentos relacionados y
el caso sin campos devolvía un `Confirmado` ficticio. Se corrigió la propagación solo hacia campos vacíos
y el caso sin valores ahora produce `NoAplica`. Se requiere una nueva muestra de rama 0 además de la rama
1 antes de aprobar `manual_qa`.

La segunda corrida autorizada de rama 0, realizada el 2026-09-20 sobre una nueva generación preparada
de la misma muestra, terminó con `success=true`, `CONFIRMED` en la creación idempotente y modo
`without-expedient`. El recurso pasó por `E2E_RESOURCE_READY`, reserva y consumo; seis controles
persistentes cambiaron y la caché de vínculo no cambió. El cierre confirmó el gate en `false`, alcance
vacío y páginas legacy sin diferencias. El control `import-document-index-state` solo observa estados del
diario y no los valores físicos del gabinete; por tanto esta corrida no demuestra todavía que
`MATRICULA`, `NITCEDULA` y `RAZONSOCIAL` quedaron actualizados en todos los documentos del `ENLASE`.
La comprobación física de esos valores y la rama 1 continúan pendientes.

La corrida definitiva de rama 0 se ejecutó el 2026-09-20 sobre la tarea descartable `220584`, recibo
`S002194109` y muestra MERCANTIL. El runner terminó con `success=true`, modo `without-expedient`,
creación idempotente `CONFIRMED`, seis controles persistentes con cambio y caché de vínculo sin cambio.
El recurso pasó por preflight, reserva y consumo; el gate terminó en `false`, con alcance vacío y páginas
legacy sin diferencias. El responsable confirmó manualmente que todos los documentos relacionados por
`ENLASE=S002194109` quedaron con `MATRICULA`, `NITCEDULA` y `RAZONSOCIAL` actualizados. La rama 0 queda
validada; `manual_qa` global continúa pendiente exclusivamente de la muestra creadora de rama 1.

La rama 1 se ejecutó el 2026-09-20 sobre la tarea descartable `220585`, recibo `S002495171` y muestra
MERCANTIL. El runner terminó con `success=true`, modo `with-expedient` y creación idempotente
`CONFIRMED`. Los siete controles persistentes cambiaron, incluida la caché de vínculo, y quedaron
confirmados ejecución, consulta y reconciliación. El recurso pasó por preflight, reserva y consumo; el
gate terminó en `false`, con usuarios/grupos vacíos y páginas legacy sin diferencias. Con esta corrida
quedan cubiertas ambas ramas exigidas por la tarea 5.2.
