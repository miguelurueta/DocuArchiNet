# DOC-117 — Preparación ALM-02 lista para revisión

Estado actualizado: **EJECUTADO CON AUTORIZACIÓN ESPECÍFICA, PASS**, plan 37/lote 21, expedientes 664/665, dos recepciones/materializaciones, 100/160 SELECT y cero almacenamiento o cambio de DLL. Ver `doc117-alm02-setup-pass-20261106.md/.json`. El resto de este documento conserva el alcance presentado antes de esa autorización; la aprobación editorial y el preflight por sí solos no la concedían. No repetir la preparación.

## Alcance propuesto

Ambiente `https://localhost/GestionDocumental-Docuarchi.net/`, cuenta de aplicación correspondiente a actor 141/empresa 2, nivel 7/versión 38/gabinete 10. Controles por los DSN RA/DA ya registrados, con cuenta MySQL exclusivamente SELECT ingresada por consola. La cuenta de aplicación se contrasta mediante el lote propio 19 y el destino actual. Conservar la política TLS del perfil local; si admite el certificado local, su uso requiere quedar comprendido en la autorización.

| Efecto máximo | Presupuesto |
| --- | --- |
| Preparar plan / confirmar lote | 1 / 1 |
| Recibir originales PDF | 2, de 677 bytes cada uno |
| Crear expedientes | 2 |
| Materializar valores | 2 |
| Documentos definitivos / solicitudes ALMACENAR | 0 / 0 |
| Instalación o cambio de DLL | 0 |
| Consultas SELECT | Máximo 160, sin reintentos |

Raíces: `DOC117-ALM02-A-20261106` y `DOC117-ALM02-B-20261106`. En cada una, original `DOC117-ALM02-20261106.pdf`, proveniente del fixture existente `doc111-carga-individual.pdf`, SHA256 `a5f1924afbae4bd631de893c49a56d9f09219075113e6da2ec983602cff84c6c`. Tipología 79 e índice 14/TEXTO/`EEE` son expectativas históricas que el driver contrasta con publicación, preparación y valores fijados antes de declarar disponibilidad. Una discrepancia detiene el recorrido.

Propuesta completa y presupuesto: `doc117-alm02-recursos-propuestos-20261106.json`.

## Compuertas implementadas

El driver reutiliza el login, transporte, receptor, controles SELECT, preservación y lectura de originales existentes. Revalida ausencia de nombres, publicación exacta del preflight y tipología antes de preparar; comprueba plan elegible y valores antes de confirmar; recibe y verifica ambos originales antes de crear. Conserva IDs conocidos en un diario antes de cada efecto y no reintenta una respuesta incierta.

Solo recorre CREAR_EXPEDIENTE y MATERIALIZAR_VALORES. Al terminar, exige los expedientes/relaciones propios y valores exactos, archivos RECIBIDO/NO_INICIADO, originales íntegros, ningún intento ALMACENAR ni documento/incorporación y huellas intactas de recursos previos, incluidos lote 18/intento 41 y lotes 19/20. La DLL original debe mantener su hash durante todo el recorrido. No cambia permisos, configuración, esquema ni recursos anteriores; no limpia nada.

Con los IDs y revisiones ya conocidos calcula las dos huellas documentales mediante la consola .NET compilada. Conserva un manifiesto de ejecución privado bajo `.tmp` con los IDs, revisiones y claves de operación generadas, sin credenciales, cookies o contexto completo. La evidencia pública solo contiene referencias y conteos. Ese manifiesto no autoriza enviar las solicitudes de almacenamiento.

La candidata específica se construirá y revisará después con ese alcance, antes de solicitar activación/almacenamiento/fallos. No se instala una candidata desactivada ni sintética para descubrir identidades.

## Verificación local y comando preparado

`node --test tests/doc117-alm02-setup.test.cjs`: 10/10 PASS con dobles. Incluye flujo acotado, plan existente, fallo del diario, respuesta perdida, creación incierta, valores discordantes, documento detectado, cambio en recursos protegidos, cálculo fallido y rechazo de permisos exclusivamente editoriales o SELECT.

El modo `node tools/e2e/scripts/run-doc117-alm02-setup.cjs --check` acredita forma y huellas sin autenticación o efectos. El calculador .NET está compilado en `.tmp/doc117-alm02-hash-tool-reviewed-20261106`, con hashes del ejecutable, modelo y serializador. La autorización concedida posteriormente ya se consumió en la corrida documentada.

Comando real preparado, para ejecutar únicamente tras registrar la autorización específica y abrir la consola de captura enmascarada:

```powershell
node tools/e2e/scripts/run-doc117-alm02-setup.cjs --profile tools/e2e/profiles/doc111-importacion-carpetas-lote.runtime.json --authorization .tmp/doc117-alm02-setup-authorization.json --hash-tool .tmp/doc117-alm02-hash-tool-reviewed-20261106/Doc117SolicitudHash.exe
```

No repetir el comando después de un resultado incierto o un recurso creado. La siguiente actuación será consultar/revisar las referencias conocidas, manteniendo cero solicitudes de almacenamiento hasta otra autorización.
