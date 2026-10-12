# Rotación, retención y rollback

## Rotación HMAC

El protector persiste `CodeHash` como `v1:keyId:mac`. La rotación es install-first/remove-last:

1. Instalar la nueva entrada `LoginSecondFactorHmacKey.<nuevoKeyId>` en todos los nodos, sin cambiar aún la llave activa.
2. Verificar de forma booleana que todos los nodos cargan llave anterior y nueva; no imprimir IDs ni material.
3. Cambiar `LoginSecondFactorHmacActiveKeyId` al nuevo ID y reciclar controladamente.
4. Mantener la llave anterior durante los 10 minutos máximos del challenge más el margen operativo aprobado.
5. Confirmar por conteo agregado que no quedan challenges elegibles protegidos con la llave anterior.
6. Retirar la llave anterior y volver a comprobar salud.

Nunca se retira primero la llave anterior. El ID y el material son sensibles y no forman parte de logs o evidencias.

## Retención

La implementación no posee scheduler. `Sql/07-terminal-cleanup-preview-readonly.sql` solo cuenta candidatos terminales con más de 30 días. Si el responsable autoriza una limpieza futura, debe existir un ticket separado, preview, límite de lote, transacción, respaldo, autorización específica y verificación posterior. DOC-98 no ejecuta ni incluye un borrado automático.

Un registro es terminal para esa evaluación si `Consumed=1`, `Attempts>=5` o `ExpiresAtUtc<=UTC_TIMESTAMP()`. Nunca se considera candidato un challenge todavía elegible.

## Rollback de configuración

1. Detener nuevas oleadas.
2. Restaurar por ID los tres valores capturados en el snapshot de cada fila afectada.
3. Exigir que el valor actual coincida con el objetivo aplicado; si cambió concurrentemente, afectar cero filas y escalar.
4. Verificar el conteo exacto y el login legacy.
5. Permitir expiración natural de challenges pendientes; no borrar tabla ni registros para “limpiar”.
6. Preservar evidencia agregada y paquete previo.

## Casos que disparan rollback

- configuración de módulo ausente, ambigua o inválida;
- SMTP ausente, ambiguo, inválido o entrega fallida por encima del umbral aprobado;
- verificación OTP no disponible o finalización inconsistente;
- cookie emitida antes del segundo factor;
- regresión del login legacy;
- cardinalidad SQL distinta de la manifestada;
- cambio de una fila fuera del manifiesto.

Un principal con identidad/correo inválido falla cerrado y se remedia en su fuente de datos. No se crea un bypass por usuario.
