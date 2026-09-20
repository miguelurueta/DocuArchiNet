<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08 -->
## 1. Modelo y configuración

- [x] 1.1 Introducir modo autoritativo y `NoAplica`, verificando persistencia textual y MySQL 5.1. Origen: D-01, RQ-01; D-03, RQ-03
- [x] 1.2 Separar gestión y creación en `MySqlImportExpedientConfigurationRepository`. Origen: D-01, RQ-01

## 2. Preflight e intención

- [x] 2.1 Proyectar almacenamiento e índices documentales como obligatorios y solo efectos de expediente como `NotApplicable`; conservar rama 1. Origen: D-02, RQ-02; D-04, RQ-04
- [x] 2.2 Incluir el modo en fingerprint y rechazar cambios 0↔1 antes de persistir. Origen: D-06, RQ-06

## 3. Ejecución y reconciliación

- [x] 3.1 Bifurcar antes de efectos de expediente y conservar el procesamiento del universo documental por `ENLASE`. Origen: D-02, RQ-02; D-04, RQ-04
- [x] 3.2 Implementar actualización segura de índices documentales por `ID` + `ENLASE`, sin exigir ni modificar `ID_EXPEDIENTE`, con postlectura. Origen: D-04, RQ-04
- [x] 3.3 Persistir `NoAplica`, almacenar sin `ExpedientId` y completar solo tras confirmar índices documentales. Origen: D-03, RQ-03; D-05, RQ-05
- [x] 3.4 Ajustar consulta, mapper, reintento y reconciliación sin fabricar efectos físicos. Origen: D-05, RQ-05
- [x] 3.5 Preservar rama DOC-67 y guarda tardía de creación. Origen: D-07, RQ-07
- [x] 3.6 Tratar NIT/cédula y razón social ausentes como datos opcionales después de consultar expediente SII: no bloquear almacenamiento y no sobrescribir índices existentes con vacío. Origen: D-04, RQ-04
- [x] 3.7 Propagar metadatos disponibles del sello a campos vacíos de la inscripción y evitar `Confirmado` cuando no existe ningún índice físico por escribir. Origen: D-04, RQ-04
- [x] 3.8 Consultar siempre el expediente SII antes de bifurcar efectos físicos, aceptar identidad parcial segura y exigir confirmación física de índices en modo `SinExpediente`. Origen: D-04, RQ-04; D-05, RQ-05

## 4. Pruebas automatizadas

- [x] 4.1 Agregar regresión de bandera 0: cero efectos de expediente y actualización confirmada de todos los índices documentales relacionados. Origen: D-04, RQ-04
- [x] 4.2 Cubrir persistencia/relectura de `NoAplica`, finalización y fingerprint obsoleto. Origen: D-03, RQ-03; D-05, RQ-05; D-06, RQ-06
- [x] 4.3 Ejecutar regresión de bandera 1, suite ImportarServicioWeb y MSBuild. Origen: D-07, RQ-07; D-08, RQ-08

## 5. E2E y documentación

- [x] 5.1 Extender arnés existente para controles sensibles al modo, solo SELECT. Origen: D-08, RQ-08
- [x] 5.2 Ejecutar E2E autorizada de ramas 0/1, idempotencia y restauración del gate. Origen: D-08, RQ-08
- [x] 5.3 Documentar arquitectura, estados, contratos, seguridad, rollback e inventario real. Origen: D-01, RQ-01; D-08, RQ-08
- [x] 5.4 Validar OpenSpec estricto y OPSXJ con evidencia saneada. Origen: D-08, RQ-08
