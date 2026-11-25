# DOC-127 — Propuesta de aceptación de lectura vigente

Estado **EJECUTADA_PASS** tras la respuesta expresa «autorizado»: una E2E real,1/1 PASS, sin reintentos, finalizada2026-11-25T23:28:09Z. [Resultado y límites](doc127-lectura-real-20261125.md). Se reutilizaron el runner DOC110, la suite Excel y el validador DOC126 en modo `read`, con un scope nuevo identificado como aceptación DOC127. La ventana queda consumida; no autoriza otra ejecución.

| Recurso | Alcance concreto |
| --- | --- |
| Ambiente | IIS local HTTPS, `https://localhost/GestionDocumental-Docuarchi.net/`, módulo GESTOR, empresa2, DA11/RA12 |
| Cuentas | asesor.comercial194, sólo cargas; oficial.cumplimiento196, sin cargas. Permisos actuales conservados |
| Recursos | Referencia de conexión6, plantilla60, destino existente `imp_02_doc126_e2e_asistente_c38a2522` |
| Acciones | Una ejecución de la E2E de lectura: dos accesos autenticados; verificar denegación, abrir/listar/buscar/consultar plantilla con194 y cerrar con Escape |
| Integración | Comprobar en el DOM real que Excel no participa en la validación de `form1`, antes de abrir y después de cerrar. No se envía un postback del gestor |
| Conservación | Huellas antes/después de32 tablas y configuración de plantillas/campos. Comprobación posterior de78 perfiles y cinco resultados previos mediante SELECT existente |
| Efectos funcionales | Cero archivos recibidos, plantillas guardadas, preparaciones, confirmaciones, filas insertadas/borradas o cambios de permisos |
| Versión | Script doc127-1 y hashes del runtime/assets/página/configuración verificados; credenciales sólo en consola privada y memoria |

El scope y la consola privada permanecen en `.tmp/doc127-read-preparacion/` y `.tmp/doc127-read-session.ps1`, excluidos de Git. La consola recibió las dos contraseñas sólo en memoria y limpió sus variables al terminar. Las32 tablas/configuración conservaron sus huellas;78 perfiles y cinco resultados previos coincidieron con la línea base fresca. Se registró una diferencia de huella histórica de perfiles anterior al login, sin atribuir su causa a esta ventana ni modificar permisos. Un error detiene la ejecución sin reintento automático.

Esta ventana comprueba lectura e integración de formularios; no acredita toda5.1 (autoridad/preparación adicional), capacidad o creación web. Las18 combinaciones XLS/XLSX de1000/5000/10000 filas siguen siendo ventanas separadas pendientes; esta propuesta no las autoriza. EXPLAIN29 también conserva su estado pendiente manual.
