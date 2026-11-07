# DOC-111: degradación y carga individual PASS

Sesión 26 **PASS**, terminada el 2026-11-02 a las 21:36:31.395 UTC, sin fallos. Autorización específica `tools/e2e/artifacts/doc111-degradation-authorization-26.json`. Se cierran 13.15 y 13.16; avance **128/130**. No se acredita QA final o importación SII real.

## Ejecución real autorizada

Se seleccionó nivel 7 mediante el árbol existente y se comprobó AbrirImportacion disponible. El puente administrativo autorizado renombró temporalmente únicamente ra_integracion_evento a doc111_rec_evento_20261102. Durante esa ventana, DOC-111 respondió 503/CAPACIDAD_NO_DISPONIBLE y su interfaz mostró indisponibilidad; Producción siguió funcionando.

El flujo individual oficial DOC-88/Producción almacenó e incorporó exactamente un PDF de 677 bytes, doc111-carga-individual.pdf, en expediente 467, tipología 50. Documento **16212**, inventario **23301**. La UI confirmó almacenamiento/proyección y un solo envío. SELECT con cuenta exclusivamente de lectura confirmó un registro adicional, usuario/empresa/tipología, documento físico en gabinete y relación en el índice del expediente. La tabla operativa continuaba oculta tras la carga; no se acredita esta prueba a una carga realizada después de restaurarla.

El puente restauró la tabla en finally y contrastó contenido, columnas, índices, claves foráneas y próximo ID. Registro de recuperación RESTORED/restored=true del 2026-11-02 a las 21:35:33.835 UTC: **98 filas, próximo ID 99**. El API operativo volvió a responder correctamente. Los snapshots completos de los lotes **2–5** permanecieron idénticos antes/después. El documento individual y los recursos anteriores se conservan.

## Evidencia y límites

- Resultado saneado: `tools/e2e/artifacts/doc111-degradation-session-26.json`.
- Estado de pasos/carga/restauración: `tools/e2e/artifacts/doc111-degradation-state.json`, PASS/restored=true.
- Recuperación y hashes de snapshot: `tools/e2e/artifacts/doc111-operational-schema-recovery.json`.
- Preservación estática: `tools/e2e/artifacts/doc111-degradation-preservation-20261102.json`: **31 archivos idénticos a HEAD normalizando finales de línea**, incluidos consumidores SII, almacenamiento legacy, carga individual y Web.config. Código actual coincide con la huella ejecutada.
- **88 pruebas locales PASS** previas, incluidas DOC-88, restauración ante fallos y consumidores de sesión/control. Son evidencia local separada de la ejecución real.

La degradación real cubre ausencia de una tabla operativa; no se alteraron columnas ni se ensayaron todas las variantes de incompatibilidad. La carga individual sí se ejecutó realmente. La preservación SII se acredita por revisión/comparación estática y regresiones locales previamente documentadas; **importación SII real NO_EJECUTADO**, sin ampliar autorización ni atribuir resultados de Producción a SII. No se modificaron permisos, Web.config, función de almacenamiento ni DLL del sitio.

## Huellas ejecutadas

- HEAD: `b2e93b726f221868cf2b82d43200a8e568871117`.
- Código: `be095c78d774dc7f2d8b5fafb7f7dfb68c5eee0fe6623c8a9c8c94728e9a9a1e`.
- Driver: `cad1f02f242be858a6efe03ea865510a57617261801230bd2fa3fa08e8e3faa6`.

La huella de código cambió respecto de REC anteriores por la extensión del puente E2E compartido incluido en su ámbito; no hubo despliegue productivo. Permanecen pendientes 13.17 (matriz final) y 13.18 (gobierno/revisión).
