# DOC-111: lote 2 completado y corrección del control E2E

Actualización 2026-11-02: **sesión 21 PASS** para lectura autenticada, verificación SELECT y reapertura sin cambios. [Evidencia vigente](doc111-lote2-verificacion-pass-20261102.md). Resuelve esas comprobaciones pendientes en los cortes históricos inferiores; la repetición final de comandos sigue pendiente y no se acredita REC completo por LOTE.

## Aprobación manual del usuario — 2026-11-01

Tras contrastar por SELECT el índice TIPO (referencia 14) con valor EEE y la presencia de clase, expediente, fecha, tipología, serie y subserie en los documentos 16204/16205, el usuario confirmó: «si quedaron bien aprobado». Se registra la aprobación manual de los índices de ambos documentos del lote 2 y su resultado correcto. Evidencia de la comprobación: `tools/e2e/artifacts/doc111-document-indices-readonly.json` (15:18:09 UTC).

Esta aprobación corresponde a los índices comprobados; no se amplía a pruebas REC completas de otros modos, replay mutante o reapertura autenticada que no se ejecutaron. No se repiten cargas, no se cambia el estado del lote y no se declara cerrado todo DOC-111 por esta aceptación parcial.

Actualización: la sesión 14 se detuvo al iniciar sesión, antes de la verificación final/reapertura. El lote continúa COMPLETADO/revisión 25; no se repitieron efectos. [Estado del acceso y comprobaciones posteriores](doc111-sesion14-login-20261101.md).

## Resultado persistido

SELECT del 2026-11-01, 12:11:22 UTC: **lote 2 COMPLETADO, revisión 25**, dos raíces COMPLETADA y ambos archivos recibidos/COMPLETADO, sin reclamaciones activas. Expedientes/relaciones originales conservados:

| Expediente | Relación | Intento documental | Documento | Inventario |
|---|---|---|---|---|
| 648 | 1073 | 6 CONFIRMADO, reconciliado | 16204 | 23293 |
| 649 | 1074 | 7 CONFIRMADO | 16205 | 23294 |

La sesión 13 confirmó la reconciliación del intento 6, materializó los valores del segundo archivo y almacenó/incorporó el segundo PDF una sola vez. Eventos 21–29 muestran reconciliación, raíz 3 completa, continuación, nuevo intento 7 y completitud final. El intento 5 conserva su rechazo por cierre sin inicio. No se creó otro lote ni expediente y no se volvió a almacenar el primer PDF.

Los verificadores productivos finales, invocados por SELECT contra los IDs persistidos, pasaron para ambos documentos a las 12:16:08/12:16:13 UTC. Reportes `tools/e2e/artifacts/doc111-document-16204-final-readonly.json` y `doc111-document-16205-final-readonly.json`. Estado completo y diario en `tools/e2e/artifacts/doc111-lot2-completed-readonly.json`. No ejecutaron checkpoint, reconciliación, almacenamiento o replay.

## Fallo del control y corrección

La sesión 13 terminó con `DOC111_DOCUMENT_VALUES_INVALID` en la primera verificación SELECT final, después de completar ambos documentos. La consulta pedía `r.FORMATO` e `i.formato` sin alias distintos. El puente existente arma cada fila con un diccionario PowerShell que no distingue mayúsculas; las dos columnas colisionan y el objeto JSON no puede conservar ambas claves. El mock JavaScript sí las distinguía y ocultaba el problema.

La consulta y comprobación usan ahora `inventory_format` e `index_format`. Se mantienen ambos requisitos `.PDF` y todos los demás valores/nombres/relaciones/índices. No se altera el puente compartido, el documento o la DLL del sitio para hacer pasar el control. La regresión reproduce con PowerShell real la colisión y la conservación de alias distintos, y rechaza un formato incorrecto en cualquiera de los dos campos.

Se agrega `--scenario LOTE --verify-lot 2` al runner existente: solo sesión, consulta, SELECT y reapertura. Mantiene efectos/descartables sin autorización; un driver restrictivo rechaza preparación, confirmación, fases, reconciliación y recepción antes de llamar al transporte. El resultado `DOC111_COMPLETED_VERIFICATION_RESULT` acredita únicamente datos completos y reapertura sin cambios. No ejecuta ni acredita replay mutante; nunca se emite REC PASS por esta variante.

## Comprobaciones y límites

- **54 pruebas locales PASS**: driver/sesión/controles/ODBC, incluida reproducción PowerShell real; casos de ambos formatos y guard de lectura sin efectos.
- Selector `--check` de verificación completada PASS, sin ejecución real.
- DLL vigente sin cambios: `73e7bcc9476172ae6cf06be53099d06506b2f6ea89fe6a83b311296b1197f176`. No se compiló ni desplegó otra DLL para un cambio exclusivo del harness.
- 27 archivos protegidos idénticos a HEAD normalizando finales de línea, incluidos almacenamiento legacy y Web.config.
- Manifiesto `doc111-sha-control-final-20261101.json`, 110 archivos, fuentes SHA-256 `9b235816d3082e78b67808e5fa555da737ac0d08f566e2883bfa0ac3b0382fcf`. La diferencia de fuentes respecto del despliegue corresponde al harness.

Consola de lectura preparada `.tmp/doc111-lote2-verify-console-14.ps1`, con estado saneado `tools/e2e/artifacts/doc111-lote2-verify-session-14.json`. Autorizaciones previas conservadas únicamente para ambiente/cuenta/SELECT/certificado; credenciales nuevas solo en TTY enmascarado, sin persistir secretos. No solicita autorización de efectos, URL MySQL ni datos privados en chat.

**121/130 tareas.** La carga real del lote 2 está completada; la sesión 13 no es PASS de aceptación porque falló el control final antes del replay del segundo comando y la reapertura. La verificación autenticada con cuenta exclusivamente SELECT y la reapertura quedan pendientes de la sesión 14. No se registra REC completo ni se cierran las demás pruebas EXPEDIENTE/PARCIAL/revisión por estos resultados.
