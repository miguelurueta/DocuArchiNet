# DOC-126: E2E funcional real aprobada

Estado DOC126_REAL_UI_FUNCTIONAL_PASS. Base 0a3117662d78f69c1314989593abd4e0796671d2; resultado 2026-11-25T04:14:01.3613567Z. Un test Playwright real ejecutado y aprobado, con dos casos por controles de la interfaz. Usuario141, empresa2, DA11/RA12 y conexión6, con permisos actuales conservados. Se reutilizaron runner, perfil como contrato, navegación, E2E Excel, generador/archivos sintéticos y controles SELECT existentes.

| Caso | Plantilla | Carga | Filas iniciales | Importadas | Filas finales |
| --- | --- | --- | --- | --- | --- |
| Agregar | 58 | 36f39f62-c6d3-49cb-9d14-ebf61e7e2e02 | 1 | 1000 | 1001 |
| Reemplazar | 59 | 31ca6cd5-716e-4829-a0ca-eac7700192d6 | 2 | 1000 | 1000 |

Ambas cargas CONFIRMADA y propiedad2/141/DA:11:6 verificadas por SELECT posterior. La fila sintética inicial de agregar permanece; las dos filas iniciales de reemplazar ya no están. Se verificaron archivo/hash, revisión/huella, recepción, validación completa, consentimiento obligatorio de reemplazo, una confirmación por intención, recuperación de resultado, conteos y liberación de locks. No se sustituyeron los clicks de importación por ASMX directo.

Las huellas completas de27 tablas protegidas coincidieron antes/después, incluyendo la importación manual del usuario. Coincidieron las huellas de todas las cabeceras/campos de plantilla preexistentes. Se preservan los nuevos resultados y fixtures; no hay limpieza o reinstalación. Las dos fuentes/políticas y plantillas de fixture fueron preparadas por SQL23–25 bajo autorización específica anterior. No se prueba autorregistro de políticas o publicación automática de fuentes.

El usuario aclaró que su prueba manual se hizo al lanzar IIS. Se contrastó el IISUrl HTTPS del proyecto y la aplicación física de IIS; el perfil compartido anterior usaba HTTP. Se creó un perfil local separado, con URL https://localhost/GestionDocumental-Docuarchi.net/, certificado local no confiable aceptado sólo para ese contexto, módulo GESTOR seleccionado por el usuario en consola privada y contraseña fresca. El perfil compartido DOC88, Web.config, DLL y permisos no se modificaron. El runner expone runExcelUi existente para recibir ese perfil sin cambiar sus guards o CLI predeterminado. Se acredita la alineación del acceso y el resultado, sin deducir que HTTPS fuera por sí solo la causa del rechazo anterior.

Se conservan los dos intentos previos: el primero detuvo el control de una columna histórica Unicode antes de login; el segundo completó27 huellas iniciales y recibió LOGIN_CREDENTIALS_REJECTED. La corrección del helper compartido admite AÑO_FERIADO con validación cerrada, conserva huellas íntegras, paraleliza tres metadatos independientes y muestra progreso por conteos. Regresión afectada38/38 PASS. En el tercer intento se acreditó el resultado funcional real; los fallos anteriores no se cuentan como éxitos. Las claves se descartaron al terminar cada consola y nunca se guardaron o imprimieron.

El runtime/DLL/assets coincidió con el scope al finalizar. El tiempo total reportado7.2min incluye controles completos de27 tablas antes/después y navegación; no es una medición de rendimiento de importación por fila. Evidencia por hashes de fuente en el JSON adjunto; datos de ejecución, perfiles y salida privada excluidos de Git.

Alcance: agregar y reemplazar con cuenta existente aprobados. Crear plantilla e importar siguen aceptados por la validación manual del usuario. Este resultado no acredita permiso exclusivo/restringido, rollback, escritor externo ni creación automatizada por el asistente; las tareas originales de esos alcances permanecen separadas. Estado formal57/62, con avance real de6.10 registrado sin atribuir tres modos automatizados a dos casos.
