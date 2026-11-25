# DOC-126: asistente e importación CREAR reales aprobados

Estado DOC126_REAL_UI_ASSISTANT_PASS. Final 2026-11-25T05:17:13.4764149Z; base Git 5046842b05bad4ec1e8b85a8c9d8f09df9f04e92, hashes de los archivos ejecutados en el JSON adjunto. Un test Playwright real aprobado en IIS local HTTPS/GESTOR con LUZ.AGUILERA, usuario141, empresa2, DA11/RA12 y conexión6. Reutiliza runner, E2E, autenticación, libro, analizador y recursos existentes. Contraseña efímera en consola privada, descartada al terminar.

Se creó la plantilla **60**, con dos campos y clave CLAVE, desde el archivo existente de1000 filas. Destino nuevo **imp_02_doc126_e2e_asistente_c38a2522**, carga **9d7a1ee7-f7f8-4592-ad34-0625bb77f80c**, estado CONFIRMADA y1000 filas verificadas mediante SELECT. El analizador acreditó hoja Datos, huella del libro y tipos CLAVE VARCHAR(63)/VALOR VARCHAR(57). Los defaults nuevos son ceros explícitos compatibles con la instalación.

Los controles visibles recorrieron menú, Crear desde Excel, análisis, elección de clave, Guardar, Recibir, Validar, Confirmar y Consultar resultado. Guardar no creó la tabla ni importó filas; recibir requirió otra acción explícita con el mismo File retenido sólo en memoria. El doble click envió una confirmación. El scope limitó exactamente una plantilla, dos campos, un destino,1000 filas y cero borrados.

Las huellas íntegras de29 tablas protegidas coincidieron antes/después, incluidos los fixtures agregar1002 y reemplazar1000. Las cabeceras y campos anteriores conservaron sus huellas al excluir únicamente la nueva plantilla. Una conexión nueva verificó el resultado, propietario, campos, permisos intactos y los cuatro resultados anteriores: dos CONFIRMADA y dos FALLIDA esperadas. No se instalaron SQL, sembraron/limpiaron datos ni cambiaron permisos, Web.config, DLL o código productivo.

Se cierran **6.10 y8.8:60/62 tareas**. La matriz de tres modos combina este CREAR real con AGREGAR/REEMPLAZAR reales del corte anterior,1000 filas confirmadas por modo. La validación manual del usuario permanece reconocida por separado. Rollback y concurrencia conservan su evidencia de6.11.

El modo assistant admite la cuenta vigente sólo con política EXISTING_AUTHORIZED explícita y sus cinco permisos fijados. Conserva autorización DOC126_ASSISTANT_E2E_AUTHORIZED propia, guards de solicitudes exactas y perfil exclusivo predeterminado. Regresión de los adaptadores afectados43/43 PASS, sin cambiar producto. Esta ejecución no acredita permiso exclusivo/restringido ni un corte físico de red/COMMIT, tampoco volumen20000 o rendimiento por fila.6.9 y el cierre de gobierno7.5 continúan abiertos.

Conservar plantilla, tabla y carga. El destino ya existe: este scope no autoriza repetir CREAR ni borrar para preparar otra prueba. Runtime/assets y evidencia SELECT saneada figuran en el JSON; staging, perfiles y ejecución quedan excluidos de Git.

Revisión posterior:40/40 pruebas Chromium locales PASS, con transporte sintético explícito. Se amplía el fixture existente para XLS incompatible con plantilla XLSX y10000001bytes frente a10MB decimales: ambos rechazos acreditan cero IniciarRecepcionExcel/Upload/Preparar/Confirmar y confirmación deshabilitada. Se resuelve el hueco RQ-05 detectado en revisión sin cambiar código productivo ni repetir escrituras reales.
