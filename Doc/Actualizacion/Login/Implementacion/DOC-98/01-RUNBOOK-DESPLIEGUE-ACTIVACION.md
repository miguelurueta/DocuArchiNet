# Runbook de despliegue y activación

## Responsabilidades

| Rol | Responsabilidad | No autorizado |
|---|---|---|
| Responsable de cambio | Aprueba ambiente, ventana, empresas, módulos y oleadas | Compartir secretos o aprobar implícitamente otro ambiente |
| DBA operador | Ejecuta preflight, snapshot y scripts acotados | Ejecutar un `UPDATE` sin IDs y valores esperados |
| Operador de aplicación | Despliega paquete compilado y configuración protegida | Imprimir llaves, credenciales, cookies u OTP |
| QA autorizado | Ejecuta smoke/E2E con cuentas y buzón descartables | Usar cuentas reales sin autorización expresa |
| Observador | Revisa métricas agregadas y decide avance | Consultar o publicar datos personales |

Una misma persona puede asumir varios roles, pero cada autorización y resultado debe quedar registrado de forma saneada.

## Precondiciones comunes

1. Identificar exactamente ambiente, catálogo central, empresas, módulos y versión desplegable.
2. Obtener autorización expresa para desplegar; obtener una autorización adicional antes de cualquier mutación o E2E real.
3. Respaldar las filas objetivo de `gestor_modulos` con sus tres valores 2FA y conservar el artefacto fuera del repositorio si contiene datos del ambiente.
4. Ejecutar `Sql/01-preflight-readonly.sql`. No continuar si faltan columnas/índices, si la tabla de challenges no satisface el contrato o si SMTP activo no tiene cardinalidad uno.
5. Confirmar sin revelar valores:
   - `LoginSecondFactorHmacActiveKeyId` está definido.
   - Existe `LoginSecondFactorHmacKey.<keyId>`, es Base64 y decodifica al menos 32 bytes.
   - `<sessionState mode="InProc" timeout="10" />` conserva diez minutos.
   - El paquete compila y las pruebas locales indicadas en `04-VALIDACION-Y-EVIDENCIA.md` pasan.
6. Ejecutar el diagnóstico agregado de principales. Se usa para anticipar fallos de datos; no habilita, excluye ni bloquea usuarios individualmente.

## Fase 1 — Compatibilidad con 2FA apagado

- Precondición: filas objetivo en `RequiereSegundoFactor=0` o `NULL`.
- Acción: desplegar los binarios y configuración, reiniciar de forma controlada y probar login legacy representativo de los cuatro tipos de módulo.
- GO: el login legacy conserva contrato, ruta local y cookie esperados.
- NO-GO: cualquier regresión de login, resolución de empresa/módulo o inicialización.
- Aborto: restaurar paquete/configuración anterior; no tocar datos 2FA.

## Fase 2 — Piloto

- Precondición: Fase 1 aprobada; SMTP exactamente uno y válido; llaves instaladas; cuenta/buzón piloto autorizados; snapshot capturado.
- Acción: completar `Sql/02-pilot-activation.sql` con una sola fila y habilitar explícitamente `@doc98_authorized=1` únicamente en la sesión aprobada.
- GO de SQL: preselección cardinalidad uno, `DOC98_AFFECTED_ROWS=1` y postcondición exacta.
- Acción posterior: ejecutar login, entrega, OTP válido, inválido, reenvío, expiración, cancelación, refresh y doble envío según la E2E vigente.
- GO funcional: no hay cookie antes de validar; hay una sola finalización; el destino permanece enmascarado; el login se completa después del OTP válido.
- NO-GO: ambigüedad, ausencia de correo, error de entrega, aumento de fallos, cookie anticipada o divergencia del login legacy.
- Aborto: ejecutar `Sql/03-pilot-disable-rollback.sql` con el snapshot exacto y dejar challenges pendientes expirar naturalmente.

## Fase 3 — Oleadas transversales

- Precondición: piloto aprobado durante la ventana de observación definida por el responsable del cambio.
- Acción: construir un manifiesto explícito por oleada con cada `ID_EMPRESA`, `ID_MODULO`, nombre/tipo, valores anteriores y valores objetivo.
- Acción: ejecutar `Sql/06-global-wave-activation.sql`; cada fila se activa por módulo, nunca por usuario.
- GO: filas candidatas = manifiesto, filas modificadas = manifiesto y postcondiciones = manifiesto.
- NO-GO: diferencia de cardinalidad, valor concurrente, fila fuera del alcance, fallo funcional o métrica fuera del umbral aprobado.
- Aborto: detener oleadas siguientes y restaurar únicamente la oleada fallida con snapshots por fila.

## Fase 4 — Cierre

- Verificar con `SELECT` que todas las filas del manifiesto están en `1/1/1..10`.
- Comparar snapshot antes/después y confirmar cero filas ajenas modificadas.
- Ejecutar smoke autorizado de al menos un principal por cada tipo de módulo presente.
- Registrar métricas agregadas, versión, hora UTC, conteos y decisión; nunca registrar OTP, correo, login, `AuthUserId`, `CodeHash`, llaves, cookies o conexiones.
- Declarar por ambiente una de dos salidas: `ACTIVADO` con evidencia, o `BLOQUEADO` con la puerta exacta. Preparar scripts no equivale a activar.

## Regla transversal

La única puerta de activación en ejecución es la configuración resuelta por `OdbcSecondFactorLoginModuleRepository` desde `gestor_modulos` para empresa y módulo activos. Los repositorios de principales no contienen ni deben adquirir una decisión de activación.
