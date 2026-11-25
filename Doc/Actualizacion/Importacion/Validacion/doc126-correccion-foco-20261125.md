# DOC-126: corrección del foco descubierta en lectura real

Estado DEPLOYED_FOCUS_FIX_REAL_REREAD_PENDING. La primera E2E read autenticó las cuentas194/196, comprobó rechazo de la restringida y abrió/listó/consultó con permiso exclusivo. Falló en **DOC126_ESCAPE_FAILED**, sin alcanzar las huellas finales. No se declara PASS por haber comprobado sólo parte del recorrido.

La causa se reprodujo en el fixture browser existente: Buscar/seleccionar plantilla deshabilita el control enfocado mientras espera la respuesta. El navegador traslada el foco al cuerpo y el keydown deja de pasar por el diálogo. La prueba local nueva falla antes del cambio; no se corrigió la E2E enfocando artificialmente Cerrar ni omitiendo Escape.

Servicio/contratos/SQL intactos por esta corrección. El cliente conserva el foco anterior, coloca temporalmente el foco en el diálogo antes de deshabilitar los controles y lo restaura al control visible/habilitado cuando termina la misma generación. Si el control desapareció o quedó deshabilitado, utiliza Cerrar. No roba un foco nuevo establecido por el usuario/callback, ni restaura foco al cerrar o recrear el host porque conserva el guard epoch.

**48/48 PASS:**41 pruebas Chromium con transporte sintético y7 de cliente. Incluye Buscar→seleccionar→Escape, callbacks tardíos, cierre durante actividad, tres modos, consentimiento, CAS, incertidumbre, storage, teclado y viewport/zoom. Los44 tests de adaptadores ya aprobados cubren lectura sin efectos y30 tablas protegidas; no se atribuyen estas pruebas locales a otra ejecución IIS/MySQL.

Assets **doc126-6** servidos por IIS, con hash HTTP igual al local y respaldo exacto de Vista.js/ASPX v5. Sólo Vista.js y versión de referencias ASPX cambian en producto; API/CSS mantienen sus bytes y DLL/Web.config permanecen iguales. No se requiere build VB, SQL adicional ni reaplicar el permiso.

SELECT posterior al intento fallido confirmó que los78 perfiles conservan íntegramente la huella posterior a la única concesión autorizada, fixtures crear1000/agregar1002/reemplazar1000 y cinco resultados anteriores. Este control de conteos/resultados no se presenta como las30 huellas finales que el test abortado no completó.

Se prepara otro intento read con los mismos194/196, cero efectos y30 tablas; scope nuevo con hashes v6, autorización ya concedida y contraseñas frescas en consola privada. La primera consola descartó sus secretos; no se reutilizan ni se almacenan. Resultado de la repetición separado.60/62 tareas hasta acreditarlo; validación manual del usuario y casos positivos/rollback previos conservados.

## Cierre posterior y ajuste final mínimo

La repetición real **v6 pasó**: cuentas194/196, Escape tras buscar/seleccionar,30 huellas protegidas y configuración intacta. Ver `doc126-e2e-permisos-real-20261125.md`. En revisión final se elimina también la deshabilitación transitoria de Cerrar durante sync: se conserva habilitado desde el principio, para mantener su foco y Escape durante una apertura pendiente. Caso añadido al mismo fixture y49/49 pruebas locales PASS (42 browser,7 cliente), con transporte sintético. Assets **doc126-7** servidos con hash HTTP igual al local; DLL/API/CSS/SQL intactos.

Este último ajuste de una condición DOM se acredita localmente; **no se afirma otra E2E real después de v6**. La aceptación real de permisos/datos y la validación real de Escape después de consultas conservan los hashes v6; la cobertura adicional de apertura pendiente corresponde al browser local v7. Los dos cortes y sus fuentes quedan diferenciados, sin repetir importaciones ni conceder permisos otra vez. Regresión total actual93/93:44 adaptadores y49 UI/cliente.
