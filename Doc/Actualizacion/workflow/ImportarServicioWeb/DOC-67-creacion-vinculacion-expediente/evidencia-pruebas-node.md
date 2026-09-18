# Evidencia local Node DOC-67

Fecha: 2026-09-15. Ambiente local; sin E2E autenticada, carga ni activación del gate.

## Suite focal

Comando:

```text
node --test Tests/importar-servicio-web-expedient-configuration.test.cjs Tests/importar-servicio-web-expedient-planning.test.cjs Tests/importar-servicio-web-expedient-creation.test.cjs Tests/importar-servicio-web-expedient-relation.test.cjs Tests/importar-servicio-web-document-link-cache.test.cjs Tests/importar-servicio-web-related-documents.test.cjs Tests/importar-servicio-web-electronic-index.test.cjs Tests/importar-servicio-web-failure-injection.test.cjs Tests/importar-servicio-web-reconciliation.test.cjs Tests/importar-servicio-web-state-machine.test.cjs Tests/importar-servicio-web-gate-regression.test.cjs Tests/importar-servicio-web-legacy-regression.test.cjs Tests/importar-servicio-web-storage-invariance.test.cjs
```

Resultado real: código de salida `0`; 59 pruebas, 59 aprobadas, 0 fallidas.

## Suite local completa

Comando PowerShell:

```text
$tests = Get-ChildItem Tests -Filter 'importar-servicio-web-*.test.cjs' | Sort-Object Name | Select-Object -ExpandProperty FullName; node --test $tests
```

Primera ejecución: 64 archivos; código de salida `1`. Falló `contrato incluye mínimos documentales y excluye dato_lista` porque la aserción inspeccionaba todo el archivo y confundía el DTO aditivo 1.1 (`ElectronicIndexSqlStatus`) con el contrato v1 protegido. No se omitió ni deshabilitó la prueba: se acotó a `ImportItemResultDto`.

Reejecución completa: 64 archivos; código de salida `0`; 278 pruebas, 278 aprobadas, 0 fallidas, 0 omitidas y 0 canceladas.

El gate versionado permaneció en `false`, con usuarios y grupos vacíos.

## Plataforma E2E DOC-56 ampliada para DOC-67

Comando PowerShell:

```text
$tests = Get-ChildItem tools/e2e/tests -Filter 'workflow-e2e-platform*.test.cjs' | Select-Object -ExpandProperty FullName; $tests += (Resolve-Path 'tools/e2e/tests/importar-servicio-web-modern.spec.cjs').Path; node --test $tests
```

Resultado real: código de salida `0`; 27 pruebas, 27 aprobadas y 0 fallidas. Los cuatro controles DOC-67 adicionales usan una única consulta `SELECT` parametrizada por tarea para expediente, relación, caché e índices SQL/XML. La evidencia conserva únicamente el identificador del control y si su huella cambió; no expone filas ni valores de las huellas.

## Reporte de las 18 aserciones DOC-67

Se reejecutó la suite de plataforma anterior después de distribuir las 18 aserciones entre `import-sii-execution`, `import-sii-retry`, `import-sii-recovery` e `import-sii-concurrency`.

Resultado real: código de salida `0`; 28 pruebas, 28 aprobadas y 0 fallidas. Cada aserción se registra por separado con identificador `DOC67-E2E-01` a `DOC67-E2E-18`, escenario, estado cerrado `passed|failed|blocked`, conteos y código técnico. La plataforma rechaza campos adicionales y no conserva datos del radicado, matrícula, expediente, documento, secretos o cuerpos externos. Una evidencia ausente permanece `blocked` hasta una corrida autorizada.

## Perfiles DOC-56 de retry y recovery

Primera ejecución focal: código de salida `1`; 13 pruebas, 12 aprobadas y 1 fallida. El fixture de autorización de `retry` aún heredaba campos propios de ejecución y fue rechazado correctamente por el cargador estricto. Se corrigió el fixture para usar el perfil mínimo dedicado, sin relajar la validación.

Reejecución de la suite de plataforma: código de salida `0`; 28 pruebas, 28 aprobadas y 0 fallidas. Los perfiles disponibles cubren ejecución, retry, recovery y concurrencia. `retry` y `recovery` aceptan únicamente tarea, intención opaca, ambiente y presupuestos comunes; rechazan radicado, código de barras, tipología, tamaño de muestra o concurrencia que no corresponden. El escenario paralelo `import-sii-expedient-execution` continúa sin estar registrado.
