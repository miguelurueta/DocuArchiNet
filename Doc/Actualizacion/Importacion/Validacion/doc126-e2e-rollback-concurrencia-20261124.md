# DOC-126: rollback y concurrencia reales aprobados

Estado DOC126_REAL_UI_RESILIENCE_PASS. Resultado 2026-11-25T04:39:08.7522174Z; base 12ae034eca355c9d301250644908c7f210b35693. Un test Playwright real aprobado, dos casos por controles de interfaz. Tarea6.11 completada:58/62. Permanecen6.9/6.10/7.5/8.8 en sus alcances originales; se mantiene la creación/importación manual ya validada por el usuario.

Usuario141, empresa2, DA11/RA12, conexión6, HTTPS local/GESTOR del acceso previamente verificado. El usuario indicó continuar después de proponer rollback y concurrencia con esa cuenta. Se reutilizaron runner, perfil local, E2E, archivos, plantillas58/59, lector, controles SELECT y actor nativo DOC125. Sin instalaciones nuevas, siembra, limpieza o modificaciones de permisos, plantillas, Web.config o DLL del producto. Contraseña fresca en consola privada, descartada al terminar.

| Caso | Plantilla | Carga | Entrada | Antes | Después | Resultado esperado |
| --- | --- | --- | --- | --- | --- | --- |
| Rollback | 59 | e18c33ab-c248-402c-8309-480d1cd93ae7 | 300 | 1000 | 1000 | FALLIDA / CONFLICTO_CLAVE / 0 confirmadas |
| Concurrencia | 58 | 47ebdd39-83e0-40a7-9769-dbc5a1bddd9a | 10 | 1001 | 1002 | FALLIDA / CONFLICTO_CLAVE / 0 propias confirmadas |

Rollback: el archivo existente contiene un VALOR repetido en la fila final, con claves distintas. El preflight leyó toda la hoja y verificó el UNIQUE secundario que permite provocar el fallo durante escritura. El intento de reemplazo revierte DELETE y los lotes previos: coincide la huella íntegra del destino antes/después, no sólo su conteo1000.

Concurrencia: el actor independiente inserta únicamente la primera fila del archivo, con la misma clave que intenta importar el cargador, y mantiene su transacción abierta. Se observó el INSERT real del importador bloqueado en PROCESSLIST y dos propietarios distintos de locks de carga/destino mientras seguía vigente esa espera. Sólo después se confirmó la fila externa. Evidencia nativa: locked/importWaitObserved/activeLocksObserved/committed=true, exitCode0. Se evitan inferencias desde una consulta de locks que llegue después de liberar la transacción.

La importación propia se revierte: queda únicamente una fila externa adicional. Al excluir su clave mediante parámetro, la huella de las1001 filas anteriores coincide íntegramente con la inicial; se conservan datos y metadatos. Se verificaron resultado durable, identidad de archivo/plantilla/usuario, consentimiento de reemplazo, una confirmación por intención y locks liberados. Las dos cargas positivas previas siguen CONFIRMADA con1000 filas cada una y sus mismos archivos/IDs.

Coincidieron las huellas íntegras de27 tablas protegidas y todas las cabeceras/campos de plantilla. Las cargas de esta prueba figuran FALLIDA intencionalmente; los tests están aprobados precisamente porque acreditan los fallos y la atomicidad esperados. No se atribuye rollback o concurrencia a una simulación o a un simple conteo.

El nuevo modo resilience tiene autorización propia DOC126_RESILIENCE_E2E_AUTHORIZED, no heredada de funcional/read/write. Scope exacto:300+10 filas propias más una externa (máximo311),1000 borrados intentados que deben revertirse, dos destinos/archivos/plantillas fijados y máximo una fila externa persistida. El actor valida archivo/snapshots/propietario/esquema y sus guards antes de actuar. Los modos anteriores conservan sus restricciones de cuenta.

Validación local:41/41 pruebas Node y4607 checks nativos sintéticos (DOC122307, DOC123368, DOC124492, DOC1253440), incluidos cuatro casos de propietarios de locks presentes/ausentes/iguales/distintos. Se reutilizó y compiló el harness existente de forma aislada, sin publicar DLL. La ejecución real posterior acredita los dos casos; el tiempo7.7min del test incluye controles de27 tablas antes/después y no mide rendimiento por fila.

Hashes de fuente/runtime/assets y evidencia SELECT en el JSON adjunto. Datos de ejecución, perfil, actor, staging y secretos efímeros quedan en ubicaciones locales excluidas. Conservar fixtures y resultados: agregar contiene1002 y reemplazar1000; la clave externa ya existe, por lo que este scope no autoriza repetir ciegamente el actor. SQL23–25 ya estaban aplicados y no se reaplicaron.
