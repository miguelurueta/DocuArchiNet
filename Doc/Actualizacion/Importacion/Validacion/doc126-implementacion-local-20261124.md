# DOC126: implementación local y aceptación pendiente

Corte 2026-11-24T18:08:50.458Z. 40/44 tareas; abiertas 6.9, 6.10, 6.11, 7.5. Menú y diálogo Excel implementados con autoridad propia, cliente actual, plantillas/CAS, recepción, hojas, preparación, muestra/incidencias, consentimiento de reemplazo y recuperación por ID. No se atribuye aprobación humana ni E2E real DOC126.

Validación local: 150/150 pruebas; 4505 comprobaciones nativas sintéticas, libros/lector reales y regresión heredada PASS. Comando:

```text
node --test tests/doc126-importacion-excel-client.test.cjs tests/doc126-importacion-excel-ui.test.cjs tests/doc126-e2e-reuse.test.cjs tests/doc122-importacion-excel-behavior.test.cjs tests/doc122-e2e-reuse.test.cjs tests/doc124-e2e-reuse.test.cjs tests/doc125-e2e-reuse.test.cjs tests/doc119-navigation.test.cjs tests/doc119-context-menu.test.cjs tests/doc110-perfil-importacion-editor.test.cjs tools/e2e/tests/doc111-importacion-carpetas-driver.test.cjs
```

MSBuild net461 aislado: 0 errores/308 advertencias heredadas. DLL candidata 525ab00555e7be727e4f37bbf316a5b7adaee40a4c683c55e422f4bdeb50d257; la DLL IIS a14034d586facb7f700ff1cdf92c22bf74e607e69b43d3b3ab7a8556efc2e0b0 y Web.config permanecen intactos. El Content se ha editado en la carpeta principal; el renderer opcional mantiene compatibilidad con el binario anterior hasta desplegar el nuevo. El diálogo aún no se acredita contra IIS/DB.

Pruebas browser locales con transporte simulado cubren tres modos, precisión/XSS, incidencias con ok=false, CAS al cambiar hoja, doble click, timeout/acuse ignorado, cierre/respuesta tardía, guardado incierto, storage bloqueado, host reemplazado/endRequest y visibilidad/teclado con zoom 200%. La suite DOC119 prueba coexistencia con carpetas/perfiles/fuentes/proveedores. Son pruebas locales, no una matriz física.

La E2E está registrada en la suite/runner existentes: --excel-ui-read y --excel-ui-write, scope DOC126 propio, controles DOC32 SELECT DA/RA, login actual y actor externo DOC125 acotado al nuevo prefijo. Los cinco XLSX sintéticos de 1000/1000/1000/300/10 filas fueron leídos íntegramente con DOC123; 0 escrituras DB. La ubicación y los manifiestos de ejecución quedan en .tmp excluido de Git. No se recuperan claves o sesiones anteriores.

[SQL 22–25 y orden manual](../Sql/README.md): entregados, sin aplicar. No modificar EGRESOS, CONTRATOS, CRÉDITOS ni los resultados DOC125; los nuevos positivos requieren fixtures nuevos y CREAR ausente. La cuenta propietaria debe acreditar permiso exclusivo del cargador; los permisos adicionales de otra cuenta no sustituyen ese caso.

Límites: 20000 filas / 10000000 bytes configurados, muestra 25, incidencias 25, HTTP 30 s y cero retries. La capacidad física anterior de 1000 por modo no certifica esta interfaz ni 20000. No hay pausa durable por filas, cancelación al cerrar ni corte físico de COMMIT/red inducido. Guardado de plantilla por UI aún sin cobertura física DOC126. Gobierno/cierre continúan abiertos por aceptación real y revisión final. Los 25 hashes del JSON identifican la fuente exacta de esta entrega.

Auditoría de refinamiento: 15/15 controles aprobados. OpenSpec estricto global: 97/97 elementos válidos. Se contrastaron los 25 hashes de fuentes. Estos controles locales no cierran las cuatro tareas pendientes ni sustituyen la aceptación real.
